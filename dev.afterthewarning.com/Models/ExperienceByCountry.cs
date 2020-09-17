using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class ExperienceByCountry
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("heavenly")]
        public int Heavenly { get; set; }

        [JsonProperty("hellish")]
        public int Hellish { get; set; }

        [JsonProperty("purgatorial")]
        public int Purgatorial { get; set; }

        [JsonProperty("other")]
        public int Other { get; set; }
    }
}