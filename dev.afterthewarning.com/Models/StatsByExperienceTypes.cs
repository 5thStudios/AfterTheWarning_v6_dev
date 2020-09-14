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
    public class StatsByExperienceTypes
    {
        public string Labels { get; set; }
        public string Values { get; set; }
        public string BgColors { get; set; }
        public string HoverBgColors { get; set; }
    }
}