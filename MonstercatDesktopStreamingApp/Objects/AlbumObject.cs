using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace MonstercatDesktopStreamingApp.Objects
{
    public class AlbumObject
    {
        public Album album { get; set; }
        public string albumName { get; set; }
        public string albumArtistName { get; set; }
        public string albumCoverURL { get; set; }
        public BitmapImage albumCoverImage { get; set; }

        public List<Track> albumSongs { get; set; }

        public AlbumObject(Album a, List<Track> songs)
        {
            albumCoverImage = null;
            album = a;
            albumName = a.name;
            albumArtistName = a.artist.name;
            albumCoverURL = a.coverURL;
            albumCoverImage = new BitmapImage(new Uri(albumCoverURL));
            albumSongs = songs;
        }
    }
}
