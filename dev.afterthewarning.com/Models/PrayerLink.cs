using System;
using System.ComponentModel.DataAnnotations;


namespace Models
{
    public class PrayerLink
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string MemberName { get; set; }
        public DateTime Date { get; set; }
        public string Url { get; set; }
        public string PrayerSummary { get; set; }

        public int currentPercentage { get; set; }
        public DateTime baseCalculationDate { get; set; }
        public string CandleUrl { get; set; }
    }
}