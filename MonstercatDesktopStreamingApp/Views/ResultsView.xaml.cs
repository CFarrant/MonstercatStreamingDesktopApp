using MonstercatDesktopStreamingApp.Objects;
using MonstercatDesktopStreamingApp.Pages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace MonstercatDesktopStreamingApp.Views
{

    public sealed partial class ResultsView : Page
    {
        public ResultsView()
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

            queryType.SelectedIndex = MainPage.searchIndex;

            object[] songResults = (object[])e.Parameter;

            Dictionary<string, Album> albumsDisplayed = new Dictionary<string, Album>();

            if (songResults != null)
            {
                string query = (string)songResults[0];

                List<Track> objects = (List<Track>)songResults[1];

                foreach(Track t in objects){
                    if (!albumsDisplayed.ContainsKey(t.album.id))
                    {
                        albumsDisplayed.Add(t.album.id, t.album);
                    }
                }

                foreach (Album a in albumsDisplayed.Values.ToList())
                {
                    StackPanel albumDisplay = new StackPanel();
                    albumDisplay.Orientation = Orientation.Horizontal;

                    Image albumCover = new Image();
                    albumCover.Source = new BitmapImage(new Uri(a.coverURL));
                    albumCover.Width = 155;
                    albumCover.Height = 155;
                    albumCover.Margin = new Thickness(50, 50, 0, 0);
                    albumCover.VerticalAlignment = VerticalAlignment.Top;

                    albumDisplay.Children.Add(albumCover);

                    StackPanel tracks = new StackPanel();
                    tracks.Margin = new Thickness(50, 35, 0, 0);

                    foreach (Track t in objects)
                    {
                        if (t.album.id.Equals(a.id))
                        {
                            if (t.title.ToLower().Contains(query))
                            {
                                StackPanel buttons = new StackPanel();
                                buttons.Orientation = Orientation.Horizontal;

                                Button s = new Button();
                                s.Margin = new Thickness(0, 15, 0, 0);

                                s.Background = new SolidColorBrush(Windows.UI.Colors.DarkGray);
                                s.RequestedTheme = ElementTheme.Dark;
                                s.Content = t.tracknumber + " ~ " + t.title;
                                s.Click += new RoutedEventHandler(SongViewer_ItemClick);
                                s.Tag = t;

                                buttons.Children.Add(s);

                                if (MainPage.currentSong != null || (MainPage.history.Count + MainPage.queue.Count) >= 1)
                                {
                                    Button c = new Button();
                                    c.Margin = new Thickness(15, 15, 0, 0);
                                    c.Background = new SolidColorBrush(Windows.UI.Colors.DarkGray);
                                    c.RequestedTheme = ElementTheme.Dark;
                                    c.Content = "+";
                                    c.Tag = t;
                                    c.Click += new RoutedEventHandler(SongViewer_AddClick);
                                    buttons.Children.Add(c);
                                }
                                tracks.Children.Add(buttons);
                            }
                        }
                    }
                    albumDisplay.Children.Add(tracks);
                    resultsPanel.Children.Add(albumDisplay);
                }
            }
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
                var button = (Button)sender;

                Track t = (Track)button.Tag;

                TrackObject songObject = new TrackObject(t.album, t);
                MainPage.window.Navigate(typeof(SongView), songObject);
            }
        }

        private async void SongViewer_AddClick(object sender, RoutedEventArgs e)
        {
            if (MainPage.window.CurrentSourcePageType != typeof(SongView))
            {
                var button = (Button)sender;

                Track t = (Track)button.Tag;

                TrackObject songObject = new TrackObject(t.album, t);

                Stack<TrackObject> temp = new Stack<TrackObject>();

                while (MainPage.queue.Count >= 1)
                {
                    if (MainPage.queue.Count == 1)
                    {
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            MainPage.mediaPlayerGUI.TransportControls.IsNextTrackButtonVisible = false;
                            MainPage.mediaPlayerGUI.TransportControls.IsPreviousTrackButtonVisible = false;
                        });
                    }
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

        private void QueryType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainPage.searchIndex = queryType.SelectedIndex;
        }
    }
}
