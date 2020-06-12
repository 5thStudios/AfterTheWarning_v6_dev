using System.Collections.Generic;

namespace Models
{
    public class IlluminationStoryList
    {
        public int HeavenlyExperienceCount { get; set; } = 0;
        public int HellishExperienceCount { get; set; } = 0;
        public int PurgatorialExperienceCount { get; set; } = 0;
        public int OtherExperienceCount { get; set; } = 0;

        public string SortBy { get; set; } = "";
        public string ViewBy { get; set; } = "";

        public List<Models.illuminationStoryLink> lstStoryLink { get; set; } = new List<Models.illuminationStoryLink>();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}
