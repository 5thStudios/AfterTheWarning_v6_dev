using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Extensions;
using Umbraco.Web.Mvc;
using ContentModels = Umbraco.Web.PublishedContentModels;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Umbraco.Web.PublishedContentModels;
using X.PagedList;
using Examine;
using UmbracoExamine;
using Examine.Providers;
using Examine.SearchCriteria;
using Examine.LuceneEngine.SearchCriteria;

namespace Controllers
{
    public class MessageController : SurfaceController
    {
        #region "Renders"
        public ActionResult RenderAllMessages()
        {
            //Instantiate variables
            MsgList msgList = new MsgList();
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);


            try
            {
                //Get all prayers
                BaseSearchProvider mySearcher = ExamineManager.Instance.SearchProviderCollection[Common.searchProviders.MessagesSearcher];
                ISearchCriteria criteria = mySearcher.CreateSearchCriteria(IndexTypes.Content);
                IBooleanOperation query = criteria.Field(Common.NodeProperties.indexType, Common.NodeProperties.content); //gets all items
                query.And().OrderByDescending(Common.NodeProperties.publishDate);
                query.And().OrderBy(Common.NodeProperties.nodeName);
                ISearchResults isResults = mySearcher.Search(query.Compile());


                //Get item counts and total experiences.
                msgList.Pagination.itemsPerPage = 20;
                msgList.Pagination.totalItems = isResults.Count();


                //Determine how many pages/items to skip and take, as well as the total page count for the search result.
                if (msgList.Pagination.totalItems > msgList.Pagination.itemsPerPage)
                {
                    msgList.Pagination.totalPages = (int)Math.Ceiling((double)msgList.Pagination.totalItems / (double)msgList.Pagination.itemsPerPage);
                }
                else
                {
                    msgList.Pagination.itemsPerPage = msgList.Pagination.totalItems;
                    msgList.Pagination.totalPages = 1;
                }


                //Determine current page number 
                var pageNo = 1;
                if (!string.IsNullOrEmpty(Request.QueryString[Common.miscellaneous.PageNo]))
                {
                    int.TryParse(Request.QueryString[Common.miscellaneous.PageNo], out pageNo);
                    if (pageNo <= 0 || pageNo > msgList.Pagination.totalPages)
                    {
                        pageNo = 1;
                    }
                }
                msgList.Pagination.pageNo = pageNo;


                //Determine how many pages/items to skip
                if (msgList.Pagination.totalItems > msgList.Pagination.itemsPerPage)
                {
                    msgList.Pagination.itemsToSkip = msgList.Pagination.itemsPerPage * (pageNo - 1);
                }


                //Convert list of SearchResults to list of classes
                foreach (SearchResult sRecord in isResults.Skip(msgList.Pagination.itemsToSkip).Take(msgList.Pagination.itemsPerPage))
                {
                    var msgLink = new Models.MsgLink();
                    msgLink.Id = sRecord.Id;
                    msgLink.Title = sRecord.Fields[Common.NodeProperties.nodeName];
                    msgLink.Subtitle = sRecord.Fields[Common.NodeProperties.subtitle];
                    msgLink.Url = Umbraco.NiceUrl(sRecord.Id);
                    //msgLink.Date = Convert.ToDateTime(sRecord.Fields[Common.NodeProperties.publishDate]);
                    msgLink.Dates = (Convert.ToDateTime(sRecord.Fields[Common.NodeProperties.publishDate])).ToString("MMMM dd");

                    msgList.lstMsgLinks.Add(msgLink);
                }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"MessageController.cs : RenderAllMessages()");
                sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(msgList));
                Common.SaveErrorMessage(ex, sb, typeof(MessageController));


