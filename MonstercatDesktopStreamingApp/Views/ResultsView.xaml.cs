using MonstercatDesktopStreamingApp.Objects;
using MonstercatDesktopStreamingApp.Pages;
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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MonstercatDesktopStreamingApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ResultsView : Page
    {
        public ResultsView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

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

                    albumDisplay.Children.Add(albumCover);

                    StackPanel tracks = new StackPanel();
                    tracks.Margin = new Thickness(50, 75, 0, 50);

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
                                //s.Click += new RoutedEventHandler(SongViewer_ItemClick);
                                s.Tag = t.tracknumber;

                                buttons.Children.Add(s);

                                if (MainPage.currentSong != null || (MainPage.history.Count + MainPage.queue.Count) >= 1)
                                {
                                    Button c = new Button();
                                    c.Margin = new Thickness(15, 15, 0, 0);
                                    c.Background = new SolidColorBrush(Windows.UI.Colors.DarkGray);
                                    c.RequestedTheme = ElementTheme.Dark;
                                    c.Content = "+";
                                    c.Tag = t.tracknumber;
                                    //c.Click += new RoutedEventHandler(SongViewer_AddClick);
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
    }
}
