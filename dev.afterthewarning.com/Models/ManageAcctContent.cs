using formulate.core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;

namespace Models
{
    public class ManageAcctContent
    {
        public string CredentialsUrl { get; set; }
        public string IlluminationStoryUrl { get; set; }
        public string Inactive { get; set; }
        public Boolean IsManageAcctPg { get; set; }
        public Boolean Redirect { get; set; }
        public string RedirectTo { get; set; }

    }
}