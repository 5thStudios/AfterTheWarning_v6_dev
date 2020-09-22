using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class IlluminationStats
    {
        public int experienceType_Heavenly { get; set; }
        public int experienceType_Hellish { get; set; }
        public int experienceType_Purgatorial { get; set; }
        public int experienceType_Other { get; set; }

        public int age { get; set; }
        public string experienceType { get; set; }
        //public string gender { get; set; }
        public List<string> lstRace { get; set; }
        //public int raceId { get; set; }
        //public string religion { get; set; }


        public bool hasCountry { get; set; }
        public bool hasAge { get; set; }
        public bool hasGender { get; set; }
        public bool hasReligion { get; set; }
        public bool hasRace { get; set; }


        public List<Models.ExperienceByCountry> LstExperiences_byCountry { get; set; }

        public List<Models.LineChart> lstAge_Heavenly { get; set; }
        public List<Models.LineChart> lstAge_Hellish { get; set; }
        public List<Models.LineChart> lstAge_Purgatorial { get; set; }
        public List<Models.LineChart> lstAge_Unknown { get; set; }

        public List<Models.ExperienceByGender> lstGender_Heavenly { get; set; }
        public List<Models.ExperienceByGender> lstGender_Hellish { get; set; }
        public List<Models.ExperienceByGender> lstGender_Purgatorial { get; set; }
        public List<Models.ExperienceByGender> lstGender_Unknown { get; set; }

        public List<Models.ExperienceByReligion> lstReligions { get; set; }
         
        public List<Models.ExperienceByRace> lstRaces { get; set; }




        public IlluminationStats()
        {
            experienceType_Heavenly = 0;
            experienceType_Hellish = 0;
            experienceType_Purgatorial = 0;
            experienceType_Other = 0;

            LstExperiences_byCountry = new List<ExperienceByCountry>();
            lstRace = new List<string>();

            lstAge_Heavenly = DefaultData_Linechart();
            lstAge_Hellish = DefaultData_Linechart();
            lstAge_Purgatorial = DefaultData_Linechart();
            lstAge_Unknown = DefaultData_Linechart();

            lstGender_Heavenly = DefaultData_ExperienceByGender();
            lstGender_Hellish = DefaultData_ExperienceByGender();
            lstGender_Purgatorial = DefaultData_ExperienceByGender();
            lstGender_Unknown = DefaultData_ExperienceByGender();

            lstReligions = DefaultData_ExperienceByReligion();

            lstRaces = DefaultData_ExperienceByRace();
        }

        private List<LineChart> DefaultData_Linechart()
        {
            List<LineChart> lst = new List<LineChart>();
            lst.Add(new LineChart("0"));
            lst.Add(new LineChart("5"));
            lst.Add(new LineChart("10"));
            lst.Add(new LineChart("15"));
            lst.Add(new LineChart("20"));
            lst.Add(new LineChart("25"));
            lst.Add(new LineChart("30"));
            lst.Add(new LineChart("35"));
            lst.Add(new LineChart("40"));
            lst.Add(new LineChart("45"));
            lst.Add(new LineChart("50"));
            lst.Add(new LineChart("55"));
            lst.Add(new LineChart("60"));
            lst.Add(new LineChart("65"));
            lst.Add(new LineChart("70"));
            lst.Add(new LineChart("75"));
            lst.Add(new LineChart("80"));
            lst.Add(new LineChart("85"));
            lst.Add(new LineChart("90"));
            lst.Add(new LineChart("95"));
            lst.Add(new LineChart("100+"));
            return lst;
        }
        private List<ExperienceByGender> DefaultData_ExperienceByGender()
        {
            List<ExperienceByGender> lst = new List<ExperienceByGender>();
            lst.Add(new ExperienceByGender("0-5"));
            lst.Add(new ExperienceByGender("6-10"));
            lst.Add(new ExperienceByGender("11-15"));
            lst.Add(new ExperienceByGender("16-20"));
            lst.Add(new ExperienceByGender("21-25"));
            lst.Add(new ExperienceByGender("26-30"));
            lst.Add(new ExperienceByGender("31-35"));
            lst.Add(new ExperienceByGender("36-40"));
            lst.Add(new ExperienceByGender("41-45"));
            lst.Add(new ExperienceByGender("46-50"));
            lst.Add(new ExperienceByGender("51-55"));
            lst.Add(new ExperienceByGender("56-60"));
            lst.Add(new ExperienceByGender("61-65"));
            lst.Add(new ExperienceByGender("66-70"));
            lst.Add(new ExperienceByGender("71-75"));
            lst.Add(new ExperienceByGender("76-80"));
            lst.Add(new ExperienceByGender("81-85"));
            lst.Add(new ExperienceByGender("86-90"));
            lst.Add(new ExperienceByGender("91-95"));
            lst.Add(new ExperienceByGender("96-100+"));
            return lst;
        }
        private List<ExperienceByReligion> DefaultData_ExperienceByReligion()
        {
            List<ExperienceByReligion> lst = new List<ExperienceByReligion>();
            lst.Add(new ExperienceByReligion("Agnostic"));
            lst.Add(new ExperienceByReligion("Atheist"));
            lst.Add(new ExperienceByReligion("Baptist"));
            lst.Add(new ExperienceByReligion("Buddhist"));
            lst.Add(new ExperienceByReligion("Catholic"));
            lst.Add(new ExperienceByReligion("Evangelical"));
            lst.Add(new ExperienceByReligion("Lutheran"));
            lst.Add(new ExperienceByReligion("Hindu"));
            lst.Add(new ExperienceByReligion("Muslim"));
            lst.Add(new ExperienceByReligion("Other Christian"));
            lst.Add(new ExperienceByReligion("Protestant"));
            lst.Add(new ExperienceByReligion("Satinism"));
            lst.Add(new ExperienceByReligion("Wiccan or New Age"));
            lst.Add(new ExperienceByReligion("Other or Keep Private"));
            return lst;
        }
        private List<ExperienceByRace> DefaultData_ExperienceByRace()
        {
            List<ExperienceByRace> lst = new List<ExperienceByRace>();
            lst.Add(new ExperienceByRace("Arabic"));
            lst.Add(new ExperienceByRace("Asian"));
            lst.Add(new ExperienceByRace("Black or African"));
            lst.Add(new ExperienceByRace("Indian"));
            lst.Add(new ExperienceByRace("Jewish"));
            lst.Add(new ExperienceByRace("Latin or Hispanic"));
            lst.Add(new ExperienceByRace("Native American"));
            lst.Add(new ExperienceByRace("Pacific Islander"));
            lst.Add(new ExperienceByRace("White or Caucasian"));
            lst.Add(new ExperienceByRace("Other or Keep Private"));
            return lst;
        }
    }
}