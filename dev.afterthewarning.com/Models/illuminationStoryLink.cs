using System;
using System.Collections.Generic;

namespace Models
{
    public class illuminationStoryLink
    {
        public Int32? id { get; set; } = null;
        public Int32? memberId { get; set; } = null;
        public string title { get; set; } = string.Empty;
        public string url { get; set; } = string.Empty;
        public string memberName { get; set; } = string.Empty;
        public string experienceType { get; set; } = string.Empty;
    }
}



//  <add Name = "id" />
//  < add Name="nodeName"/>

//  <add Name = "member" />
//  < add Name="title"/>
//  <add Name = "experienceType" />
//  < add Name="story"/>