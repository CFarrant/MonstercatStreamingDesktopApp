using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonstercatDesktopStreamingApp.Objects
{
    [Serializable]
    public class Album
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string releaseCode { get; set; }
        public string genreprimary { get; set; }
        public string genresecondary { get; set; }
        public string coverURL { get; set; }
        public Artist artist { get; set; }
    }
}
