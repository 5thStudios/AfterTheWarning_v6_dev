using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.PublishedContentModels;
using ContentModels = Umbraco.Web.PublishedContentModels;


namespace Models
{
    public class PieChart
    {
        public string Labels { get; set; }
        public string Values { get; set; }
        public string BgColors { get; set; }
        public string HoverBgColors { get; set; }
        public int TotalEntries { get; set; }

        public List<string> lstLabels { get; set; }
        public List<int> lstValues { get; set; }



        public PieChart()
        {
            lstLabels = new List<string>();
            lstValues = new List<int>();
            TotalEntries = 0;
        }
    }
}