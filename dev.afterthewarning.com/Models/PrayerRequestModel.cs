using System;
using System.ComponentModel.DataAnnotations;


namespace Models
{
    public class PrayerRequestModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Url { get; set; }
        public DateTime date { get; set; }
        public int totalPrayersOffered { get; set; }

        [Required(ErrorMessage = "*Title is required")]
        [DataType(DataType.Text)]
        [Display(Name = "Title")]
        public string PrayerTitle { get; set; }


        [Required(ErrorMessage = "*Prayer is required")]
        [DataType(DataType.Text)]
        [Display(Name = "Prayer")]
        public string Prayer { get; set; }

    }
}