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
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            TrackObject o = (TrackObject)e.Parameter;

            if (o == null)
            {
                o = MainPage.currentSong;
                this.songArtist.Text = o.trackArtistName;
                this.songTitle.Text = o.trackTitle;
                this.songArtwork.Source = o.albumCoverImage;
                this.songAlbumArtistName.Text = o.album.artist.name;
                this.songAlbumName.Text = o.album.name;

                NagivatedToAsyncChangedSong(o);
            }
            else
            {
                this.songArtist.Text = o.trackArtistName;
                this.songTitle.Text = o.trackTitle;
                this.songArtwork.Source = o.albumCoverImage;
                this.songAlbumArtistName.Text = o.album.artist.name;
                this.songAlbumName.Text = o.album.name;

                NagivatedToAsync(o);
            }
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

        private async void NagivatedToAsyncChangedSong(TrackObject o)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
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
            });
        }
    }
}
