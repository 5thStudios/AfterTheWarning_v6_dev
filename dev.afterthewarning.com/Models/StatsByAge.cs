using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.PublishedContentModels;
using ContentModels = Umbraco.Web.PublishedContentModels;


namespace Models
{
    public class StatsByAge
    {
        public List<string> LstAgeRange { get; set; }
        public string jsonAgeRange { get; set; }
        public List<ChartDataset> LstChartData { get; set; }
        public uint TotalEntries { get; set; }

        public List<Stat_AgeRange> LstAgeStats_Heavenly { get; set; }
        public List<Stat_AgeRange> LstAgeStats_Hellish { get; set; }
        public List<Stat_AgeRange> LstAgeStats_Purgatorial { get; set; }
        public List<Stat_AgeRange> LstAgeStats_Unknown { get; set; }



        //public string jsonInfo { get; set; }
        //public string Duration { get; set; }


        //public StatsByAge(IPublishedContent ipByAge, Boolean useTestData = false)
        //{
        //    //Initialize variables.
        //    Stopwatch stopwatch = Stopwatch.StartNew();
        //    Random random = new Random();
        //    LstChartData = new List<ChartDataset>();
        //    LstAgeRange = new List<string>();
        //    ChartDataset HeavenlyDataset = new ChartDataset("Heavenly", "Heavenly", "#4f81bc");
        //    ChartDataset HellishDataset = new ChartDataset("Hellish", "Hellish", "#bf4b49");
        //    ChartDataset PurgatorialDataset = new ChartDataset("Purgatorial", "Purgatorial", "#9bbb57");
        //    ChartDataset UnknownDataset = new ChartDataset("Unknown", "Unknown/Unsure", "#21bea6");
        //    TotalEntries = 0;

        //    //Set values
        //    LstAgeRange.Add("0");
        //    LstAgeRange.Add("5");
        //    LstAgeRange.Add("10");
        //    LstAgeRange.Add("15");
        //    LstAgeRange.Add("20");
        //    LstAgeRange.Add("25");
        //    LstAgeRange.Add("30");
        //    LstAgeRange.Add("35");
        //    LstAgeRange.Add("40");
        //    LstAgeRange.Add("45");
        //    LstAgeRange.Add("50");
        //    LstAgeRange.Add("55");
        //    LstAgeRange.Add("60");
        //    LstAgeRange.Add("65");
        //    LstAgeRange.Add("70");
        //    LstAgeRange.Add("75");
        //    LstAgeRange.Add("80");
        //    LstAgeRange.Add("85");
        //    LstAgeRange.Add("90");
        //    LstAgeRange.Add("95");
        //    LstAgeRange.Add("100+");

        //    //
        //    if (useTestData)
        //    {
        //        //Create random lists
        //        for (int i = 0; i < 21; i++)
        //        {
        //            //PurgatorialDataset.LstData.Add(random.Next(50, 101));
        //            //HellishDataset.LstData.Add(random.Next(30, 71));
        //            //HeavenlyDataset.LstData.Add(random.Next(0, 51));
        //            //UnknownDataset.LstData.Add(random.Next(0, 11));
        //            PurgatorialDataset.LstData.Add(0);
        //            HellishDataset.LstData.Add(15);
        //            HeavenlyDataset.LstData.Add(30);
        //            UnknownDataset.LstData.Add(45);
        //        }


        //        //   

        //        //jsonInfo = ipByAge.Name + " | " + ipByAge.DocumentTypeAlias;
        //        jsonInfo = ipByAge.GetPropertyValue<string>(Common.NodeProperties.statsHeavenly);
        //        //JsonConvert.DeserializeObject<List<Models._prayerRequest>>(member.GetValue(NodeProperties.prayerRequests).ToString());


        //    }
        //    else
        //    {
        //        //Initialize variables
        //        List<int> LstAge_Heavenly = Enumerable.Repeat(0, 21).ToList();
        //        List<int> LstAge_Hellish = Enumerable.Repeat(0, 21).ToList();
        //        List<int> LstAge_Purgatorial = Enumerable.Repeat(0, 21).ToList();
        //        List<int> LstAge_Unknown = Enumerable.Repeat(0, 21).ToList();

