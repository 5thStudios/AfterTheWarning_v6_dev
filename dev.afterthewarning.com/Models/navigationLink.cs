using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class navigationLink
    {
        public string name { get; set; } = string.Empty;
        public string url { get; set; } = string.Empty;
        public Int32? id { get; set; } = null;
        public Int16? level { get; set; } = null;
        public List<navigationLink> lstChildLinks = new List<navigationLink>();
    }
}