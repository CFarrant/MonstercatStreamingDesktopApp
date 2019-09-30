using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonstercatDesktopStreamingApp.Objects
{
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
    }
}
