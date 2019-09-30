using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace MonstercatDesktopStreamingApp.Objects
{
    class LibraryObject
    {
        public Album album {get; set;}
        public string albumName { get; set; }
        public string albumArtistName { get; set; }
        public string albumCoverURL { get; set; }
        public BitmapImage albumCoverImage { get; set; }

        public LibraryObject(Album a)
        {
            albumCoverImage = null;
            album = a;
            albumName = a.name;
            albumArtistName = a.artist.name;
            albumCoverURL = a.coverURL;
            albumCoverImage = new BitmapImage(new Uri(albumCoverURL));
        }
    }
}
