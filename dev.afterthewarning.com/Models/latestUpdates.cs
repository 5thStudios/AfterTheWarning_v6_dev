using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class latestUpdates
    {
        public DateTime datePublished { get; set; }
        public List<visionary> lstVisionaries = new List<visionary>();
    }



    public class visionary
    {
        public int id { get; set; } = -1;
        public string name { get; set; }
        public string url { get; set; }
        public List<message> lstMessages = new List<message>();
    }


    public class message
    {
        public int id { get; set; }
        public string title { get; set; }
        public string url { get; set; }
    }
}