using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

namespace Models
{
    public class PaginationContent
    {
        public Uri baseUri { get; set; }
        public NameValueCollection queryString { get; set; }
        public string baseUrl { get; set; }
        public int previous { get; set; }
        public int next { get; set; }

    }
}