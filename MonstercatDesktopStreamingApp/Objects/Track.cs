using System;

namespace MonstercatDesktopStreamingApp.Objects
{
    [Serializable]
    public class Track
    {
        public string id { get; set; }
        public int tracknumber { get; set; }
        public string title { get; set; }
        public string genreprimary { get; set; }
        public string genresecondary { get; set; }
        public string songURL { get; set; }
        public Artist artist { get; set; }
        public Album album { get; set; }

        public override string ToString()
        {
            return album.releaseCode + " ~ " + tracknumber + ") " + title;
        }
    }
}
