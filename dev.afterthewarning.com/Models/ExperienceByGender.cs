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
    }
}