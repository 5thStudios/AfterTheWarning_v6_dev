using formulate.core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;

namespace Models
{
    public class TOCContent
    {
        public string currentTestament { get; set; }
        public string currentBookset { get; set; }
        public string prefaceUrl { get; set; }
        public tocBook book { get; set; }
        public tocBookSet bookSet { get; set; }
        public tocTestament testament { get; set; }
        public Models.TableOfContent toc { get; set; }



        public TOCContent()
        {
            bookSet = new tocBookSet();
            testament = new tocTestament();
            toc = new Models.TableOfContent();
        }
    }
}