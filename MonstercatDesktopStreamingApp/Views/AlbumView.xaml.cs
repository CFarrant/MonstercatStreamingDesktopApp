using MonstercatDesktopStreamingApp.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        private Track[] tList;

        public AlbumView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var libraryObject = (LibraryObject)e.Parameter;
            Album album = libraryObject.album;
            List<Track> songs = new List<Track>();
            foreach(Track t in MainPage.tracks)
            {
                if (t.album.id.Equals(album.id))
                {
                    songs.Add(t);
                }
            }
            a = album;
            tList = new Track[songs.Count];
            int i = 0;
            foreach(Track t in songs)
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

            foreach(Track t in tList)
            {
                if (t != null) {
                    Button b = new Button();
                    b.Margin = new Thickness(0, 15, 0, 0);
                    b.Background = new SolidColorBrush(Windows.UI.Colors.DarkGray);
                    b.RequestedTheme = ElementTheme.Dark;
                    b.Content = t.tracknumber + " ~ " + t.title;
                    b.Click += new RoutedEventHandler(SongViewer_ItemClick);
                    songViewer.Children.Add(b);
                }
            }
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
