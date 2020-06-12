using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Core.Services;
using Umbraco.Core;
using Newtonsoft.Json.Linq;
using System.Text;
using Models;
//using Umbraco.Web.PublishedContentModels;

namespace afterthewarning.com.UserControls
{
    public partial class ImportMsgs : System.Web.Mvc.ViewUserControl
    {
        //
        private UmbracoHelper uhelper = new UmbracoHelper(UmbracoContext.Current);
        private IContentService icService = ApplicationContext.Current.Services.ContentService;


        protected void Page_Load(object sender, EventArgs e)
        {
            //NOTE: REMEMBER TO SET THE CONFIG FILE FOR UDATESYFOLDER TO ENABLED AFTER THIS IS COMPLETED!!

            //
            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                //Read the msg json file
                using (StreamReader r = new StreamReader("C:\\Inetpub\\vhosts\\afterthewarning.com\\dev.AfterTheWarning.com\\dev.afterthewarning.com\\tempData\\msgs.txt"))
                {
                    //Extract json and convert back to a list of message classes
                    string json = r.ReadToEnd();
                    List<Msg> lstMsgs = JsonConvert.DeserializeObject<List<Msg>>(json);

                    //Display a record count
                    lblRecordCount.Text = lstMsgs.Count.ToString();

                    //Create lists
                    List<Visionary> lstVisionariesInSite = new List<Visionary>();
                    List<string> lstVisionaryNamesInJson = new List<string>();

                    //Create list of all current visionary folders and obtain IDs if they exist
                    IPublishedContent ipMsgs = uhelper.TypedContent((int)Common.siteNode.MessagesFromHeaven);  
                    foreach (IPublishedContent ipVisionary in ipMsgs.Children.ToList())
                    {
                        var nameExists = lstVisionariesInSite.Any(x => x.nodeName == ipVisionary.Name);
                        if (!nameExists)
                        {
                            Visionary visionary = new Visionary();
                            visionary.nodeName = ipVisionary.Name;
                            visionary.id = ipVisionary.Id;
                            lstVisionariesInSite.Add(visionary);
                        }
                    }

                    //Obtain list of all visionary names within json
                    foreach (Msg msg in lstMsgs)
                    {
                        if (!lstVisionaryNamesInJson.Contains(msg.visionary))
                        {
                            lstVisionaryNamesInJson.Add(msg.visionary);
                        }
                    }

                    //Loop thru and create all missing visionaries
                    foreach (string visionaryName in lstVisionaryNamesInJson)
                    {
                        var nameExists = lstVisionariesInSite.Any(x => x.nodeName == visionaryName);
                        if (!nameExists)
                        {
                            //Add new visionary
                            IContent icMsgs = icService.GetById((int)Common.siteNode.MessagesFromHeaven);
                            IContent icVisionary = icService.CreateContentWithIdentity(visionaryName, icMsgs, Common.docType.Visionary);
                            icVisionary.SetValue(Common.NodeProperties.visionarysName, visionaryName);
                            icService.SaveAndPublishWithStatus(icVisionary);

                            //Add visionary to list in site
                            Visionary visionary = new Visionary();
                            visionary.nodeName = visionaryName;
                            visionary.id = icVisionary.Id;
                            lstVisionariesInSite.Add(visionary);
                        }

                    }

                    //
                    foreach (Msg msg in lstMsgs)
                    {
                        //Obtain the correct visionary folder
                        int visionaryId = (int)lstVisionariesInSite.FirstOrDefault(x => x.nodeName == msg.visionary).id;

                        //Obtain the date folder
                        IContent icDate = getDateFolder(msg.datePublished, visionaryId);

                        //Create new message and save to date folder within visionary folder
                        IContent icMsg = icService.CreateContentWithIdentity(msg.pageTitle, icDate, "message");
                        icMsg.SetValue("publishDate", msg.datePublished);
                        icMsg.SetValue("content", getContentAsJson(msg.content));
                        icMsg.SetValue("dateOfMessages", JsonConvert.SerializeObject(msg.lstDateOfMsgs).Replace("T00:00:00", ""));

                        icService.SaveAndPublishWithStatus(icMsg);
                    }

                    //Display current list of visionaries on site
                    gvVisionaryFolders.DataSource = lstVisionariesInSite;
                    gvVisionaryFolders.DataBind();

                    //Display list of all visionary names in json
                    gvVisionariesInJson.DataSource = lstVisionaryNamesInJson;
                    gvVisionariesInJson.DataBind();

                    //Display list of all data.  (Don't display this when the full version is imported
                    //gv.DataSource = lstMsgs;
                    //gv.DataBind();

                }
            }
            catch (Exception ex)
            {
                lblErrors.Text = ex.ToString();
            }
            finally
            {
                //Display time lapsed
                sw.Stop();
                lblTimeToProcess.Text = sw.Elapsed.TotalSeconds.ToString();
            }
        }




