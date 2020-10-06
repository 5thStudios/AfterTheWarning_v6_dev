using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Models
{
    public class TitlePanelContent
    {
        public String heightClass { get; set; }
        public String docType { get; set; }
        public String topBanner { get; set; }
        public String Name { get; set; }
        public String ParentName { get; set; }
        public String visionaryName { get; set; }
        public String strH1 { get; set; }
        public String strH3 { get; set; }
        public String title { get; set; }
        public String subtitle { get; set; }
        public StringBuilder sbDateRange { get; set; }
        public StringBuilder sbCite { get; set; }
 




        public TitlePanelContent()
        {
            sbDateRange = new StringBuilder();
            sbCite = new StringBuilder();
        }
    }
}