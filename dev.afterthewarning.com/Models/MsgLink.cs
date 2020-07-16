using System;
using System.ComponentModel.DataAnnotations;


namespace Models
{
    public class MsgLink
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public DateTime Date { get; set; }
        public string Dates { get; set; }
        public string Url { get; set; }

        public int? VisionaryId { get; set; }
        public string VisionaryName { get; set; }
    }
}