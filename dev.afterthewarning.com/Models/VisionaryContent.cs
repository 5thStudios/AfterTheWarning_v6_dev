using formulate.core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Umbraco.Core.Models;

namespace Models
{
    public class VisionaryContent
    {
        public string VisionarysName { get; set; }
        public string PageImage { get; set; }
        public string Religion { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string phoneNo { get; set; }
        public string OriginalSiteUrl { get; set; }
        public string OriginalSiteName { get; set; }
        public List<addressRecord> lstAddressRecord { get; set; }
        public StringBuilder strAddress { get; set; }
        public Boolean isOtherOrKeepPrivate { get; set; }
        public Boolean isAddressNull { get; set; }



        public VisionaryContent()
        {
            lstAddressRecord = new List<addressRecord>();
            strAddress = new StringBuilder();
        }
    }
}