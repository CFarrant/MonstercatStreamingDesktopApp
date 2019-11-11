using MonstercatDesktopStreamingApp.Objects;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MonstercatDesktopStreamingApp.Pages
{
    public sealed partial class AlbumView : Page
    {
        #region Variables
        private Album a;
        private List<Track> aTracks;
        private Track[] tList;
        #endregion

        public AlbumView()
        {
            this.InitializeComponent();
            aTracks = new List<Track>();
        }

        #region API Calls
        private void BuildLocalTrackList(string albumId)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(@"http://www.monstercatstreaming.tk:8080");
                //httpClient.BaseAddress = new Uri(@"http://localhost:8080");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));
                string endpoint = @"/api/album/"+ albumId;
                //string endpoint = @"/album/" + albumId;
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

                        aTracks.Add(new Track
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
        }
        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var libraryObject = (LibraryObject)e.Parameter;
            Album album = libraryObject.album;
            a = album;
            BuildLocalTrackList(a.id);
            tList = new Track[aTracks.Count];
            int i = 0;
            foreach(Track t in aTracks)
            {
                try
                {
                    tList[t.tracknumber - 1] = t;
                    i++;
                }
                catch (Exception ex)
                {
                    tList[i] = t;
                    i++;
                }
            }

            this.albumName.Text = album.name;
            this.albumArtistName.Text = album.artist.name;
            this.albumCoverImage.Source = libraryObject.albumCoverImage;

            Button b = new Button();
            b.Margin = new Thickness(0, 15, 0, 0);
            b.Background = new SolidColorBrush(Windows.UI.Colors.DarkGray);
            b.RequestedTheme = ElementTheme.Dark;
            b.Content = "Play All Song(s)";
            b.Click += new RoutedEventHandler(SongViewer_PlayAllClick);
            songViewer.Children.Add(b);

            foreach (Track t in tList)
            {
                if (t != null) {
                    StackPanel wrap = new StackPanel();
                    wrap.Orientation = Orientation.Horizontal;
                    Button s = new Button();
                    s.Margin = new Thickness(0, 15, 0, 0);
                    s.Background = new SolidColorBrush(Windows.UI.Colors.DarkGray);
                    s.RequestedTheme = ElementTheme.Dark;
                    s.Content = t.tracknumber + " ~ " + t.title;
                    s.Click += new RoutedEventHandler(SongViewer_ItemClick);
                    s.Tag = t.tracknumber;
                    wrap.Children.Add(s);
                    if (MainPage.currentSong != null || (MainPage.history.Count + MainPage.queue.Count) >= 1)
                    {
                        Button c = new Button();
                        c.Margin = new Thickness(15, 15, 0, 0);
                        c.Background = new SolidColorBrush(Windows.UI.Colors.DarkGray);
                        c.RequestedTheme = ElementTheme.Dark;
                        c.Content = "+";
                        c.Tag = t.tracknumber;
                        c.Click += new RoutedEventHandler(SongViewer_AddClick);
                        wrap.Children.Add(c);
                    }
                    songViewer.Children.Add(wrap);
                }
            }
        }

        #region Button Overrides
        private void SongViewer_PlayAllClick(object sender, RoutedEventArgs e)
        {
            if (MainPage.currentSong != null)
            {
                MainPage.mediaPlayer.Pause();
                MainPage.mediaPlayer.Source = null;
                MainPage.history.Clear();
                MainPage.queue.Clear();
            }
            foreach (Track t in tList.Reverse())
            {
                if (t != null)
                {
                    TrackObject songObject = new TrackObject(a, t);
                    MainPage.queue.Push(songObject);
                }
            }
            MainPage.window.Navigate(typeof(SongView), MainPage.queue.Pop());
        }

        private void SongViewer_ItemClick(object sender, RoutedEventArgs e)
        {
            if (MainPage.window.CurrentSourcePageType != typeof(SongView))
            {
                if (MainPage.currentSong != null)
                {
                    MainPage.mediaPlayer.Pause();
                    MainPage.mediaPlayer.Source = null;
                    MainPage.history.Clear();
                    MainPage.queue.Clear();
                }
                Track t = null;
                var button = (Button)sender;
                foreach(Track s in tList)
                {
                    if (s != null)
                    {
                        if(s.tracknumber == (int)button.Tag)
                        {
                            t = s;
                        }
                    }
                }
                TrackObject songObject = new TrackObject(a, t);
                MainPage.window.Navigate(typeof(SongView), songObject);
            }
        }

        private void SongViewer_AddClick(object sender, RoutedEventArgs e)
        {
            if (MainPage.window.CurrentSourcePageType != typeof(SongView))
            {
                Track t = null;
                var button = (Button)sender;
                foreach (Track s in tList)
                {
                    if (s != null)
                    {
                        if (s.tracknumber == (int)button.Tag)
                        {
                            t = s;
                        }
                    }
                }
                TrackObject songObject = new TrackObject(a, t);

                Stack<TrackObject> temp = new Stack<TrackObject>();

                while (MainPage.queue.Count >= 1)
                {
                    TrackObject to = MainPage.queue.Pop();
                    temp.Push(to);
                }

                temp.Push(songObject);

                while (temp.Count >= 1)
                {
                    TrackObject to = temp.Pop();
                    MainPage.queue.Push(to);
                }
            }
        }
        #endregion
    }
}