                ModelState.AddModelError("", "*An error occured while retrieving all messages.");
                return CurrentUmbracoPage();
            }


            //Return data to partialview
            return PartialView("~/Views/Partials/MessagesFromHeaven/_msgList.cshtml", msgList);
        }
        public ActionResult RenderLatestMessages()
        {
            //Instantiate variables
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
            List<latestUpdates> lstLatestUpdates = new List<latestUpdates>();


            try
            {
                //Get all prayers
                BaseSearchProvider mySearcher = ExamineManager.Instance.SearchProviderCollection[Common.searchProviders.MessagesSearcher];
                ISearchCriteria criteria = mySearcher.CreateSearchCriteria(IndexTypes.Content);
                IBooleanOperation query = criteria.Field(Common.NodeProperties.indexType, Common.NodeProperties.content); //gets all items
                query.And().OrderByDescending(Common.NodeProperties.publishDate);
                query.And().OrderBy(Common.NodeProperties.nodeName);
                query.And().OrderBy(Common.miscellaneous.Path);
                ISearchResults isResults = mySearcher.Search(query.Compile());


                if (isResults.Any())
                {
                    //Instantiate variables
                    DateTime msgDate = new DateTime(1900, 1, 1);
                    DateTime prevDate = new DateTime(1900, 1, 1);
                    latestUpdates latestUpdate = new latestUpdates();
                    visionary visionary = new visionary();
                    message message;
                    IPublishedContent ipMsg;
                    IPublishedContent ipVisionary;


                    //Get top 'n' results and determine link structure
                    foreach (SearchResult srResult in isResults.Take(20))
                    {
                        //Obtain message's node
                        ipMsg = Umbraco.TypedContent(Convert.ToInt32(srResult.Id));
                        if (ipMsg != null)
                        {
                            //Obtain date of message
                            msgDate = ipMsg.GetPropertyValue<DateTime>(Common.NodeProperties.publishDate);

                            //Create a new date for messages
                            if (msgDate != prevDate)
                            {
                                //Update current date
                                prevDate = msgDate;

                                //Create new instances for updates and add to list of all updates.
                                latestUpdate = new latestUpdates();
                                latestUpdate.datePublished = msgDate;
                                lstLatestUpdates.Add(latestUpdate);

                                //Reset the visionary class on every new date change.
                                visionary = new visionary();
                            }

                            //Obtain current visionary or webmaster
                            if (ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.DocumentTypeAlias == Common.docType.Visionary) != null)
                            {
                                if (visionary.id != ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.DocumentTypeAlias == Common.docType.Visionary).Id)
                                {
                                    //Obtain visionary node
                                    ipVisionary = ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.DocumentTypeAlias == Common.docType.Visionary);

                                    //Create new visionary class and add to latest update class
                                    visionary = new visionary();
                                    visionary.id = ipVisionary.Id;
                                    visionary.name = ipVisionary.Name;
                                    visionary.url = ipVisionary.Url;
                                    latestUpdate.lstVisionaries.Add(visionary);
                                }
                            }
                            else if (ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.DocumentTypeAlias == Common.docType.WebmasterMessageList) != null)
                            {
                                if (visionary.id != ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.DocumentTypeAlias == Common.docType.WebmasterMessageList).Id)
                                {
                                    //Obtain visionary node
                                    ipVisionary = ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.DocumentTypeAlias == Common.docType.WebmasterMessageList);

                                    //Create new visionary class and add to latest update class
                                    visionary = new visionary();
                                    visionary.id = ipVisionary.Id;
                                    visionary.name = ipVisionary.Name;
                                    visionary.url = ipVisionary.Url;
                                    latestUpdate.lstVisionaries.Add(visionary);
                                }
                            }

                            //Create new message and add to existing visionary class.
                            message = new message();
                            message.id = ipMsg.Id;
                            message.title = ipMsg.Name;
                            message.url = ipMsg.Url;
                            visionary.lstMessages.Add(message);
                        }
                    }
                }


                //Return data to partialview
                return PartialView("~/Views/Partials/Common/LatestUpdates.cshtml", lstLatestUpdates);
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"MessageController.cs : RenderLatestMessages()");
                sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(lstLatestUpdates));
                Common.SaveErrorMessage(ex, sb, typeof(MessageController));


                ModelState.AddModelError("", "*An error occured while creating the latest message list.");
                return CurrentUmbracoPage();
            }
        }
        //public ActionResult RenderMsgs_byVisionary(IPublishedContent ipVisionary)
        //{
        //    //Instantiate variables
        //    MsgList msgList = new MsgList();
        //    var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);


        //    try
        //    {
        //        msgList.VisionaryName = "working";

        //        //Get all prayers
        //        BaseSearchProvider mySearcher = ExamineManager.Instance.SearchProviderCollection[Common.searchProviders.MessagesSearcher];
        //        ISearchCriteria criteria = mySearcher.CreateSearchCriteria(IndexTypes.Content);
        //        IBooleanOperation query = criteria.Field(Common.NodeProperties.indexType, Common.NodeProperties.content); //gets all items when this exists for every record. 
        //        query.And().OrderByDescending(Common.NodeProperties.publishDate);
        //        query.And().OrderBy(Common.NodeProperties.nodeName);
        //        query.And().Field(Common.miscellaneous.Path, ipVisionary.Path.MultipleCharacterWildcard());
        //        ISearchResults isResults = mySearcher.Search(query.Compile());


        //        //Get item counts and total experiences.
        //        msgList.Pagination.itemsPerPage = 30;
        //        msgList.Pagination.totalItems = isResults.Count();


        //        //Determine how many pages/items to skip and take, as well as the total page count for the search result.
        //        if (msgList.Pagination.totalItems > msgList.Pagination.itemsPerPage)
        //        {
        //            msgList.Pagination.totalPages = (int)Math.Ceiling((double)msgList.Pagination.totalItems / (double)msgList.Pagination.itemsPerPage);
        //        }
        //        else
        //        {
        //            msgList.Pagination.itemsPerPage = msgList.Pagination.totalItems;
        //            msgList.Pagination.totalPages = 1;
        //        }


        //        //Determine current page number 
        //        var pageNo = 1;
        //        if (!string.IsNullOrEmpty(Request.QueryString[Common.miscellaneous.PageNo]))
        //        {
        //            int.TryParse(Request.QueryString[Common.miscellaneous.PageNo], out pageNo);
        //            if (pageNo <= 0 || pageNo > msgList.Pagination.totalPages)
        //            {
        //                pageNo = 1;
        //            }
        //        }
        //        msgList.Pagination.pageNo = pageNo;


        //        //Determine how many pages/items to skip
        //        if (msgList.Pagination.totalItems > msgList.Pagination.itemsPerPage)
        //        {
        //            msgList.Pagination.itemsToSkip = msgList.Pagination.itemsPerPage * (pageNo - 1);
        //        }


        //        //Convert list of SearchResults to list of classes
        //        foreach (SearchResult sRecord in isResults.Skip(msgList.Pagination.itemsToSkip).Take(msgList.Pagination.itemsPerPage))
        //        {
        //            var msgLink = new Models.MsgLink();
        //            msgLink.Id = sRecord.Id;
        //            msgLink.Title = sRecord.Fields[Common.NodeProperties.nodeName];
        //            msgLink.Subtitle = sRecord.Fields[Common.NodeProperties.subtitle];
        //            msgLink.Url = Umbraco.NiceUrl(sRecord.Id);


        //            //msgLink.Date = Convert.ToDateTime(sRecord.Fields[Common.NodeProperties.publishDate]);


        //            //Obtain list of all dates
        //            var ipMsg = Umbraco.TypedContent(sRecord.Id);
        //            List<DateTime> lstDateRange = ipMsg.GetPropertyValue<List<DateTime>>(Common.NodeProperties.dateOfMessages);


        //            //Determine proper date range for messages
        //            if (lstDateRange != null && lstDateRange.Count > 0)
        //            {
        //                if (lstDateRange.Count == 1)
        //                {
        //                    msgLink.Dates = lstDateRange.First().ToString("MMM d");
        //                }
        //                else
        //                {
        //                    StringBuilder sbDateRange = new StringBuilder();
        //                    sbDateRange.Append(lstDateRange.First().ToString("MMM d"));
        //                    sbDateRange.Append(" — ");
        //                    sbDateRange.Append(lstDateRange.Last().ToString("MMM d"));

        //                    msgLink.Dates = sbDateRange.ToString();
        //                }
        //            }
        //            else
        //            {
        //                msgLink.Dates = Convert.ToDateTime(sRecord.Fields[Common.NodeProperties.publishDate]).ToString("MMM d");
        //            }

        //            msgList.lstMsgLinks.Add(msgLink);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        sb.AppendLine(@"MessageController.cs : RenderMsgs_byVisionary()");
        //        //sb.AppendLine("ipVisionary:" + Newtonsoft.Json.JsonConvert.SerializeObject(ipVisionary));
        //        sb.AppendLine("msgList:" + Newtonsoft.Json.JsonConvert.SerializeObject(msgList));
        //        Common.SaveErrorMessage(ex, sb, typeof(MessageController));


        //        ModelState.AddModelError("", "*An error occured while retrieving messages by visionary.");
        //    }


        //    //Return data to partialview
        //    return PartialView("~/Views/Partials/MessagesFromHeaven/_msgList.cshtml", msgList);
        //}



        public ActionResult RenderMsgs_byVisionary(IPublishedContent ipVisionary)
        {
            //Instantiate variables
            MsgList msgList = new MsgList();
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);


            try
            {
                msgList.VisionaryName = "working";

                //Get all prayers
                BaseSearchProvider mySearcher = ExamineManager.Instance.SearchProviderCollection[Common.searchProviders.MessagesSearcher];
                ISearchCriteria criteria = mySearcher.CreateSearchCriteria(IndexTypes.Content);
                IBooleanOperation query = criteria.Field(Common.NodeProperties.indexType, Common.NodeProperties.content); //gets all items when this exists for every record. 
                query.And().OrderByDescending(Common.NodeProperties.publishDate);
                query.And().OrderBy(Common.NodeProperties.nodeName);
                query.And().Field(Common.miscellaneous.Path, ipVisionary.Path.MultipleCharacterWildcard());
                ISearchResults isResults = mySearcher.Search(query.Compile());


                //Get item counts and total experiences.
                msgList.Pagination.itemsPerPage = 30;
                msgList.Pagination.totalItems = isResults.Count();


                //Determine how many pages/items to skip and take, as well as the total page count for the search result.
                if (msgList.Pagination.totalItems > msgList.Pagination.itemsPerPage)
                {
                    msgList.Pagination.totalPages = (int)Math.Ceiling((double)msgList.Pagination.totalItems / (double)msgList.Pagination.itemsPerPage);
                }
                else
                {
                    msgList.Pagination.itemsPerPage = msgList.Pagination.totalItems;
                    msgList.Pagination.totalPages = 1;
                }


                //Determine current page number 
                var pageNo = 1;
                if (!string.IsNullOrEmpty(Request.QueryString[Common.miscellaneous.PageNo]))
                {
                    int.TryParse(Request.QueryString[Common.miscellaneous.PageNo], out pageNo);
                    if (pageNo <= 0 || pageNo > msgList.Pagination.totalPages)
                    {
                        pageNo = 1;
                    }
                }
                msgList.Pagination.pageNo = pageNo;


                //Determine how many pages/items to skip
                if (msgList.Pagination.totalItems > msgList.Pagination.itemsPerPage)
                {
                    msgList.Pagination.itemsToSkip = msgList.Pagination.itemsPerPage * (pageNo - 1);
                }


                //Convert list of SearchResults to list of classes
                foreach (SearchResult sRecord in isResults) //.Skip(msgList.Pagination.itemsToSkip).Take(msgList.Pagination.itemsPerPage))
                {
                    var msgLink = new Models.MsgLink();
                    msgLink.Id = sRecord.Id;
                    msgLink.Title = sRecord.Fields[Common.NodeProperties.nodeName];
                    msgLink.Subtitle = sRecord.Fields[Common.NodeProperties.subtitle];
                    msgLink.Url = Umbraco.NiceUrl(sRecord.Id);


                    /* msgLink.Date = */
                    Convert.ToDateTime(sRecord.Fields[Common.NodeProperties.publishDate]);


                    //Obtain list of all dates
                    var ipMsg = Umbraco.TypedContent(sRecord.Id);
                    List<DateTime> lstDateRange = ipMsg.GetPropertyValue<List<DateTime>>(Common.NodeProperties.dateOfMessages);


                    //Determine proper date range for messages
                    if (lstDateRange != null && lstDateRange.Count > 0)
                    {
                        if (lstDateRange.Count == 1)
                        {
                            msgLink.Dates = lstDateRange.First().ToString("MMM d");
                        }
                        else
                        {
                            StringBuilder sbDateRange = new StringBuilder();
                            sbDateRange.Append(lstDateRange.First().ToString("MMM d"));
                            sbDateRange.Append(" — ");
                            sbDateRange.Append(lstDateRange.Last().ToString("MMM d"));

                            msgLink.Dates = sbDateRange.ToString();
                        }

                        msgLink.Date = lstDateRange.First(); //Used for resorting list before displaying
                    }
                    else
                    {
                        msgLink.Dates = Convert.ToDateTime(sRecord.Fields[Common.NodeProperties.publishDate]).ToString("MMM d");
                        msgLink.Date = Convert.ToDateTime(sRecord.Fields[Common.NodeProperties.publishDate]); //Used for resorting list before displaying
                    }

                    msgList.lstMsgLinks.Add(msgLink);
                }

                //Reorder messages by date and obtain only what is to be displayed.
                msgList.lstMsgLinks = msgList.lstMsgLinks.OrderByDescending(x => x.Date).Skip(msgList.Pagination.itemsToSkip).Take(msgList.Pagination.itemsPerPage).ToList();

            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"MessageController.cs : RenderMsgs_byVisionary()");
                //sb.AppendLine("ipVisionary:" + Newtonsoft.Json.JsonConvert.SerializeObject(ipVisionary));
                sb.AppendLine("msgList:" + Newtonsoft.Json.JsonConvert.SerializeObject(msgList));
                Common.SaveErrorMessage(ex, sb, typeof(MessageController));


                ModelState.AddModelError("", "*An error occured while retrieving messages by visionary.");
            }


            //Return data to partialview
            return PartialView("~/Views/Partials/MessagesFromHeaven/_msgList.cshtml", msgList);
        }
        #endregion



        #region "Renders"
        public static List<latestUpdates> ObtainLatestMessages()
        {
            //Instantiate variables
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
            List<latestUpdates> lstLatestUpdates = new List<latestUpdates>();


            try
            {
                //Get all messages
                BaseSearchProvider mySearcher = ExamineManager.Instance.SearchProviderCollection[Common.searchProviders.MessagesSearcher];
                ISearchCriteria criteria = mySearcher.CreateSearchCriteria(IndexTypes.Content);
                IBooleanOperation query = criteria.Field(Common.NodeProperties.indexType, Common.NodeProperties.content); //gets all items
                query.And().OrderByDescending(Common.NodeProperties.publishDate);
                query.And().OrderBy(Common.NodeProperties.nodeName);
                query.And().OrderBy(Common.miscellaneous.Path);
                ISearchResults isResults = mySearcher.Search(query.Compile());


                if (isResults.Any())
                {
                    //Instantiate variables
                    DateTime msgDate = new DateTime(1900, 1, 1);
                    //DateTime prevDate = new DateTime(1900, 1, 1);
                    latestUpdates latestUpdate = new latestUpdates();
                    visionary visionary = new visionary();
                    message message;
                    IPublishedContent ipMsg;
                    IPublishedContent ipVisionary;
                    Boolean isFirst = true;


                    //Get top 'n' results and determine link structure
                    foreach (SearchResult srResult in isResults)
                    {
                        //Obtain message's node
                        ipMsg = umbracoHelper.TypedContent(Convert.ToInt32(srResult.Id));
                        if (ipMsg != null)
                        {
                            if (isFirst)
                            {
                                //Obtain date of latest message
                                msgDate = ipMsg.GetPropertyValue<DateTime>(Common.NodeProperties.publishDate);
                                isFirst = false;
                            }
                            else
                            {
                                //Exit loop if a different publish date exists
                                if (msgDate != ipMsg.GetPropertyValue<DateTime>(Common.NodeProperties.publishDate))
                                {
                                    break;
                                }
                            }


                            //Create a new date for messages
                            //if (msgDate != prevDate)
                            //{
                            //    //Update current date
                            //    prevDate = msgDate;

                            //Create new instances for updates and add to list of all updates.
                            latestUpdate = new latestUpdates();
                            latestUpdate.datePublished = msgDate;
                            lstLatestUpdates.Add(latestUpdate);

                            //Reset the visionary class on every new date change.
                            visionary = new visionary();
                            //}

                            //Obtain current visionary or webmaster
                            if (ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.DocumentTypeAlias == Common.docType.Visionary) != null)
                            {
                                if (visionary.id != ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.DocumentTypeAlias == Common.docType.Visionary).Id)
                                {
                                    //Obtain visionary node
                                    ipVisionary = ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.DocumentTypeAlias == Common.docType.Visionary);

                                    //Create new visionary class and add to latest update class
                                    visionary = new visionary();
                                    visionary.id = ipVisionary.Id;
                                    visionary.name = ipVisionary.Name;
                                    visionary.url = ipVisionary.UrlAbsolute();
                                    latestUpdate.lstVisionaries.Add(visionary);
                                }
                            }
                            else if (ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.DocumentTypeAlias == Common.docType.WebmasterMessageList) != null)
                            {
                                if (visionary.id != ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.DocumentTypeAlias == Common.docType.WebmasterMessageList).Id)
                                {
                                    //Obtain visionary node
                                    ipVisionary = ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.DocumentTypeAlias == Common.docType.WebmasterMessageList);

                                    //Create new visionary class and add to latest update class
                                    visionary = new visionary();
                                    visionary.id = ipVisionary.Id;
                                    visionary.name = ipVisionary.Name;
                                    visionary.url = ipVisionary.UrlAbsolute();
                                    latestUpdate.lstVisionaries.Add(visionary);
                                }
                            }

                            //Create new message and add to existing visionary class.
                            message = new message();
                            message.id = ipMsg.Id;
                            message.title = ipMsg.Name;
                            message.url = ipMsg.UrlAbsolute();
                            visionary.lstMessages.Add(message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //StringBuilder sb = new StringBuilder();
                //sb.AppendLine(@"MessageController.cs : RenderLatestMessages()");
                //sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(lstLatestUpdates));
                //Common.saveErrorMessage(ex.ToString(), sb.ToString());


                //ModelState.AddModelError("", "*An error occured while creating the latest message list.");
                //return CurrentUmbracoPage();
            }


            //
            return lstLatestUpdates;
        }




        public static LatestUpdateList ObtainAllMessages(int pageNo = 1)
        {

            //Instantiate variables
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
            LatestUpdateList lstLatestUpdates = new LatestUpdateList();


            //try
            //{
            //Get all ...
            BaseSearchProvider mySearcher = ExamineManager.Instance.SearchProviderCollection[Common.searchProviders.MessagesSearcher];
            ISearchCriteria criteria = mySearcher.CreateSearchCriteria(IndexTypes.Content);
            IBooleanOperation query = criteria.Field(Common.NodeProperties.indexType, Common.NodeProperties.content); //gets all items
            query.And().OrderByDescending(Common.NodeProperties.publishDate);
            query.And().OrderBy(Common.NodeProperties.nodeName);
            query.And().OrderBy(Common.miscellaneous.Path);
            ISearchResults isResults = mySearcher.Search(query.Compile());

            if (isResults.Any())
            {
                //Instantiate variables
                DateTime msgDate = new DateTime(1900, 1, 1);
                DateTime prevDate = new DateTime(1900, 1, 1);
                latestUpdates latestUpdate = new latestUpdates();
                visionary visionary = new visionary();
                message message;
                IPublishedContent ipMsg;
                IPublishedContent ipVisionary;



                //Get item counts and total experiences.
                lstLatestUpdates.Pagination.itemsPerPage = 20;
                lstLatestUpdates.Pagination.totalItems = isResults.Count();


                //Determine how many pages/items to skip and take, as well as the total page count for the search result.
                if (lstLatestUpdates.Pagination.totalItems > lstLatestUpdates.Pagination.itemsPerPage)
                {
                    lstLatestUpdates.Pagination.totalPages = (int)Math.Ceiling((double)lstLatestUpdates.Pagination.totalItems / (double)lstLatestUpdates.Pagination.itemsPerPage);
                }
                else
                {
                    lstLatestUpdates.Pagination.itemsPerPage = lstLatestUpdates.Pagination.totalItems;
                    lstLatestUpdates.Pagination.totalPages = 1;
                }


                //Determine current page number 
                if (pageNo <= 0 || pageNo > lstLatestUpdates.Pagination.totalPages)
                {
                    pageNo = 1;
                }
                lstLatestUpdates.Pagination.pageNo = pageNo;


                //Determine how many pages/items to skip
                if (lstLatestUpdates.Pagination.totalItems > lstLatestUpdates.Pagination.itemsPerPage)
                {
                    lstLatestUpdates.Pagination.itemsToSkip = lstLatestUpdates.Pagination.itemsPerPage * (pageNo - 1);
                }



                //Get top 'n' results and determine link structure
                //foreach (SearchResult srResult in isResults.Take(30))
                foreach (SearchResult srResult in isResults.Skip(lstLatestUpdates.Pagination.itemsToSkip).Take(lstLatestUpdates.Pagination.itemsPerPage))
                {
                    //Obtain message's node
                    ipMsg = umbracoHelper.TypedContent(Convert.ToInt32(srResult.Id));
                    if (ipMsg != null)
                    {
                        //Obtain date of message
                        msgDate = ipMsg.GetPropertyValue<DateTime>(Common.NodeProperties.publishDate);

                        //Create a new date for messages
                        if (msgDate != prevDate)
                        {
                            //Update current date
                            prevDate = msgDate;

                            //Create new instances for updates and add to list of all updates.
                            latestUpdate = new latestUpdates();
                            latestUpdate.datePublished = msgDate;
                            lstLatestUpdates.LstLatestUpdates.Add(latestUpdate);

                            //Reset the visionary class on every new date change.
                            visionary = new visionary();
                        }

                        //Obtain current visionary or webmaster
                        if (ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.DocumentTypeAlias == Common.docType.Visionary) != null)
                        {
                            if (visionary.id != ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.DocumentTypeAlias == Common.docType.Visionary).Id)
                            {
                                //Obtain visionary node
                                ipVisionary = ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.DocumentTypeAlias == Common.docType.Visionary);

                                //Create new visionary class and add to latest update class
                                visionary = new visionary();
                                visionary.id = ipVisionary.Id;
                                visionary.name = ipVisionary.Name;
                                visionary.url = ipVisionary.Url;
                                latestUpdate.lstVisionaries.Add(visionary);
                            }
                        }
                        else if (ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.DocumentTypeAlias == Common.docType.WebmasterMessageList) != null)
                        {
                            if (visionary.id != ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.DocumentTypeAlias == Common.docType.WebmasterMessageList).Id)
                            {
                                //Obtain visionary node
                                ipVisionary = ipMsg.AncestorsOrSelf().FirstOrDefault(x => x.DocumentTypeAlias == Common.docType.WebmasterMessageList);

                                //Create new visionary class and add to latest update class
                                visionary = new visionary();
                                visionary.id = ipVisionary.Id;
                                visionary.name = ipVisionary.Name;
                                visionary.url = ipVisionary.Url;
                                latestUpdate.lstVisionaries.Add(visionary);
                            }
                        }

                        //Create new message and add to existing visionary class.
                        message = new message();
                        message.id = ipMsg.Id;
                        message.title = ipMsg.Name;
                        message.url = ipMsg.Url;
                        visionary.lstMessages.Add(message);
                    }
                }
            }


            //}
            //catch (Exception ex)
            //{
            //    //StringBuilder sb = new StringBuilder();
            //    //sb.AppendLine(@"MessageController.cs : RenderLatestMessages()");
            //    //sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(lstLatestUpdates));
            //    //Common.SaveErrorMessage(ex, sb, typeof(MessageController));


            //    //ModelState.AddModelError("", "*An error occured while creating the latest message list.");
            //    //return CurrentUmbracoPage();
            //}


            //Return data to partialview
            return lstLatestUpdates;
        }
        #endregion
    }
}