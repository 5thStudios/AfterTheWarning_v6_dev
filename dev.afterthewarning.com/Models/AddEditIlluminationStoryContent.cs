using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;

namespace Models
{
    public class AddEditIlluminationStoryContent
    {
        public bool DoesStoryExist { get; set; }
        public IMember Member { get; set; }
    }
}