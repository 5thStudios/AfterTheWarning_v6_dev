using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class ExperienceByGender
    {
        [JsonProperty("ageRange")]
        public string AgeRange { get; set; }

        [JsonProperty("males")]
        public int Males { get; set; }

        [JsonProperty("females")]
        public int Females { get; set; }


        public ExperienceByGender() { }
        public ExperienceByGender(string _ageRange)
        {
            AgeRange = _ageRange;
            Males = 0;
            Females = 0;
        }
    }
}