using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class LatestUpdateList
    {
        public List<latestUpdates> LstLatestUpdates { get; set; }
        public Pagination Pagination { get; set; } = new Pagination();



        public LatestUpdateList()
        {
            LstLatestUpdates = new List<latestUpdates>();
        }
    }
}