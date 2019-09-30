using MonstercatDesktopStreamingApp.Objects;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


namespace MonstercatDesktopStreamingApp.Pages
{
    public sealed partial class SongView : Page
    {
        public SongView()
        {
            this.InitializeComponent();
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
            MainPage.nowPlaying.Text = "Now Playing: \""+o.trackArtistName+" ~ "+o.trackTitle+"\"";
            MainPage.audioStreaming.Navigate(webLink);
        }
    }
}
