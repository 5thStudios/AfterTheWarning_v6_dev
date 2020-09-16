using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;


namespace Models
{
    public class LineChart
    {
        [JsonProperty("ageRange")]
        public string AgeRange { get; set; }


        [JsonProperty("count")]
        public uint Count { get; set; }
    }
}