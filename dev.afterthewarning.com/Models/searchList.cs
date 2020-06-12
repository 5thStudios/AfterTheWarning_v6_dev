using System.Collections.Generic;

namespace Models
{
    public class SearchList
    {
        public string ErrorMsg { get; set; } = "";
        public bool ShowErrorMsg { get; set; } = false;
        public bool ShowResults { get; set; } = false;

        public string SearchFor { get; set; } = "";
        public string SearchIn { get; set; } = "";
        public string SearchInTitle { get; set; } = "";

        public bool ShowIlluminationStories { get; set; } = false;
        public List<Models.illuminationStoryLink> lstStoryLink { get; set; } = new List<Models.illuminationStoryLink>();

        public bool ShowMsgsFromHeaven { get; set; } = false;
        public List<MsgLink> lstMsgsFromHeavenLinks { get; set; } = new List<MsgLink>();

        public bool ShowArticles { get; set; } = false;
        public List<ArticleLink> lstArticleLinks { get; set; } = new List<ArticleLink>();

        public bool ShowPrayers { get; set; } = false;
        public List<PrayerLink> lstPrayerLinks { get; set; } = new List<PrayerLink>();

        public bool ShowBible { get; set; } = false;
        public List<ScriptureLink> lstBibleLinks { get; set; } = new List<ScriptureLink>();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}
