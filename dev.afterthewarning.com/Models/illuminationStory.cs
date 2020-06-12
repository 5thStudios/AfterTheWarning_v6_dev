using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Umbraco.Core.Models;
using ContentModels = Umbraco.Web.PublishedContentModels;


namespace Models
{
    public class illuminationStory
    {

        [Required(ErrorMessage = "*Enter a title for your story")]
        [DataType(DataType.Text)]
        [Display(Name = "Title")]
        public string Title { get; set; }


        [Required(ErrorMessage = "*Add your story")]
        [StringLength(int.MaxValue, ErrorMessage = "*Please add your FULL story before submitting", MinimumLength = 100)]
        [DataType(DataType.Text)]
        [Display(Name = "Story")]
        public string Story { get; set; }


        [Required(ErrorMessage = "*Select what type of experience you had")]
        public Common.ExperienceTypes? ExperienceType { get; set; }


        public int storyId { get; set; }
        public int memberId { get; set; }
        public string Author { get; set; } = string.Empty;
        public int? Age { get; set; } = null;


        public Common.Genders? Gender { get; set; }
        public Common.Countries? Country { get; set; }
        public Common.Races? Race { get; set; }
        public Common.Religions? Religion { get; set; }
        
        public List<string> lstAge { get; set; } = new List<string>();



        //public IPublishedContent IpIlluminationStory { get; set; } = null;
        //var p = new ContentModels.IlluminationStory(ipIlluminationStory);
        //var m = new ContentModels.Member(p.Member);
        public ContentModels.IlluminationStory CmIpIlluminationStory { get; set; }
        public ContentModels.Member CmMember { get; set; }

    }
}