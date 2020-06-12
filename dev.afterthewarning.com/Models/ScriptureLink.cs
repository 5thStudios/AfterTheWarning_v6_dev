using System;
using System.ComponentModel.DataAnnotations;


namespace Models
{
    public class ScriptureLink
    {
        public int Id { get; set; }
        public string Breadcrumb { get; set; }
        public string Url { get; set; }
    }
}