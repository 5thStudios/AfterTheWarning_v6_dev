using formulate.core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;

namespace Models
{
    public class MainNavContent
    {
        public string searchUrl { get; set; }
        public IPublishedContent ipHome { get; set; }
        public Boolean firstItem { get; set; }
        public Boolean isLoggedIn { get; set; }
        public Boolean activateIlluminationControls { get; set; }
        public List<navigationLink> lstMainNavlinks { get; set; }
        public List<navigationLink> lstMinorNavlinks { get; set; }




        public MainNavContent()
        {
            firstItem = true;
            lstMainNavlinks = new List<navigationLink>();
            lstMinorNavlinks = new List<navigationLink>();
        }
    }
}