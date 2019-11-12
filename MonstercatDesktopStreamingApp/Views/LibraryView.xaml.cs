using MonstercatDesktopStreamingApp.Objects;
using MonstercatDesktopStreamingApp.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MonstercatDesktopStreamingApp.Pages
{
    public sealed partial class LibraryView : Page
    {
        public LibraryView()
        {
            this.InitializeComponent();
        }

        #region API Calls
        private List<Track> BuildQueriedTrackList(POSTTrack track)
        {
            List<Track> results = new List<Track>();

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(@"http://www.monstercatstreaming.tk:8080");
                //httpClient.BaseAddress = new Uri(@"http://localhost:8080");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));
                string endpoint = @"/api/song/search";
                //string endpoint = @"/album/" + albumId;
                string json = "";

                try
                {
                    HttpContent content = new StringContent(JsonConvert.SerializeObject(track), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = httpClient.PostAsync(endpoint, content).Result;
                    response.EnsureSuccessStatusCode();
                    json = response.Content.ReadAsStringAsync().Result;

                    JArray jArray = JArray.Parse(json);
                    foreach (JObject item in jArray)
                    {
                        JProperty songArt = (JProperty)item.First.Next.Next.Next.Next.Next.Next.Next;
                        JObject alb = (JObject)item.Last.First;
                        JObject albArt = (JObject)alb.Last.First;

                        results.Add(new Track
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
            }

            return results;
        }
        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            List<Album> albums;

            if (e.Parameter == null)
            {
                albums = MainPage.albums;
            }
            else
            {
                albums = (List<Album>)e.Parameter;
            }

            foreach(Album a in albums)
            {
                albumsViewer.Items.Add(new LibraryObject(a));
            }
        }

        #region Navigation
        private void AlbumsViewer_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (MainPage.window.CurrentSourcePageType != typeof(AlbumView))
            {
                MainPage.window.Navigate(typeof(AlbumView), e.ClickedItem);
            }
        }
        #endregion

        private void SearchButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            List<Album> results = new List<Album>();

            switch (queryType.SelectedIndex)
            {
                //Album
                case 0:
                    foreach (Album a in MainPage.albums)
                    {
                        if (a.name.ToLower().Contains(queryContent.Text.ToLower()))
                        {
                            results.Add(a);
                        }
                    }
                    MainPage.searchedLibrary = true;
                    MainPage.window.Navigate(typeof(LibraryView), results);
                    break;
                //Artist
                case 1:
                    foreach (Album a in MainPage.albums)
                    {
                        if (a.artist.name.ToLower().Contains(queryContent.Text.ToLower()))
                        {
                            results.Add(a);
                        }
                    }
                    MainPage.searchedLibrary = true;
                    MainPage.window.Navigate(typeof(LibraryView), results);
                    break;
                //Song
                case 2:
                    List<Track> tracks;
                    if (MainPage.TRACK_COUNT == MainPage.tracks.Count)
                    {
                        tracks = new List<Track>();
                        foreach (Track t in MainPage.tracks)
                        {
                            if (t.title.ToLower().Contains(queryContent.Text.ToLower()))
                            {
                                tracks.Add(t);
                            }
                        }
                    }
                    else
                    {
                        POSTTrack query = new POSTTrack();
                        query.query = queryContent.Text.ToLower();
                        tracks = BuildQueriedTrackList(query);
                    }

                    object[] details = new object[] { queryContent.Text.ToLower(), tracks };
                    MainPage.window.Navigate(typeof(ResultsView), details);
                    break;
                //Genre
                case 3:
                    foreach (Album a in MainPage.albums)
                    {
                        if (a.genreprimary.ToLower().Contains(queryContent.Text.ToLower()) || a.genresecondary.ToLower().Contains(queryContent.Text.ToLower()))
                        {
                            results.Add(a);
                        }
                    }
                    MainPage.searchedLibrary = true;
                    MainPage.window.Navigate(typeof(LibraryView), results);
                    break;
                default:
                    break;
            }
        }
    }
}
