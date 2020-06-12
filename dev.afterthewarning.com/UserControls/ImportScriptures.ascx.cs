using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using Umbraco.Web;
using Umbraco.Core.Services;
using Umbraco.Core;
using System.Web;
using Models;
using System.Web.UI;
using System.Diagnostics;
using Umbraco.Core.Models;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using Newtonsoft.Json;
using System.Collections;
using ClientDependency.Core;
using System.Globalization;
using Newtonsoft.Json.Converters;


namespace dev.afterthewarning.com.UserControls
{
    public partial class ImportScriptures : System.Web.Mvc.ViewUserControl
    {
        //
        private UmbracoHelper uhelper = new UmbracoHelper(UmbracoContext.Current);
        private IContentService icService = ApplicationContext.Current.Services.ContentService;



        protected void Page_Load(object sender, EventArgs e)
        {
            //
            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                //Instantiate list of scripture books
                List<Book> lstBooks = new List<Book>();

                //Obtain list of all files within folder
                List<string> lstFiles = Directory.GetFiles(@"C:\Inetpub\vhosts\afterthewarning.com\dev.AfterTheWarning.com\dev.afterthewarning.com\tempData\DouayRheimsBibleXML\").ToList();

                //Loop thru each item in list and convert to class
                foreach (string filePath in lstFiles)
                {
                    try
                    {
                        Book book = null;
                        XmlSerializer serializer = new XmlSerializer(typeof(Book));
                        StreamReader reader = new StreamReader(filePath);
                        book = (Book)serializer.Deserialize(reader);
                        reader.Close();
                        lstBooks.Add(book);
                    }
                    catch (Exception ex)
                    {
                        //lblErrors.Text = "<br />Error: " + filePath + "<br />" + ex.ToString();
                    }
                }


                //Get root folder of bible
                IContent icBible = icService.GetById((int)Common.siteNode.TheDouayRheimsBible);

                //gv1.DataSource = lstBooks.OrderBy(x => int.Parse(x.Order));
                //gv1.DataBind();

                foreach (Book book in lstBooks.OrderBy(x => int.Parse(x.Order)))
                {
                    try
                    {
                        //Add new book
                        IContent icScripture = icService.CreateContentWithIdentity(book.Name, icBible, Common.docType.Scripture);
                        string[] bookName = book.FullName.Split('[');
                        icScripture.SetValue(Common.NodeProperties.fullName, bookName.FirstOrDefault());
                        if (bookName.Count() > 1) { icScripture.SetValue(Common.NodeProperties.alternateName, bookName.LastOrDefault().Replace("]", "")); }
                        if (int.Parse(book.Order) < 47)
                        {
                            icScripture.SetValue(Common.NodeProperties.testament, Common.getPrevalueId(Common.miscellaneous.ScriptureTestaments, Common.miscellaneous.OldTestament));
                        }
                        else
                        {
                            icScripture.SetValue(Common.NodeProperties.testament, Common.getPrevalueId(Common.miscellaneous.ScriptureTestaments, Common.miscellaneous.NewTestament));
                        }
                        icScripture.SetValue(Common.NodeProperties.chapters, book.TotalChapters);

                        int bookId = int.Parse(book.Order);
                        int? bookSet = 0;

                        if (bookId >= 1 && bookId <= 8) { bookSet = Common.getPrevalueId(Common.miscellaneous.ScriptureSet, Common.miscellaneous.ThePentateuchBooks); }
                        else if (bookId >= 9 && bookId <= 21) { bookSet = Common.getPrevalueId(Common.miscellaneous.ScriptureSet, Common.miscellaneous.TheHistoricalBooks); }
                        else if (bookId >= 22 && bookId <= 28) { bookSet = Common.getPrevalueId(Common.miscellaneous.ScriptureSet, Common.miscellaneous.TheWisdomBooks); }
                        else if (bookId >= 29 && bookId <= 46) { bookSet = Common.getPrevalueId(Common.miscellaneous.ScriptureSet, Common.miscellaneous.ThePropheticBooks); }
                        else if (bookId >= 47 && bookId <= 50) { bookSet = Common.getPrevalueId(Common.miscellaneous.ScriptureSet, Common.miscellaneous.TheGospels); }
                        else if (bookId >= 51 && bookId <= 73) { bookSet = Common.getPrevalueId(Common.miscellaneous.ScriptureSet, Common.miscellaneous.TheEpistles); }
                        icScripture.SetValue(Common.NodeProperties.bookSet, bookSet);

                        //Save node
                        icService.SaveAndPublishWithStatus(icScripture);



                        //Add chapters for book
                        foreach (Chapter chapter in book.Chapter)
                        {
                            //Add new chapter
                            IContent icChapter = icService.CreateContentWithIdentity(chapter.Number, icScripture, Common.docType.Chapter);




                            //var scriptureVerse = Serialize.ToJson(chapter.Versicle.ToArray());

                            List<ScriptureVerse> lstScriptureVerses = new List<ScriptureVerse>();
                            foreach (Versicle versicle in chapter.Versicle)
                            {
                                ScriptureVerse scriptureVerse = new ScriptureVerse();
                                scriptureVerse.Verse = Convert.ToInt64(versicle.Number);
                                scriptureVerse.Content = versicle.Text;
                                lstScriptureVerses.Add(scriptureVerse);
                            }

                            //
                            icChapter.SetValue(Common.NodeProperties.verses, JsonConvert.SerializeObject(lstScriptureVerses));

                            //Save node
                            icService.SaveAndPublishWithStatus(icChapter);
                        }
                    }
                    catch (Exception ex)
                    {
                        lblErrors.Text = "<br />Error: " + ex.ToString() + "<br />" + JsonConvert.SerializeObject(book);
                    }
                }
            }
            catch (Exception ex)
            {
                lblErrors.Text = "Big Error: " + ex.ToString();
            }
            finally
            {
                //Display time lapsed
                sw.Stop();
                lblTimeToProcess.Text = sw.Elapsed.TotalSeconds.ToString();
            }
        }

    }
}



        //public static int? getPrevalueId(string dataTypeName, string dataTypeValue)
        //{
        //    // Instantiate datatype service
        //    IDataTypeService dtService = ApplicationContext.Current.Services.DataTypeService;

        //    // Obtain prevalue collection from datatypes
        //    IDataTypeDefinition dtDefinition = dtService.GetDataTypeDefinitionByName(dataTypeName);
        //    PreValueCollection pvCollection = dtService.GetPreValuesCollectionByDataTypeId(dtDefinition.Id);

        //    return pvCollection.PreValuesAsDictionary.FirstOrDefault(preValue => string.Equals(preValue.Value.Value, dataTypeValue)).Value.Id;
        //}
    //namespace QuickType
    //{
    //    public partial class ScriptureVerse
    //    {
    //        [JsonProperty("verse")]
    //        public long Verse { get; set; }

    //        [JsonProperty("content")]
    //        public string Content { get; set; }
    //    }

    //    //public partial class ScriptureVerse
    //    //{
    //    //    public static ScriptureVerse[] FromJson(string json) => JsonConvert.DeserializeObject<ScriptureVerse[]>(json, Converter.Settings);
    //    //}

    //    public static class Serialize
    //    {
    //        public static string ToJson(this ScriptureVerse[] self) => JsonConvert.SerializeObject(self, Converter.Settings);
    //    }

    //    internal static class Converter
    //    {
    //        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    //        {
    //            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
    //            DateParseHandling = DateParseHandling.None,
    //            Converters =
    //        {
    //            new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
    //        },
    //        };
    //    }
    //}

//public class Visionary
//{
//    public int? id { get; set; }
//    public string nodeName { get; set; } = string.Empty;
//}

//public class Msg
//{
//    public int? id { get; set; }
//    public DateTime datePublished { get; set; }
//    public string nodeName { get; set; } = string.Empty;
//    public string pageTitle { get; set; } = string.Empty;
//    public string subtitle { get; set; } = string.Empty;
//    public string author { get; set; } = string.Empty;
//    public string visionary { get; set; } = string.Empty;
//    public int? visionaryId { get; set; }
//    public string originalSource { get; set; } = string.Empty;
//    public string originalSourceName { get; set; } = string.Empty;
//    public List<DateTime> lstDateOfMsgs { get; set; } = new List<DateTime>();
//    public string content { get; set; } = string.Empty;
//}

