using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class BarChart
    {
        //Chart labels
        public List<string> lstLabels { get; set; }
        //Chart Values
        public List<int> lstValues_Heavenly_Males { get; set; }
        public List<int> lstValues_Heavenly_Females { get; set; }
        public List<int> lstValues_Hellish_Males { get; set; }
        public List<int> lstValues_Hellish_Females { get; set; }
        public List<int> lstValues_Purgatorial_Males { get; set; }
        public List<int> lstValues_Purgatorial_Females { get; set; }
        public List<int> lstValues_Other_Males { get; set; }
        public List<int> lstValues_Other_Females { get; set; }
        //Chart data as json
        public string jsonLabels { get; set; }
        public string jsonValues_Heavenly_Males { get; set; }
        public string jsonValues_Heavenly_Females { get; set; }
        public string jsonValues_Hellish_Males { get; set; }
        public string jsonValues_Hellish_Females { get; set; }
        public string jsonValues_Purgatorial_Males { get; set; }
        public string jsonValues_Purgatorial_Females { get; set; }
        public string jsonValues_Other_Males { get; set; }
        public string jsonValues_Other_Females { get; set; }

        public int Height { get; set; }


        public BarChart()
        {
            //Instantiate variables
            lstLabels = new List<string>();
            lstValues_Heavenly_Males = new List<int>();
            lstValues_Heavenly_Females = new List<int>();
            lstValues_Hellish_Males = new List<int>();
            lstValues_Hellish_Females = new List<int>();
            lstValues_Purgatorial_Males = new List<int>();
            lstValues_Purgatorial_Females = new List<int>();
            lstValues_Other_Males = new List<int>();
            lstValues_Other_Females = new List<int>();
        }
    }
}