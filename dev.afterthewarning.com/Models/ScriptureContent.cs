using formulate.core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;

namespace Models
{
    public class ScriptureContent
    {
        public int chapterCount { get; set; }
        public int currentChapter { get; set; }
        public string bibleUrl { get; set; }
        public dynamic verses { get; set; }
    }
}