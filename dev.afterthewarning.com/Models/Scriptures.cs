using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Models
{
    [XmlRoot(ElementName = "Versicle")]
    public class Versicle
    {
        [XmlAttribute(AttributeName = "Number")]
        public string Number { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Chapter")]
    public class Chapter
    {
        [XmlElement(ElementName = "Versicle")]
        public List<Versicle> Versicle { get; set; }
        [XmlAttribute(AttributeName = "Number")]
        public string Number { get; set; }
    }

    [XmlRoot(ElementName = "Book")]
    public class Book
    {
        [XmlElement(ElementName = "Chapter")]
        public List<Chapter> Chapter { get; set; }
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "Order")]
        public string Order { get; set; }
        [XmlAttribute(AttributeName = "TotalChapters")]
        public string TotalChapters { get; set; }
        [XmlAttribute(AttributeName = "FullName")]
        public string FullName { get; set; }
    }



    public partial class ScriptureVerse
    {
        [JsonProperty("verse")]
        public long Verse { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }
}