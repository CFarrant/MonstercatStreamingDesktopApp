using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace MonstercatDesktopStreamingApp.Objects
{
    public class TrackObject
    {
        public Album album { get; set; }
        public string albumName { get; set; }
        public string albumCoverURL { get; set; }
        public BitmapImage albumCoverImage { get; set; }
        public Track track { get; set; }
        public string trackTitle { get; set; }
        public string trackArtistName { get; set; }
        public string trackStreamURL { get; set; }

        public TrackObject(Album a, Track t)
        {
            albumCoverImage = null;
            album = a;
            albumName = a.name;
            albumCoverURL = a.coverURL;
            albumCoverImage = new BitmapImage(new Uri(albumCoverURL));
            track = t;
            trackTitle = t.title;
            trackArtistName = t.artist.name;
            trackStreamURL = t.songURL;
        }
    }
}
