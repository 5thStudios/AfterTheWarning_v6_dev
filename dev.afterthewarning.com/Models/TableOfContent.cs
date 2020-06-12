using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class TableOfContent
    {
        public List<tocTestament> lstTestaments = new List<tocTestament>();
    }


    public class tocTestament
    {
        public string testament { get; set; }
        public List<tocBookSet> lstBookSets = new List<tocBookSet>();
    }


    public class tocBookSet
    {
        public string bookSet { get; set; }
        public List<tocBook> lstBooks = new List<tocBook>();
    }


    public class tocBook
    {
        public string name { get; set; }
        public string url { get; set; }
    }
}