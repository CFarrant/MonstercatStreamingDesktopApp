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
using System.Collections.Generic;
using Windows.Media;
using Windows.Storage.Streams;
using Windows.Storage;

namespace MonstercatDesktopStreamingApp.Pages
{
    public sealed partial class SongView : Page
    {
        private MediaPlayer mediaPlayer = MainPage.mediaPlayerGUI.MediaPlayer;

        public SongView()
        {
            this.InitializeComponent();
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
                if (MainPage.queue.Count == 0)
                {
                    MainPage.mediaPlayerGUI.TransportControls.IsNextTrackButtonVisible = false;
                    MainPage.mediaPlayerGUI.TransportControls.IsPreviousTrackButtonVisible = false;
                }
                else
                {
                    MainPage.mediaPlayerGUI.TransportControls.IsNextTrackButtonVisible = true;
                }
            });
        }

        public async void EndGUIPlayback()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                MainPage.mediaPlayerGUI.Visibility = Visibility.Collapsed;
            });
        }

        private async void MediaPlayer_SourceChanged(MediaPlayer sender, object args)
        {
            if (mediaPlayer.Source != null)
            {
                StartGUIPlayback();

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (MainPage.queue.Count == 0)
                    {
                        MainPage.mediaPlayerGUI.TransportControls.IsNextTrackButtonVisible = false;
                        MainPage.mediaPlayerGUI.TransportControls.IsPreviousTrackButtonVisible = false;
                    }
                    else
                    {
                        MainPage.mediaPlayerGUI.TransportControls.IsNextTrackButtonVisible = true;
                    }

                    if (MainPage.history.Count > 0)
                    {
                        MainPage.mediaPlayerGUI.TransportControls.IsPreviousTrackButtonVisible = true;
                    }
                });

                mediaPlayer.Play();
            }
        }

        private async void UpdateSongView(TrackObject o)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                UpdateNowPlaying(o.trackArtistName, o.trackTitle);
                this.songArtist.Text = o.trackArtistName;
                this.songTitle.Text = o.trackTitle;
                this.songArtwork.Source = o.albumCoverImage;
                this.songAlbumArtistName.Text = o.album.artist.name;
                this.songAlbumName.Text = o.album.name;
                Uri webLink = new Uri(o.trackStreamURL);

                MediaPlaybackItem song = new MediaPlaybackItem(MediaSource.CreateFromUri(webLink));
                MediaItemDisplayProperties props = song.GetDisplayProperties();
                props.Type = MediaPlaybackType.Music;
                props.MusicProperties.Title = o.trackTitle;
                props.MusicProperties.Artist = o.trackArtistName;
                props.MusicProperties.Genres.Add(o.track.genreprimary);
                props.MusicProperties.Genres.Add(o.track.genresecondary);
                props.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri(o.albumCoverURL));
                song.ApplyDisplayProperties(props);
                mediaPlayer.Source = song;
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

            NagivatedToAsync(o);
        }

        private async void NagivatedToAsync(TrackObject o)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (MainPage.currentSong != o)
                {
                    MainPage.currentSong = o;
                    Uri webLink = new Uri(o.trackStreamURL);
                    MediaPlaybackItem song = new MediaPlaybackItem(MediaSource.CreateFromUri(webLink));
                    MediaItemDisplayProperties props = song.GetDisplayProperties();
                    props.Type = MediaPlaybackType.Music;
                    props.MusicProperties.Title = o.trackTitle;
                    props.MusicProperties.Artist = o.trackArtistName;
                    props.MusicProperties.Genres.Add(o.track.genreprimary);
                    props.MusicProperties.Genres.Add(o.track.genresecondary);
                    props.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri(o.albumCoverURL));
                    song.ApplyDisplayProperties(props);
                    mediaPlayer.Source = song;
                    MainPage.nowPlaying.Text = "Now Playing: \"" + o.trackArtistName + " ~ " + o.trackTitle + "\"";
                }
            });
        }


        private async void ReturnToHomePage()
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                MainPage.nowPlaying.Text = "Now Playing: ";
                MainPage.window.Navigate(typeof(LibraryView));
            });
        }

        private async void MediaPlayer_MediaEnded(MediaPlayer sender, object args)
        {
            if (!MainPage.mediaPlayer.IsLoopingEnabled)
            {
                if (MainPage.queue.Count > 0)
                {
                    MainPage.history.Push(MainPage.currentSong);
                    TrackObject o = MainPage.queue.Pop();
                    MainPage.currentSong = o;
                    UpdateSongView(o);
                }
                else if (MainPage.queue.Count == 0)
                {
                    EndGUIPlayback();
                    MainPage.currentSong = null;
                    mediaPlayer.Source = null;
                    MainPage.history = new Stack<TrackObject>();
                    ReturnToHomePage();
                }
            }
        }
    }
}