        //        //Obtain the Illumination Stories node
        //        var umbracoHelper = new Umbraco.Web.UmbracoHelper(Umbraco.Web.UmbracoContext.Current);
        //        IPublishedContent ipIlluminationStories = umbracoHelper.TypedContent((int)Common.siteNode.IlluminationStories);

        //        //Loop through all stories
        //        foreach (IPublishedContent ip in ipIlluminationStories.Children.ToList())
        //        {
        //            //Obtain the content models
        //            var CmIpIlluminationStory = new IlluminationStory(ip);
        //            var CmMember = new ContentModels.Member(CmIpIlluminationStory.Member);

        //            //Increment the proper list
        //            switch (CmIpIlluminationStory.ExperienceType)
        //            {
        //                case "Heavenly":
        //                    AddToAgeLst(ref LstAge_Heavenly, CmMember.Age);
        //                    break;
        //                case "Purgatorial":
        //                    AddToAgeLst(ref LstAge_Purgatorial, CmMember.Age);
        //                    break;
        //                case "Hellish":
        //                    AddToAgeLst(ref LstAge_Hellish, CmMember.Age);
        //                    break;
        //                case "Other or Unsure":
        //                    AddToAgeLst(ref LstAge_Unknown, CmMember.Age);
        //                    break;
        //                default:
        //                    AddToAgeLst(ref LstAge_Heavenly, CmMember.Age);
        //                    break;
        //            }

        //            //Increment total story count
        //            TotalEntries++;
        //        }

        //        //Create average of each entry
        //        for (int i = 0; i < 21; i++)
        //        {
        //            GetAgeAvg(ref LstAge_Heavenly, ref LstAge_Hellish, ref LstAge_Purgatorial, ref LstAge_Unknown, i);
        //        }

        //        //Add final list to class
        //        HeavenlyDataset.LstData = LstAge_Heavenly;
        //        HellishDataset.LstData = LstAge_Hellish;
        //        PurgatorialDataset.LstData = LstAge_Purgatorial;
        //        UnknownDataset.LstData = LstAge_Unknown;
        //    }







        //    //Stringify lists
        //    jsonAgeRange = Newtonsoft.Json.JsonConvert.SerializeObject(LstAgeRange);
        //    HeavenlyDataset.JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(HeavenlyDataset.LstData);
        //    HellishDataset.JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(HellishDataset.LstData);
        //    PurgatorialDataset.JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(PurgatorialDataset.LstData);
        //    UnknownDataset.JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(UnknownDataset.LstData);

        //    //Add Charts to list.
        //    LstChartData.Add(HeavenlyDataset);
        //    LstChartData.Add(HellishDataset);
        //    LstChartData.Add(PurgatorialDataset);
        //    LstChartData.Add(UnknownDataset);

        //    //Determine duration it took to process.
        //    stopwatch.Stop();
        //    Duration = string.Format("Time elapsed: {0:hh\\:mm\\:ss}", stopwatch.Elapsed);
        //}


        //private void AddToAgeLst(ref List<int> LstAge, int Age)
        //{
        //    //
        //    if (Age > 100) Age = 100;

        //    //Determine the index to increment
        //    int ageGrp = 5 * (int)Math.Round(Age / 5.0);
        //    int index = ageGrp / 5;

        //    LstAge[index] += 1;
        //}
        //private void GetAgeAvg(ref List<int> LstAge_Heavenly, ref List<int> LstAge_Hellish, ref List<int> LstAge_Purgatorial, ref List<int> LstAge_Unknown, int index)
        //{
        //    //
        //    int Heavenly = LstAge_Heavenly[index];
        //    int Hellish = LstAge_Hellish[index];
        //    int Purgatorial = LstAge_Purgatorial[index];
        //    int Unknown = LstAge_Unknown[index];

        //    int total = Heavenly + Hellish + Purgatorial + Unknown;

        //    LstAge_Heavenly[index] = (int)(((double)Heavenly / total) * 100);
        //    LstAge_Hellish[index] = (int)(((double)Hellish / total) * 100);
        //    LstAge_Purgatorial[index] = (int)(((double)Purgatorial / total) * 100);
        //    LstAge_Unknown[index] = (int)(((double)Unknown / total) * 100);
        //}
    }
}