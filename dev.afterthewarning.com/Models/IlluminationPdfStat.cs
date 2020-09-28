using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Models
{
    public class IlluminationPdfStat
    {
        public int Id { get; set; }
        public int Age { get; set; }


        public string Author { get; set; }
        public string Country { get; set; }
        public string ExperienceType { get; set; }
        public string Gender { get; set; }
        public string Religion { get; set; }
        public string Title { get; set; }
        public string Story { get; set; }


        public List<string> Races { get; set; }



        public IlluminationPdfStat()
        {
            Races = new List<string>();
        }
    }
}