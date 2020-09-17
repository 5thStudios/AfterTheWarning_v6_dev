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
    public class StackedBarChart
    {
        public string Labels { get; set; }
        public string Values_Heavenly { get; set; }
        public string Values_Hellish { get; set; }
        public string Values_Purgatorial { get; set; }
        public string Values_Unknown { get; set; }
        public int Height { get; set; }

        //public string jsonData { get; set; }
    }
}