using MonstercatDesktopStreamingApp.Objects;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace MonstercatDesktopStreamingApp.Pages
{
    public sealed partial class AlbumView : Page
    {
        private Album a;
        private List<Track> aTracks;
        private Track[] tList;

        public AlbumView()
        {
            this.InitializeComponent();
            aTracks = new List<Track>();
        }

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
                        JProperty songArt = (JProperty)item.First.Next.Next.Next.Next.Next.Next;
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var libraryObject = (LibraryObject)e.Parameter;
            Album album = libraryObject.album;
            //List<Track> songs = new List<Track>();
            //foreach(Track t in MainPage.tracks)
            //{
            //    if (t.album.id.Equals(album.id))
            //    {
            //        songs.Add(t);
            //    }
            //}
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
                    Button s = new Button();
                    s.Margin = new Thickness(0, 15, 0, 0);
                    s.Background = new SolidColorBrush(Windows.UI.Colors.DarkGray);
                    s.RequestedTheme = ElementTheme.Dark;
                    s.Content = t.tracknumber + " ~ " + t.title;
                    s.Click += new RoutedEventHandler(SongViewer_ItemClick);
                    songViewer.Children.Add(s);
                }
            }
        }

        private void SongViewer_PlayAllClick(object sender, RoutedEventArgs e)
        {
            foreach(Track t in tList.Reverse())
            {
                TrackObject songObject = new TrackObject(a, t);
                MainPage.queue.Push(songObject);
            }
            MainPage.window.Navigate(typeof(SongView), MainPage.queue.Pop());
        }

        private void SongViewer_ItemClick(object sender, RoutedEventArgs e)
        {
            if (MainPage.window.CurrentSourcePageType != typeof(SongView))
            {
                Track t = null;
                var button = (Button)sender;
                string track = (string)button.Content;
                track = track.Split(' ')[0];
                foreach(Track s in tList)
                {
                    if (s != null)
                    {
                        if(s.tracknumber == int.Parse(track))
                        {
                            t = s;
                        }
                    }
                }
                TrackObject songObject = new TrackObject(a, t);
                MainPage.window.Navigate(typeof(SongView), songObject);
            }
        }
    }
}
