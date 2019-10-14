using MonstercatDesktopStreamingApp.Objects;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;


namespace MonstercatDesktopStreamingApp.Pages
{
    public sealed partial class SongView : Page
    {
        private MediaPlayer mediaPlayer;

        public SongView()
        {
            this.InitializeComponent();
            mediaPlayer = MainPage.mediaPlayer;
            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            mediaPlayer.SourceChanged += MediaPlayer_SourceChanged;
        }

        private void UpdateNowPlaying(string artist, string title)
        {
            MainPage.nowPlaying.Text = "Now Playing: \"" + artist + " ~ " + title + "\"";
        }

        public async void StartGUIPlayback()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                MainPage.mediaPlayerGUI.Visibility = Visibility.Visible;
            });
        }

        public async void EndGUIPlayback()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                MainPage.mediaPlayerGUI.Visibility = Visibility.Collapsed;
            });
        }

        private void MediaPlayer_SourceChanged(MediaPlayer sender, object args)
        {
            StartGUIPlayback();
            if (mediaPlayer.PlaybackSession.Position.Milliseconds > 0)
            {
                mediaPlayer.Pause();
            }
            mediaPlayer.Play();
        }

        private async void UpdateSongView(TrackObject o)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                UpdateNowPlaying(o.trackArtistName, o.trackTitle);
                mediaPlayer.Source = (MediaSource.CreateFromUri(new Uri(o.trackStreamURL)));
                this.songArtist.Text = o.trackArtistName;
                this.songTitle.Text = o.trackTitle;
                this.songArtwork.Source = o.albumCoverImage;
                this.songAlbumArtistName.Text = o.album.artist.name;
                this.songAlbumName.Text = o.album.name;
            });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            TrackObject o = (TrackObject)e.Parameter;

            this.songArtist.Text = o.trackArtistName;
            this.songTitle.Text = o.trackTitle;
            this.songArtwork.Source = o.albumCoverImage;
            this.songAlbumArtistName.Text = o.album.artist.name;
            this.songAlbumName.Text = o.album.name;

            Uri webLink = new Uri(o.trackStreamURL);
            MainPage.mediaPlayer.Source = MediaSource.CreateFromUri(webLink);
            MainPage.nowPlaying.Text = "Now Playing: \""+o.trackArtistName+" ~ "+o.trackTitle+"\"";
        }


        private async void ReturnToHomePage()
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                MainPage.nowPlaying.Text = "Now Playing: ";
                MainPage.window.Navigate(typeof(LibraryView));
            });
        }

        private void MediaPlayer_MediaEnded(MediaPlayer sender, object args)
        {
            if (MainPage.queue.Count > 0)
            {
                TrackObject o = MainPage.queue.Pop();
                UpdateSongView(o);
            }
            else if (MainPage.queue.Count == 0)
            {
                EndGUIPlayback();
                ReturnToHomePage();
            }
        }
    }
}