        private IContent getDateFolder(DateTime date, int visionaryId)
        {
            //Obtain the visionary folder
            IPublishedContent ipVisionary = uhelper.TypedContent(visionaryId);
            IPublishedContent ipYear;
            IPublishedContent ipMonth;
            IPublishedContent ipDay;
            IContent icVisionary = icService.GetById(visionaryId);
            IContent icYear;
            IContent icMonth;
            IContent icDay;

            //Obtain or create the year folder.
            var yearExist = ipVisionary.Children.Any(x => x.Name == date.Year.ToString());
            if (yearExist)
            {
                ipYear = ipVisionary.Children.FirstOrDefault(x => x.Name == date.Year.ToString());
                icYear = icService.GetById(ipYear.Id);
            }
            else
            {
                icYear = icService.CreateContentWithIdentity(date.Year.ToString(), icVisionary, "uDateFoldersyFolderYear");
                icService.SaveAndPublishWithStatus(icYear);
                ipYear = uhelper.TypedContent(icYear.Id);
            }


            //Obtain or create the month folder.
            var monthExist = ipYear.Children.Any(x => x.Name == date.ToString("MMMM"));
            if (monthExist)
            {
                ipMonth = ipYear.Children.FirstOrDefault(x => x.Name == date.ToString("MMMM"));
                icMonth = icService.GetById(ipMonth.Id);
            }
            else
            {
                icMonth = icService.CreateContentWithIdentity(date.ToString("MMMM"), icYear, "uDateFoldersyFolderMonth");
                icService.SaveAndPublishWithStatus(icMonth);
                ipMonth = uhelper.TypedContent(icMonth.Id);
            }


            ////Obtain or create the month folder.
            //var monthExist = ipYear.Children.Any(x => x.Name == date.Month.ToString("#00"));
            //if (monthExist)
            //{
            //    ipMonth = ipYear.Children.FirstOrDefault(x => x.Name == date.Month.ToString("#00"));
            //    icMonth = icService.GetById(ipMonth.Id);
            //}
            //else
            //{
            //    icMonth = icService.CreateContentWithIdentity(date.Month.ToString("#00"), icYear, "uDateFoldersyFolderMonth");
            //    icService.SaveAndPublishWithStatus(icMonth);
            //    ipMonth = uhelper.TypedContent(icMonth.Id);
            //}


            //Obtain or create the day folder.
            var dayExist = ipMonth.Children.Any(x => x.Name == date.Day.ToString("#00"));
            if (dayExist)
            {
                ipDay = ipMonth.Children.FirstOrDefault(x => x.Name == date.Day.ToString("#00"));
                icDay = icService.GetById(ipDay.Id);
            }
            else
            {
                icDay = icService.CreateContentWithIdentity(date.Day.ToString("#00"), icMonth, "uDateFoldersyFolderDay");
                icService.SaveAndPublishWithStatus(icDay);
                ipDay = uhelper.TypedContent(icDay.Id);
            }


            return icDay;
        }
        private string getContentAsJson(string content)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"{");
            sb.Append(@"""name"": ""1 column layout"",");
            sb.Append(@"""sections"": [");
            sb.Append(@"{");
            sb.Append(@"""grid"": 24,");
            sb.Append(@"""allowAll"": true,");
            sb.Append(@"""rows"": [");
            sb.Append(@"{");
            sb.Append(@"""label"": """",");
            sb.Append(@"""name"": ""x24"",");
            sb.Append(@"""areas"": [");
            sb.Append(@"{");
            sb.Append(@"""grid"": 24,");
            sb.Append(@"""allowAll"": true,");
            sb.Append(@"""hasConfig"": false,");
            sb.Append(@"""controls"": [");
            sb.Append(@"{");
            sb.Append(@"""value"": """);

            string newContent = System.Net.WebUtility.HtmlDecode(content).Replace(System.Environment.NewLine, " ").Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");
            newContent = newContent.Replace("\"", "&quot;");
            newContent = SanitizeReceivedJson(newContent);
            sb.Append(newContent);

            sb.Append(@""",");
            sb.Append(@"""editor"": {");
            sb.Append(@"""alias"": ""rte""");
            sb.Append(@"},");
            sb.Append(@"""active"": false");
            sb.Append(@"}");
            sb.Append(@"],");
            sb.Append(@"""active"": false");
            sb.Append(@"}");
            sb.Append(@"],");
            sb.Append(@"""hasConfig"": false,");
            sb.Append(@"""id"": """);
            sb.Append(Guid.NewGuid().ToString());
            sb.Append(@""",");
            sb.Append(@"""active"": false");
            sb.Append(@"}");
            sb.Append(@"]");
            sb.Append(@"}");
            sb.Append(@"]");
            sb.Append(@"}");

            return sb.ToString();


        }
        private string SanitizeReceivedJson(string uglyJson)
        {
            var sb = new StringBuilder(uglyJson);
            sb.Replace("\\\t", "\t");
            sb.Replace("\\\n", "\n");
            sb.Replace("\\\r", "\r");
            return sb.ToString();
        }
        
    }
}

public class Visionary
{
    public int? id { get; set; }
    public string nodeName { get; set; } = string.Empty;
}

public class Msg
{
    public int? id { get; set; }
    public DateTime datePublished { get; set; }
    public string nodeName { get; set; } = string.Empty;
    public string pageTitle { get; set; } = string.Empty;
    public string subtitle { get; set; } = string.Empty;
    public string author { get; set; } = string.Empty;
    public string visionary { get; set; } = string.Empty;
    public int? visionaryId { get; set; }
    public string originalSource { get; set; } = string.Empty;
    public string originalSourceName { get; set; } = string.Empty;
    public List<DateTime> lstDateOfMsgs { get; set; } = new List<DateTime>();
    public string content { get; set; } = string.Empty;
}

