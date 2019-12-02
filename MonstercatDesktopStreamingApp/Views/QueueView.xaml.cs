using MonstercatDesktopStreamingApp.Objects;
using MonstercatDesktopStreamingApp.Pages;
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
using Windows.UI.Xaml.Navigation;

namespace MonstercatDesktopStreamingApp.Views
{
    public sealed partial class QueueView : Page
    {
        public QueueView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            TrackObject[] front;
            if (MainPage.queue.Count >= 1)
            {
                front = new TrackObject[MainPage.queue.Count];
                MainPage.queue.CopyTo(front, 0);
            }
            else
            {
                front = new TrackObject[0];
            }

            TrackObject[] back;
            if (MainPage.history.Count >= 1)
            {
                back = new TrackObject[MainPage.history.Count];
                MainPage.history.CopyTo(back, 0);
            }
            else
            {
                back = new TrackObject[0];
            }


            Stack<TrackObject> songQueue = new Stack<TrackObject>();

            foreach(TrackObject t in front)
            {
                songQueue.Push(t);
            }

            songQueue.Push(MainPage.currentSong);

            foreach(TrackObject t in back)
            {
                songQueue.Push(t);
            }

            while (songQueue.Count >= 1)
            {
                TrackObject to = songQueue.Pop();

                if (to != null)
                {
                    ListBoxItem b = new ListBoxItem();
                    b.Margin = new Thickness(0, 15, 0, 0);
                    b.RequestedTheme = ElementTheme.Dark;
                    b.Content = to.album.name + " ~ " + to.track.title;
                    if (to == MainPage.currentSong)
                    {
                        b.Background = new SolidColorBrush(Windows.UI.Colors.Blue);
                    }
                    else
                    {
                        b.Background = new SolidColorBrush(Windows.UI.Colors.DarkGray);
                    }

                    queueList.Items.Add(b);
                }
            }
        }
    }
}
