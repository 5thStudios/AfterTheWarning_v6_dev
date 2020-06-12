using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class ChartDataset
    {
        public string Id { get; set; } //No spaces!!! Each Id MUST be unique.
        public string Label { get; set; }
        public string Color { get; set; }
        public List<int> LstData { get; set; }
        public string JsonData { get; set; }


        public ChartDataset(string _id, string _label, string _color)
        {
            //Instantiate variables
            LstData = new List<int>();

            //Set values
            Id = _id;
            Label = _label;
            Color = _color;
        }
    }
}