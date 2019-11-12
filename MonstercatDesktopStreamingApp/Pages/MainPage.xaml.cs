using MonstercatDesktopStreamingApp.Objects;
using MonstercatDesktopStreamingApp.Views;
using System;
using System.Collections.Generic;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MonstercatDesktopStreamingApp.Pages
{
    public sealed partial class MainPage : Page
    {
        #region Veriables
        public static Stack<TrackObject> queue;
        public static string authentication = "";
        public static TextBlock nowPlaying;
        public static Frame window;
        public static MediaPlayer mediaPlayer;
        public static List<Album> albums;
        public static List<Track> tracks;
        public static MediaPlayerElement mediaPlayerGUI;
        public static Stack<TrackObject> history;
        public static TrackObject currentSong;
        public static bool searchedLibrary = false;
        public static int TRACK_COUNT;
        public static int searchIndex = 0;
        #endregion

        public MainPage()
        {
            this.InitializeComponent();
            currentSong = null;
            window = windowView;
            queue = new Stack<TrackObject>();
            albums = new List<Album>();
            tracks = new List<Track>();
            history = new Stack<TrackObject>();
            nowPlaying = this.nowPlayingTitle;
            mediaPlayerGUI = this.mediaPlayerUI;
            mediaPlayerUI.Visibility = Visibility.Collapsed;
            mediaPlayer = mediaPlayerUI.MediaPlayer;
            windowView.Navigate(typeof(LoginPage));
            mediaPlayer.CommandManager.NextReceived += CommandManager_NextReceived;
            mediaPlayer.CommandManager.NextBehavior.EnablingRule = MediaCommandEnablingRule.Always;
            mediaPlayer.CommandManager.PreviousReceived += CommandManager_PreviousReceived;
            mediaPlayer.CommandManager.PreviousBehavior.EnablingRule = MediaCommandEnablingRule.Always;
            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            mediaPlayer.SourceChanged += MediaPlayer_SourceChanged;
        }

        #region Media Player Overrides
        private async void MediaPlayer_MediaEnded(MediaPlayer sender, object args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (!mediaPlayer.IsLoopingEnabled)
                {
                    if (queue.Count > 0)
                    {
                        history.Push(currentSong);
                        TrackObject o = queue.Pop();
                        currentSong = o;
                        if (window.CurrentSourcePageType.Equals(typeof(QueueView)))
                        {
                            window.Navigate(typeof(QueueView));
                        }
                        else
                        {
                            window.Navigate(typeof(SongView));
                        }
                    }
                    else if (queue.Count == 0)
                    {
                        EndGUIPlayback();
                        currentSong = null;
                        mediaPlayer.Source = null;
                        history = new Stack<TrackObject>();
                        ReturnToHomePage();
                    }
                }
            });
        }

        private async void MediaPlayer_SourceChanged(MediaPlayer sender, object args)
        {
            if (mediaPlayer.Source != null)
            {
                StartGUIPlayback();

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (queue.Count == 0)
                    {
                        mediaPlayerGUI.TransportControls.IsNextTrackButtonVisible = false;
                        mediaPlayerGUI.TransportControls.IsPreviousTrackButtonVisible = false;
                    }
                    else
                    {
                        mediaPlayerGUI.TransportControls.IsNextTrackButtonVisible = true;
                    }

                    if (history.Count > 0)
                    {
                        mediaPlayerGUI.TransportControls.IsPreviousTrackButtonVisible = true;
                    }
                });

                mediaPlayer.Play();
            }
        }

        private async void ReturnToHomePage()
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                nowPlaying.Text = "Now Playing: ";
                window.Navigate(typeof(LibraryView));
            });
        }

        public async void StartGUIPlayback()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                mediaPlayerGUI.Visibility = Visibility.Visible;
                if (queue.Count == 0)
                {
                    mediaPlayerGUI.TransportControls.IsNextTrackButtonVisible = false;
                    mediaPlayerGUI.TransportControls.IsPreviousTrackButtonVisible = false;
                }
                else
                {
                    mediaPlayerGUI.TransportControls.IsNextTrackButtonVisible = true;
                    mediaPlayerGUI.TransportControls.IsPreviousTrackButtonVisible = false;
                }
            });
        }

        public async void EndGUIPlayback()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                mediaPlayerGUI.Visibility = Visibility.Collapsed;
            });
        }

        private void CommandManager_PreviousReceived(MediaPlaybackCommandManager sender, MediaPlaybackCommandManagerPreviousReceivedEventArgs args)
        {
            if (history.Count >= 1)
            {
                queue.Push(currentSong);
                currentSong = history.Pop();
                if (window.CurrentSourcePageType.Equals(typeof(QueueView)))
                {
                    window.Navigate(typeof(QueueView));
                }
                else
                {
                    window.Navigate(typeof(SongView));
                }
            }
        }

        private void CommandManager_NextReceived(MediaPlaybackCommandManager sender, MediaPlaybackCommandManagerNextReceivedEventArgs args)
        {
            if (queue.Count >= 1)
            {
                history.Push(currentSong);
                currentSong = queue.Pop();
                if (window.CurrentSourcePageType.Equals(typeof(QueueView)))
                {
                    window.Navigate(typeof(QueueView));
                }
                else
                {
                    window.Navigate(typeof(SongView));
                }
            }
        }
        #endregion

        #region Navigation
        private void Library_Clicked(object sender, RoutedEventArgs e)
        {
            if (searchedLibrary == false && windowView.CurrentSourcePageType != typeof(LibraryView) && windowView.CurrentSourcePageType != typeof(LoginPage)
                && windowView.CurrentSourcePageType != typeof(ForgotPage) && windowView.CurrentSourcePageType != typeof(RegisterPage))
            {
                windowView.Navigate(typeof(LibraryView), albums);
            }
            else if (searchedLibrary == true)
            {
                searchedLibrary = false;
                windowView.Navigate(typeof(LibraryView));
            }
            else if (authentication.Equals(""))
            {
                DisplayNotLoggedInDialog();
            }
        }

        private void Queue_Clicked(object sender, RoutedEventArgs e)
        {
            if (windowView.CurrentSourcePageType != typeof(QueueView) && windowView.CurrentSourcePageType != typeof(LoginPage)
                && windowView.CurrentSourcePageType != typeof(ForgotPage) && windowView.CurrentSourcePageType != typeof(RegisterPage))
            {
                windowView.Navigate(typeof(QueueView));
            }
            else if (authentication.Equals(""))
            {
                DisplayNotLoggedInDialog();
            }
        }

        private void NowPlaying_Clicked(object sender, RoutedEventArgs e)
        {
            if (windowView.CurrentSourcePageType != typeof(SongView) && windowView.CurrentSourcePageType != typeof(LoginPage)
                && windowView.CurrentSourcePageType != typeof(ForgotPage) && windowView.CurrentSourcePageType != typeof(RegisterPage))
            {
                if (currentSong != null)
                {
                    windowView.Navigate(typeof(SongView), currentSong);
                }
            }
            else if (authentication.Equals(""))
            {
                DisplayNotLoggedInDialog();
            }
        } 

        private void Change_Clicked(object sender, RoutedEventArgs e)
        {
            if (windowView.CurrentSourcePageType != typeof(ChangePage) && windowView.CurrentSourcePageType != typeof(LoginPage)
               && windowView.CurrentSourcePageType != typeof(ForgotPage) && windowView.CurrentSourcePageType != typeof(RegisterPage))
            {
                windowView.Navigate(typeof(ChangePage));
            }
            else if (authentication.Equals(""))
            {
                DisplayNotLoggedInDialog();
            }
        }
        #endregion

        #region Tools
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return "Basic " + System.Convert.ToBase64String(plainTextBytes);
        }
        #endregion

        #region Dialog Windows
        private async void DisplayNotLoggedInDialog()
        {
            ContentDialog notLoggedIn = new ContentDialog
            {
                RequestedTheme = ElementTheme.Dark,
                Title = "User Login is Required",
                Content = "Please log in and try again!",
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await notLoggedIn.ShowAsync();
        }
        #endregion
    }
}
