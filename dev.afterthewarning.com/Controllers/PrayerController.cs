using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.PublishedContentModels;
using Umbraco.Web;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Web.Extensions;
using ContentModels = Umbraco.Web.PublishedContentModels;
using Examine;
using UmbracoExamine;
using Examine.Providers;
using Examine.SearchCriteria;
using static Models.Common;


namespace Controllers
{
    public class PrayerController : SurfaceController
    {
        //Render Form
        public ActionResult RenderForm_NewPrayer(string userName)
        {
            try
            {
                //Instantiate variables
                var prayerRequest = new PrayerRequestModel();
                var memberShipHelper = new Umbraco.Web.Security.MembershipHelper(UmbracoContext.Current);

                //Get member's ID from 
                prayerRequest.UserId = memberShipHelper.GetByUsername(userName).Id;

                return PartialView("~/Views/Partials/PrayerCorner/_newPrayer.cshtml", prayerRequest);
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"PrayerController.cs : RenderForm_NewPrayer()");
                sb.AppendLine("userName:" + userName);
                Common.saveErrorMessage(ex.ToString(), sb.ToString());


                ModelState.AddModelError("", "*An error occured while creating a new prayer form.");
                return PartialView("~/Views/Partials/PrayerCorner/_newPrayer.cshtml");
            }
        }
        public ActionResult RenderForm_EditPrayer(string nodeId)
        {
            try
            {
                //Obtain the prayer request as a model and pass to the view.
                PrayerRequest prayer = new PrayerRequest(Umbraco.TypedContent(nodeId));

                //Populate data into modell
                var prayerRequest = new PrayerRequestModel();
                prayerRequest.Prayer = prayer.Prayer;
                prayerRequest.PrayerTitle = prayer.PrayerTitle;
                prayerRequest.UserId = prayer.PrayerRequestMember.Id;
                prayerRequest.Id = Convert.ToInt32(nodeId);
                prayerRequest.totalPrayersOffered = prayer.TotalPrayersOffered;

                //Call edit page with model data
                return PartialView("~/Views/Partials/PrayerCorner/_editPrayer.cshtml", prayerRequest);
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"PrayerController.cs : RenderForm_EditPrayer()");
                sb.AppendLine("nodeId:" + nodeId);
                Common.saveErrorMessage(ex.ToString(), sb.ToString());


                ModelState.AddModelError("", "*An error occured while creating an edit form with your prayers.");
                return PartialView("~/Views/Partials/PrayerCorner/_editPrayer.cshtml");
            }

        }
        public ActionResult RenderForm_PrayerPledges(string userName)
        {
            //Intsantiate list
            List<PrayerRequestModel> lstPrayers = new List<PrayerRequestModel>();


            try
            {
                //
                IMember member = Services.MemberService.GetByUsername(userName);
                if (member != null)
                {
                    //Instantiate list
                    List<_prayerRequest> lstJsonPrayersOffered = new List<_prayerRequest>();

                    //Populate list with any existing data
                    if (member.HasProperty(Common.NodeProperties.prayersOfferedFor) && member.GetValue(Common.NodeProperties.prayersOfferedFor) != null)
                    {
                        lstJsonPrayersOffered = JsonConvert.DeserializeObject<List<_prayerRequest>>(member.GetValue(Common.NodeProperties.prayersOfferedFor).ToString());
                    }

                    //
                    foreach (_prayerRequest prayerOffered in lstJsonPrayersOffered)
                    {
                        PrayerRequestModel prayerRequest = new PrayerRequestModel();
                        IPublishedContent ipPrayer = Umbraco.TypedContent(prayerOffered.prayer);

                        prayerRequest.Id = ipPrayer.Id;
                        prayerRequest.date = Convert.ToDateTime(prayerOffered.date);
                        prayerRequest.PrayerTitle = ipPrayer.GetPropertyValue<String>(PrayerRequest.GetModelPropertyType(x => x.PrayerTitle).PropertyTypeAlias);
                        prayerRequest.Url = ipPrayer.Url;

                        lstPrayers.Add(prayerRequest);
                    }
                }

            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"PrayerController.cs : RenderForm_PrayerPledges()");
                sb.AppendLine("userName:" + userName);
                sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(lstPrayers));
                Common.saveErrorMessage(ex.ToString(), sb.ToString());


                ModelState.AddModelError("", "*An error occured while...");
                return CurrentUmbracoPage();
            }


            return PartialView("~/Views/Partials/PrayerCorner/_prayerPledges.cshtml", lstPrayers);
        }
        public ActionResult RenderList()
        {
            //Instantiate variables
            var prayerList = new Models.PrayerList();

            try
            {
                //
                var memberShipHelper = new Umbraco.Web.Security.MembershipHelper(UmbracoContext.Current);
                var CmPrayerList = new ContentModels.PrayerList(Umbraco.TypedContent((int)Common.siteNode.ThePrayerCorner));

                //Get all prayers
                BaseSearchProvider mySearcher = ExamineManager.Instance.SearchProviderCollection[Common.searchProviders.PrayersSearcher];
                ISearchCriteria criteria = mySearcher.CreateSearchCriteria(IndexTypes.Content);
                IBooleanOperation query = criteria.Field(Common.NodeProperties.indexType, Common.NodeProperties.content); //gets all items
                query.And().OrderByDescending(Common.NodeProperties.requestDate);
                query.And().OrderBy(Common.NodeProperties.prayerTitle);
                ISearchResults isResults = mySearcher.Search(query.Compile());


                //Get item counts and total experiences.
                prayerList.Pagination.itemsPerPage = 10;
                prayerList.Pagination.totalItems = isResults.Count();


                //Determine how many pages/items to skip and take, as well as the total page count for the search result.
                if (prayerList.Pagination.totalItems > prayerList.Pagination.itemsPerPage)
                {
                    prayerList.Pagination.totalPages = (int)Math.Ceiling((double)prayerList.Pagination.totalItems / (double)prayerList.Pagination.itemsPerPage);
                }
                else
                {
                    prayerList.Pagination.itemsPerPage = prayerList.Pagination.totalItems;
                    prayerList.Pagination.totalPages = 1;
                }


                //Determine current page number 
                var pageNo = 1;
                if (!string.IsNullOrEmpty(Request.QueryString[Common.miscellaneous.PageNo]))
                {
                    int.TryParse(Request.QueryString[Common.miscellaneous.PageNo], out pageNo);
                    if (pageNo <= 0 || pageNo > prayerList.Pagination.totalPages)
                    {
                        pageNo = 1;
                    }
                }
                prayerList.Pagination.pageNo = pageNo;


                //Determine how many pages/items to skip
                if (prayerList.Pagination.totalItems > prayerList.Pagination.itemsPerPage)
                {
                    prayerList.Pagination.itemsToSkip = prayerList.Pagination.itemsPerPage * (pageNo - 1);
                }


                //Convert list of SearchResults to list of classes
                foreach (SearchResult sRecord in isResults.Skip(prayerList.Pagination.itemsToSkip).Take(prayerList.Pagination.itemsPerPage))
                {
                    //Create new prayerLink class
                    var prayerLink = new Models.PrayerLink();
                    prayerLink.Id = sRecord.Id;
                    prayerLink.Title = sRecord.Fields[Common.NodeProperties.prayerTitle];
                    prayerLink.Url = Umbraco.NiceUrl(sRecord.Id);
                    prayerLink.Date = Convert.ToDateTime(sRecord.Fields[Common.NodeProperties.requestDate]);


                    //Determine current percentage
                    prayerLink.currentPercentage = int.Parse(sRecord.Fields[Common.NodeProperties.currentPercentage]);
                    prayerLink.baseCalculationDate = DateTime.Parse(sRecord.Fields[Common.NodeProperties.baseCalculationDate]);
                    int daysSinceBaseDate = (DateTime.Now - prayerLink.baseCalculationDate).Days;
                    if (daysSinceBaseDate > prayerLink.currentPercentage)
                    {
                        prayerLink.currentPercentage = 0;
                    }
                    else
                    {
                        prayerLink.currentPercentage = prayerLink.currentPercentage - daysSinceBaseDate;
                    }


                    //Determine proper candle based upon current percentage
                    prayerLink.CandleUrl = CmPrayerList.CandleOut.Url;
                    if (prayerLink.currentPercentage == 0)
                    {
                        prayerLink.CandleUrl = CmPrayerList.CandleOut.Url;
                    }
                    else if (prayerLink.currentPercentage >= 1 && prayerLink.currentPercentage <= 10)
                    {
                        prayerLink.CandleUrl = CmPrayerList.Candle10.Url;
                    }
                    else if (prayerLink.currentPercentage >= 11 && prayerLink.currentPercentage <= 20)
                    {
                        prayerLink.CandleUrl = CmPrayerList.Candle20.Url;
                    }
                    else if (prayerLink.currentPercentage >= 21 && prayerLink.currentPercentage <= 30)
                    {
                        prayerLink.CandleUrl = CmPrayerList.Candle30.Url;
                    }
                    else if (prayerLink.currentPercentage >= 31 && prayerLink.currentPercentage <= 40)
                    {
                        prayerLink.CandleUrl = CmPrayerList.Candle40.Url;
                    }
                    else if (prayerLink.currentPercentage >= 41 && prayerLink.currentPercentage <= 50)
                    {
                        prayerLink.CandleUrl = CmPrayerList.Candle50.Url;
                    }
                    else if (prayerLink.currentPercentage >= 51 && prayerLink.currentPercentage <= 60)
                    {
                        prayerLink.CandleUrl = CmPrayerList.Candle60.Url;
                    }
                    else if (prayerLink.currentPercentage >= 61 && prayerLink.currentPercentage <= 70)
                    {
                        prayerLink.CandleUrl = CmPrayerList.Candle70.Url;
                    }
                    else if (prayerLink.currentPercentage >= 71 && prayerLink.currentPercentage <= 80)
                    {
                        prayerLink.CandleUrl = CmPrayerList.Candle80.Url;
                    }
                    else if (prayerLink.currentPercentage >= 81 && prayerLink.currentPercentage <= 90)
                    {
                        prayerLink.CandleUrl = CmPrayerList.Candle90.Url;
                    }
                    else if (prayerLink.currentPercentage >= 91 && prayerLink.currentPercentage <= 100)
                    {
                        prayerLink.CandleUrl = CmPrayerList.Candle100.Url;
                    }


                    //
                    IEnumerable<string> prayerSummary = sRecord.Fields[Common.NodeProperties.prayer].Split().Take(32);
                    prayerLink.PrayerSummary = string.Join(" ", prayerSummary) + "...";


                    //Obtain member 
                    ContentModels.Member CmMember;
                    int memberId;
                    if (int.TryParse(sRecord.Fields[Common.NodeProperties.prayerRequestMember], out memberId))
                    {
                        IPublishedContent ipMember = memberShipHelper.GetById(memberId);
                        CmMember = new ContentModels.Member(ipMember);
                    }
                    else
                    {
                        CmMember = new ContentModels.Member(Udi.Parse(sRecord.Fields[Common.NodeProperties.prayerRequestMember]).ToPublishedContent());
                    }

                    StringBuilder sbAuthor = new StringBuilder();
                    sbAuthor.Append(CmMember.FirstName);
                    sbAuthor.Append("&nbsp;&nbsp;&nbsp;");
                    sbAuthor.Append(CmMember.LastName);
                    sbAuthor.Append(".");
                    prayerLink.MemberName = sbAuthor.ToString();


                    //
                    prayerList.lstPrayerLinks.Add(prayerLink);
                }

            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"PrayerController.cs : RenderList()");
                sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(prayerList));
                Common.saveErrorMessage(ex.ToString(), sb.ToString());


                ModelState.AddModelError("", "*An error occured while creating the prayer list.");
                return CurrentUmbracoPage();
            }


            //Return data to partialview
            return PartialView("~/Views/Partials/PrayerCorner/_prayerList.cshtml", prayerList);
        }




        //Submit form to create a new account
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitNewPrayerRequest(PrayerRequestModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Instantiate variables
                    IContentService contentService = Services.ContentService;
                    IContent content = contentService.CreateContent(name: model.PrayerTitle, parentId: (int)Common.siteNode.ThePrayerCorner, contentTypeAlias: docType.PrayerRequest);

                    content.SetValue(PrayerRequest.GetModelPropertyType(x => x.PrayerTitle).PropertyTypeAlias, model.PrayerTitle);
                    content.SetValue(PrayerRequest.GetModelPropertyType(x => x.Prayer).PropertyTypeAlias, model.Prayer);
                    content.SetValue(PrayerRequest.GetModelPropertyType(x => x.PrayerRequestMember).PropertyTypeAlias, model.UserId);
                    content.SetValue(PrayerRequest.GetModelPropertyType(x => x.RequestDate).PropertyTypeAlias, DateTime.Now);
                    content.SetValue(PrayerRequest.GetModelPropertyType(x => x.BaseCalculationDate).PropertyTypeAlias, DateTime.Now);

                    content.SetValue(PrayerRequest.GetModelPropertyType(x => x.TotalPrayersOffered).PropertyTypeAlias, 0);
                    content.SetValue(PrayerRequest.GetModelPropertyType(x => x.CurrentPercentage).PropertyTypeAlias, 0);

                    var result = contentService.SaveAndPublishWithStatus(content);
                    //contentService.RePublishAll(model.UserId);


                    if (result.Success)
                    {
                        //Add Prayer Request to member's records
                        IMember member = Services.MemberService.GetById(model.UserId);
                        if (member == null)
                        {
                            //Save error message to umbraco
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(@"Controllers/PrayerController.cs : SubmitNewPrayerRequest()");
                            sb.AppendLine(@"Member Id returned nothing.  Cannot add prayer request to member.");
                            sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                            Common.saveErrorMessage(sb.ToString(), sb.ToString());
                        }
                        else
                        {
                            //Instantiate list
                            List<Models._prayerRequest> lstPrayerRequests = new List<Models._prayerRequest>();

                            //Populate list with any existing data
                            if (member.HasProperty(NodeProperties.prayerRequests) && member.GetValue(NodeProperties.prayerRequests) != null)
                            {
                                lstPrayerRequests = JsonConvert.DeserializeObject<List<Models._prayerRequest>>(member.GetValue(NodeProperties.prayerRequests).ToString());
                            }

                            //Add latest prayer request to 
                            Models._prayerRequest newRequest = new Models._prayerRequest();
                            newRequest.prayer = content.Id.ToString();
                            newRequest.date = DateTime.Now.ToString("yyyy-MM-dd");
                            lstPrayerRequests.Add(newRequest);

                            //Add data to member and save
                            member.SetValue(NodeProperties.prayerRequests, JsonConvert.SerializeObject(lstPrayerRequests));
                            Services.MemberService.Save(member);
                        }

                        //Return to page
                        TempData["NewPrayerCreatedSuccessfully"] = true;
                        return RedirectToUmbracoPage((int)(Models.Common.siteNode.PrayerRequests));
                    }
                    else
                    {
                        //Save error message to umbraco
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(@"Controllers/PrayerController.cs : SubmitNewPrayerRequest()");
                        sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                        sb.AppendLine("result:" + Newtonsoft.Json.JsonConvert.SerializeObject(result));
                        Common.saveErrorMessage(sb.ToString(), sb.ToString());

                        //Return with error
                        ModelState.AddModelError(string.Empty, "An error occured while creating your prayer request.");
                        return CurrentUmbracoPage();
                    }
                }
                else
                {
                    return CurrentUmbracoPage();
                }
            }
            catch (Exception ex)
            {
                //Save error message to umbraco
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"PrayerController.cs : SubmitNewPrayerRequest()");
                sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                Common.saveErrorMessage(ex.ToString(), sb.ToString());

                ModelState.AddModelError(string.Empty, "An error occured while submitting your prayer request.");
                return CurrentUmbracoPage();
            }
        }



        //Submit form to create a new account
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePrayerRequest(PrayerRequestModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Instantiate variables
                    IContentService contentService = Services.ContentService;
                    IContent icPrayerRequest = contentService.GetById(model.Id);

                    //Update data
                    icPrayerRequest.SetValue(PrayerRequest.GetModelPropertyType(x => x.PrayerTitle).PropertyTypeAlias, model.PrayerTitle);
                    icPrayerRequest.SetValue(PrayerRequest.GetModelPropertyType(x => x.Prayer).PropertyTypeAlias, model.Prayer);
                    var result = contentService.SaveAndPublishWithStatus(icPrayerRequest);

                    if (result.Success)
                    {
                        //Return to page
                        TempData["PrayerUpdatedSuccessfully"] = true;
                        return RedirectToUmbracoPage((int)(Models.Common.siteNode.PrayerRequests));
                    }
                    else
                    {
                        //Save error message to umbraco
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(@"Controllers/PrayerController.cs : UpdatePrayerRequest()");
                        sb.AppendLine(@"Updating prayer request was not successful.");
                        sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                        sb.AppendLine("result:" + Newtonsoft.Json.JsonConvert.SerializeObject(result));
                        Common.saveErrorMessage(sb.ToString(), sb.ToString());

                        //Return with error
                        ModelState.AddModelError(string.Empty, "An error occured while updating your prayer request.");
                        return CurrentUmbracoPage();
                    }
                }
                else
                {
                    return CurrentUmbracoPage();
                }
            }
            catch (Exception ex)
            {
                //Save error message to umbraco
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"PrayerController.cs : UpdatePrayerRequest()");
                sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                Common.saveErrorMessage(ex.ToString(), sb.ToString());

                ModelState.AddModelError(string.Empty, "An error occured while updating your prayer request.");
                return CurrentUmbracoPage();
            }
        }



        //Submit a prayer pledge
        [HttpPost]
        public ActionResult PledgePrayers(String loginId, int prayerId)
        {
            try
            {
                //Instantiate variables
                IContentService contentService = Services.ContentService;
                IContent icPrayerRequest = contentService.GetById(prayerId);
                var CmPrayerList = new ContentModels.PrayerRequest(Umbraco.TypedContent(prayerId));


                //Update prayer request
                icPrayerRequest.SetValue(Common.NodeProperties.totalPrayersOffered, CmPrayerList.TotalPrayersOffered + 1);


                //Determine and update current percentage and base date
                int currentPercentage = CmPrayerList.CurrentPercentage;
                DateTime baseCalculationDate = CmPrayerList.BaseCalculationDate;
                int daysSinceBaseDate = (DateTime.Now - baseCalculationDate).Days;

                if (daysSinceBaseDate > currentPercentage)
                {
                    //Set new values for prayer request
                    currentPercentage = 2; //[given an extra day]
                    baseCalculationDate = DateTime.Today;
                }
                else
                {
                    //Recalculate current percentage
                    currentPercentage = (currentPercentage - daysSinceBaseDate) + 1;
                }
                icPrayerRequest.SetValue(Common.NodeProperties.currentPercentage, currentPercentage);
                icPrayerRequest.SetValue(Common.NodeProperties.baseCalculationDate, baseCalculationDate);


                //Save updates to prayer
                var result = contentService.SaveAndPublishWithStatus(icPrayerRequest);
                if (!result.Success)
                {
                    //Save error message to umbraco
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(@"Controllers/PrayerController.cs : PledgePrayers()");
                    sb.AppendLine("loginId: " + loginId);
                    sb.AppendLine("prayerId: " + prayerId);
                    sb.AppendLine("result: " + Newtonsoft.Json.JsonConvert.SerializeObject(result));
                    Common.saveErrorMessage(sb.ToString(), sb.ToString());

                    //Return with error
                    ModelState.AddModelError(string.Empty, "An error occured while submitting your pledge.");
                    return CurrentUmbracoPage();
                }

                //Update member requests
                IMember member = Services.MemberService.GetByEmail(loginId);
                if (member == null)
                {
                    //Save error message to umbraco
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(@"Controllers/PrayerController.cs : PledgePrayers()");
                    sb.AppendLine(@"Cannot add prayer pledge to member.");
                    sb.AppendLine("loginId: " + loginId);
                    sb.AppendLine("prayerId: " + prayerId);
                    Common.saveErrorMessage(sb.ToString(), sb.ToString());
                }
                else
                {
                    //Instantiate list
                    List<Models._prayerRequest> lstPrayerRequests = new List<Models._prayerRequest>();

                    //Populate list with any existing data
                    if (member.HasProperty(NodeProperties.prayersOfferedFor) && member.GetValue(NodeProperties.prayersOfferedFor) != null)
                    {
                        lstPrayerRequests = JsonConvert.DeserializeObject<List<Models._prayerRequest>>(member.GetValue(NodeProperties.prayersOfferedFor).ToString());
                    }

                    //Add latest prayer request to 
                    Models._prayerRequest newRequest = new Models._prayerRequest();
                    newRequest.prayer = prayerId.ToString();
                    newRequest.date = DateTime.Now.ToString("yyyy-MM-dd");
                    lstPrayerRequests.Add(newRequest);

                    //Add data to member and save
                    member.SetValue(NodeProperties.prayersOfferedFor, JsonConvert.SerializeObject(lstPrayerRequests));
                    Services.MemberService.Save(member);
                }

                //Return to page
                TempData["PrayerPledgedSuccessfully"] = true;
                return RedirectToCurrentUmbracoPage();
            }
            catch (Exception ex)
            {
                //Save error message to umbraco
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"PrayerController.cs : PledgePrayers()");
                Common.saveErrorMessage(ex.ToString(), sb.ToString());

                ModelState.AddModelError(string.Empty, "An error occured while submitting your prayer pledge.");
                return CurrentUmbracoPage();
            }
        }
    }
}