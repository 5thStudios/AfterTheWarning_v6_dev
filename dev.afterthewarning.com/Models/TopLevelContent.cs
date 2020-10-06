using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class TopLevelContent
    {
        public bool ShowAnalytics { get; set; }
        public SEOChecker.MVC.MetaData Meta { get; set; }



        public TopLevelContent()
        {
            ShowAnalytics = true;
        }
    }
}