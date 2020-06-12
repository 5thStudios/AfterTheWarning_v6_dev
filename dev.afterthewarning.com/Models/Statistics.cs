using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class Statistics
    {
        public StatsByAge statsByAge { get; set; }



        public Statistics(Boolean useTestData = false)
        {
            statsByAge = new StatsByAge(useTestData);
        }
    }
}