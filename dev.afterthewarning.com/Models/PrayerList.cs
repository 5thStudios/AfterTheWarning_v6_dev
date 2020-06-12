using System.Collections.Generic;

namespace Models
{
    public class PrayerList
    {
        public List<PrayerLink> lstPrayerLinks { get; set; } = new List<PrayerLink>();
        public Pagination Pagination { get; set; } = new Pagination();
    }
}