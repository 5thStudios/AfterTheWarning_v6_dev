using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using Umbraco;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Core.Services;
using Newtonsoft.Json.Linq;
using System.Text;
using Models;
using Umbraco.Web.PublishedContentModels;
using System.Web.Mvc;
using Umbraco.Web.Extensions;
using Umbraco.Web.Mvc;
using ContentModels = Umbraco.Web.PublishedContentModels;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using X.PagedList;
using Examine;
using UmbracoExamine;
using Examine.Providers;
using Examine.SearchCriteria;


namespace afterthewarning.com.UserControls
{
    public partial class UpdateMsgs : System.Web.Mvc.ViewUserControl
    {
        private UmbracoHelper uhelper = new UmbracoHelper(UmbracoContext.Current);
        private IContentService icService = ApplicationContext.Current.Services.ContentService;

        protected void Page_Load(object sender, EventArgs e)
        {
            //
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int recordCount = 0;
            int updatedCount = 0;
            int errorCount = 0;
            var ipMsgs = uhelper.TypedContent((int)Common.siteNode.MessagesFromHeaven);
            StringBuilder sbErrors = new StringBuilder();

            foreach (var ipMsg in ipMsgs.DescendantsOrSelf().Where(x => x.DocumentTypeAlias == Common.docType.Message))
            {
                try
                {
                    recordCount += 1;

                    if (ipMsg.GetPropertyValue<string>(Common.NodeProperties.content).Contains("&quot;"))
                    //if (ipMsg.GetPropertyValue<string>("Content").Contains("/umbraco/&quot;"))
                    //if (ipMsg.GetPropertyValue<string>("Content").Contains("/&quot;"))
                    {
                        IContent icMsg = icService.GetById(ipMsg.Id);

                        //codeB4.InnerText = icMsg.GetValue<string>(Common.NodeProperties.content);
                        string content = icMsg.GetValue<string>(Common.NodeProperties.content);
                        //codeAfter.InnerText = content.Replace("/umbraco/&quot;", "\\" + "\"").Replace("/&quot;", "\\" + "\"").Replace("&quot;", "\\" + "\"");

                        icMsg.SetValue(Common.NodeProperties.content, content.Replace("/umbraco/&quot;", "\\" + "\"").Replace("/&quot;", "\\" + "\"").Replace("&quot;", "\\" + "\""));
                        icService.SaveAndPublishWithStatus(icMsg);

                        updatedCount += 1;
                        //break;
                    }
                }
                catch (Exception ex)
                {
                    errorCount += 1;
                    sbErrors.AppendLine("Error: [" + ipMsg.Name + "] " + ex.ToString() + "<br /><br />");
                }                
            }

            //Display how many records were updated
            lblRecordCount.Text = updatedCount.ToString();
            lblTotalRecords.Text = recordCount.ToString();
            lblErrors.Text = sbErrors.ToString();

            //Display time lapsed
            sw.Stop();
            lblTimeToProcess.Text = sw.Elapsed.TotalSeconds.ToString();
        }
    }
}


                    //code.InnerText = icMsg.GetValue<string>(Common.NodeProperties.content);
////lblResults.Text = ipMsg.GetPropertyValue<string>("Content");
////lblErrors.Text = icMsg.GetValue<string>("Content");