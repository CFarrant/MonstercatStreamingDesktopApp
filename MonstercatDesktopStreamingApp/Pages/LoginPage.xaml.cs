using MonstercatDesktopStreamingApp.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace MonstercatDesktopStreamingApp.Pages
{
    public sealed partial class LoginPage : Page
    {
        #region Variables
        private string trackFileName = "tracks.db";
        private int apiCount;
        #endregion

        public LoginPage()
        {
            this.InitializeComponent();
            this.monstercatLogo.Source = new BitmapImage(new Uri("https://tr.rbxcdn.com/64807643d96804ad1d51f0446ca59c8a/420/420/Decal/Png"));
            BuildLocalAlbumAsync();
            BuildLocalTracksAsync();
        }

        public async Task<bool> isFilePresent()
        {
            var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(trackFileName);
            return item != null;
        }


        public static byte[] TrackToByteArray(Track t)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, t);
                return ms.ToArray();
            }
        }

        public static Track ByteArrayToTrack(byte[] t)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                ms.Write(t, 0, t.Length);
                ms.Seek(0, SeekOrigin.Begin);
                Track track = (Track)bf.Deserialize(ms);
                return track;
            }
        }

        #region Navigation
        private void ForgotButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (MainPage.window.CurrentSourcePageType != typeof(ForgotPage))
            {
                MainPage.window.Navigate(typeof(ForgotPage));
            }
        }

        private void RegisterButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (MainPage.window.CurrentSourcePageType != typeof(RegisterPage))
            {
                MainPage.window.Navigate(typeof(RegisterPage));
            }
        }
        #endregion

        #region Dialog Windows
        private async void DisplayInvalidLoginDialog()
        {
            ContentDialog invalidLogin = new ContentDialog
            {
                RequestedTheme = ElementTheme.Dark,
                Title = "User Login was Invalid",
                Content = "Please try again!",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await invalidLogin.ShowAsync();
        }
        #endregion

        #region API Calls
        private async void LoginButton_ClickedAsync(object sender, RoutedEventArgs e)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(@"http://www.monstercatstreaming.tk:8080");
                //httpClient.BaseAddress = new Uri(@"http://localhost:8080");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));
                httpClient.DefaultRequestHeaders.Add("Authorization", MainPage.Base64Encode(username.Text + ":" + password.Text));
                string endpoint = @"/api/user/login";
                //string endpoint = @"/user/login";

                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(endpoint);
                    string json = await response.Content.ReadAsStringAsync();
                    GETUser u = JsonConvert.DeserializeObject<GETUser>(json);

                    if (response.IsSuccessStatusCode)
                    {
                        MainPage.authentication = MainPage.Base64Encode(username.Text+":"+password.Text);
                        if (u.recovery == true)
                        {
                            MainPage.window.Navigate(typeof(ChangePage));
                        }
                        else
                        {
                            MainPage.window.Navigate(typeof(LibraryView), MainPage.albums);
                        }
                    }
                    else
                    {
                        httpClient.CancelPendingRequests();
                        DisplayInvalidLoginDialog();
                    }
                }
                catch (Exception) {}
            }
        }

        public int CheckTotalAPITrackCount()
        {
            int result = 0;

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(@"http://www.monstercatstreaming.tk:8080");
                //httpClient.BaseAddress = new Uri(@"http://localhost:8080");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));
                string endpoint = @"/api/song/count";
                //string endpoint = @"/album/" + albumId;
                string json = "";

                try
                {
                    HttpResponseMessage response = httpClient.GetAsync(endpoint).Result;
                    response.EnsureSuccessStatusCode();
                    json = response.Content.ReadAsStringAsync().Result;

                    result = int.Parse(json);
                }
                catch (Exception) { }
            }
            return result;
        }

        private async void BuildLocalTracksAsync()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file;
            bool dbIsValid = false;

            if (await isFilePresent() == false)
            {
                file = await folder.CreateFileAsync(trackFileName);
            }
            else if (await isFilePresent() == true)
            {
                file = await folder.GetFileAsync(trackFileName);
                var stream = await file.OpenAsync(FileAccessMode.Read);
                byte[] fileBytes = new byte[stream.Size]; 
                ulong size = stream.Size;
                List<Track> loaded = new List<Track>();

                var recievedStrings = "";
                using (var inputStream = stream.GetInputStreamAt(0))
                {
                    using (var dataReader = new DataReader(inputStream)) {
                        await dataReader.LoadAsync((uint)stream.Size);
                        
                        while (dataReader.UnconsumedBufferLength > 0)
                        {
                            recievedStrings += dataReader.ReadString((uint)stream.Size);
                        }
                    }
                }
                stream.Dispose();

                string[] readContents = recievedStrings.Split("\n");

                foreach(string s in readContents)
                {
                    if (!s.Equals(""))
                    {
                        byte[] bytes = Convert.FromBase64String(s);
                        loaded.Add(ByteArrayToTrack(bytes));
                    }
                }

                apiCount = CheckTotalAPITrackCount();

                if (loaded.Count == apiCount)
                {
                    dbIsValid = true;
                    MainPage.TRACK_COUNT = loaded.Count;
                    foreach (Track t in loaded)
                    {
                        MainPage.tracks.Add(t);
                    }
                }
            }
            
            if (dbIsValid == false)
            {
                List<Track> tracks = new List<Track>();
                int limit = 2000;
                int skip = 0;
                while(tracks.Count < apiCount)
                {
                    using (HttpClient httpClient = new HttpClient())
                    {
                        httpClient.BaseAddress = new Uri(@"http://www.monstercatstreaming.tk:8080");
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));
                        string endpoint = @"/api/song/" + limit + "/" + skip;
                        string json = "";

                        try
                        {
                            HttpResponseMessage response = httpClient.GetAsync(endpoint).Result;
                            response.EnsureSuccessStatusCode();
                            json = response.Content.ReadAsStringAsync().Result;

                            JArray jArray = JArray.Parse(json);
                            foreach (JObject item in jArray)
                            {
                                JProperty songArt = (JProperty)item.First.Next.Next.Next.Next.Next.Next.Next;
                                JObject alb = (JObject)item.Last.First;
                                JObject albArt = (JObject)alb.Last.First;

                                tracks.Add(new Track
                                {
                                    id = (string)item.GetValue("id"),
                                    tracknumber = (int)item.GetValue("tracknumber"),
                                    title = (string)item.GetValue("title"),
                                    genreprimary = (string)item.GetValue("genreprimary"),
                                    genresecondary = (string)item.GetValue("genresecondary"),
                                    songURL = (string)item.GetValue("songURL"),
                                    artist = new Artist()
                                    {
                                        name = (string)((JObject)songArt.First).GetValue("name")
                                    },
                                    album = new Album()
                                    {
                                        id = (string)alb.GetValue("id"),
                                        name = (string)alb.GetValue("name"),
                                        type = (string)alb.GetValue("type"),
                                        releaseCode = (string)alb.GetValue("releaseCode"),
                                        genreprimary = (string)alb.GetValue("genreprimary"),
                                        genresecondary = (string)alb.GetValue("genresecondary"),
                                        coverURL = (string)alb.GetValue("coverURL"),
                                        artist = new Artist()
                                        {
                                            name = (string)albArt.GetValue("name")
                                        }
                                    }
                                });
                            }
                        }
                        catch (Exception) { }

                        skip += 2000;
                    }
                }

                file = await folder.CreateFileAsync(trackFileName, CreationCollisionOption.ReplaceExisting);
                var stream = await file.OpenAsync(FileAccessMode.ReadWrite);
                using (var outputStream = stream.GetOutputStreamAt(0))
                {
                    using (var dataWriter = new DataWriter(outputStream))
                    {
                        foreach (Track t in tracks)
                        {
                            string temp = Convert.ToBase64String(TrackToByteArray(t)) + "\n";
                            dataWriter.WriteString(temp);
                            await dataWriter.StoreAsync();
                        }
                    }
                }
                stream.Dispose();

                MainPage.TRACK_COUNT = tracks.Count;
                foreach (Track t in tracks)
                {
                    MainPage.tracks.Add(t);
                }
            }
        }

        private async void BuildLocalAlbumAsync()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(@"http://www.monstercatstreaming.tk:8080");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));
                string endpoint = @"/api/album";
                string json = "";

                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(endpoint);
                    response.EnsureSuccessStatusCode();
                    json = await response.Content.ReadAsStringAsync();

                    JArray jArray = JArray.Parse(json);
                    foreach (JObject item in jArray)
                    {
                        JObject albArt = (JObject)item.Last.First;

                        MainPage.albums.Add(new Album()
                        {
                            id = (string)item.GetValue("id"),
                            name = (string)item.GetValue("name"),
                            type = (string)item.GetValue("type"),
                            releaseCode = (string)item.GetValue("releaseCode"),
                            genreprimary = (string)item.GetValue("genreprimary"),
                            genresecondary = (string)item.GetValue("genresecondary"),
                            coverURL = (string)item.GetValue("coverURL"),
                            artist = new Artist()
                            {
                                name = (string)albArt.GetValue("name")
                            }
                        });
                    }
                }
                catch (Exception) { }
            }
        }
        #endregion
    }
}