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
using Examine.LuceneEngine.SearchCriteria;

namespace Controllers
{
    public class SearchController : SurfaceController
    {
        //Render Search Form
        public ActionResult RenderForm()
        {
            //Instantiate variables
            var searchList = new Models.SearchList();


            try
            {
                //Obtain search parameters
                if (!string.IsNullOrEmpty(Request.QueryString[Common.miscellaneous.SearchIn]))
                {
                    searchList.SearchIn = Request.QueryString[Common.miscellaneous.SearchIn];
                }
                if (!string.IsNullOrEmpty(Request.QueryString[Common.miscellaneous.SearchFor]))
                {
                    searchList.SearchFor = Request.QueryString[Common.miscellaneous.SearchFor];
                }


                if (!String.IsNullOrEmpty(searchList.SearchIn))
                {
                    //Set results section visible
                    searchList.ShowResults = true;


                    //Determine current page number 
                    var pageNo = 1;
                    if (!string.IsNullOrEmpty(Request.QueryString[Common.miscellaneous.PageNo]))
                    {
                        int.TryParse(Request.QueryString[Common.miscellaneous.PageNo], out pageNo);
                    }


                    //Determine what examine to search in
                    switch (searchList.SearchIn)
                    {
                        case Common.SearchIn.Articles:
                            ObtainByArticle(searchList, pageNo);
                            break;
                        case Common.SearchIn.Bible:
                            ObtainByScripture(searchList, pageNo);
                            break;
                        case Common.SearchIn.Illuminations:
                            ObtainByIlluminationStories(searchList, pageNo);
                            break;
                        case Common.SearchIn.Messages:
                            ObtainByMessagesFromHeaven(searchList, pageNo);
                            break;
                        case Common.SearchIn.Prayers:
                            ObtainByPrayerCorner(searchList, pageNo);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"SearchController.cs : RenderForm()");
                sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(searchList));
                Common.saveErrorMessage(ex.ToString(), sb.ToString());

                ModelState.AddModelError("", "*An error occured while performing your search request.");
            }

            return PartialView("~/Views/Partials/Search/_searchList.cshtml", searchList);
        }


        //Search by
        private void ObtainByIlluminationStories(Models.SearchList searchList, int pageNo)
        {
            //Instantiate variables
            var memberShipHelper = new Umbraco.Web.Security.MembershipHelper(UmbracoContext.Current);
            searchList.ShowIlluminationStories = true;
            searchList.SearchInTitle = "Illumination Stories";


            if (!string.IsNullOrWhiteSpace(searchList.SearchFor))
            {
                //Set up search criteria
                BaseSearchProvider mySearcher = ExamineManager.Instance.SearchProviderCollection[Common.searchProviders.IlluminationStoriesSearcher];
                ISearchCriteria criteria = mySearcher.CreateSearchCriteria(BooleanOperation.Or);


                //Setup up search fields by importance
                IBooleanOperation query = criteria.Field(Common.NodeProperties.title, searchList.SearchFor.MultipleCharacterWildcard());
                query.Or().Field(Common.NodeProperties.story, searchList.SearchFor.MultipleCharacterWildcard());
                query.Or().Field(Common.NodeProperties.experienceType, searchList.SearchFor.MultipleCharacterWildcard());
                query.Or().Field(Common.NodeProperties.member, searchList.SearchFor.MultipleCharacterWildcard());
                //IBooleanOperation query = criteria.Field(Common.NodeProperties.title, searchList.SearchFor.Boost(2));
                //query.Or().Field(Common.NodeProperties.story, searchList.SearchFor.Boost(1));
                //query.Or().Field(Common.NodeProperties.experienceType, searchList.SearchFor);
                //query.Or().Field(Common.NodeProperties.member, searchList.SearchFor);


                //Obtain result with query
                ISearchResults searchResults = mySearcher.Search(query.Compile());


                //Get item counts and total experiences.
                searchList.Pagination.totalItems = searchResults.Count();


                //Determine how many pages/items to skip and take, as well as the total page count for the search result.
                if (searchList.Pagination.totalItems > searchList.Pagination.itemsPerPage)
                {
                    searchList.Pagination.totalPages = (int)Math.Ceiling((double)searchList.Pagination.totalItems / (double)searchList.Pagination.itemsPerPage);
                }
                else
                {
                    searchList.Pagination.itemsPerPage = searchList.Pagination.totalItems;
                    searchList.Pagination.totalPages = 1;
                }


                //Determine current page number
                if (pageNo <= 0 || pageNo > searchList.Pagination.totalPages)
                {
                    pageNo = 1;
                }
                searchList.Pagination.pageNo = pageNo;


                //Determine how many pages/items to skip
                if (searchList.Pagination.totalItems > searchList.Pagination.itemsPerPage)
                {
                    searchList.Pagination.itemsToSkip = searchList.Pagination.itemsPerPage * (pageNo - 1);
                }


                //Convert list of SearchResults to list of classes
                foreach (SearchResult sRecord in searchResults.Skip(searchList.Pagination.itemsToSkip).Take(searchList.Pagination.itemsPerPage))
                {
                    var storyLink = new Models.illuminationStoryLink();
                    storyLink.experienceType = sRecord.Fields[Common.NodeProperties.experienceType];
                    storyLink.id = sRecord.Id;
                    storyLink.title = sRecord.Fields[Common.NodeProperties.title];
                    storyLink.url = Umbraco.NiceUrl(sRecord.Id);


                    //Obtain member 
                    ContentModels.Member CmMember;
                    int memberId;
                    if (int.TryParse(sRecord.Fields[Common.NodeProperties.member], out memberId))
                    {
                        IPublishedContent ipMember = memberShipHelper.GetById(memberId);
                        CmMember = new ContentModels.Member(ipMember);
                    }
                    else
                    {
                        CmMember = new ContentModels.Member(Udi.Parse(sRecord.Fields[Common.NodeProperties.member]).ToPublishedContent());
                    }
                    //var CmMember = new ContentModels.Member(Udi.Parse(sRecord.Fields[Common.NodeProperties.member]).ToPublishedContent());
                    

                    StringBuilder sbAuthor = new StringBuilder();
                    sbAuthor.Append(CmMember.FirstName);
                    sbAuthor.Append("&nbsp;&nbsp;&nbsp;");
                    sbAuthor.Append(CmMember.LastName);
                    sbAuthor.Append(".");
                    storyLink.memberName = sbAuthor.ToString();
                    storyLink.memberId = CmMember.Id;

                    searchList.lstStoryLink.Add(storyLink);
                }
            }
        }
        private void ObtainByMessagesFromHeaven(Models.SearchList searchList, int pageNo)
        {
            //Instantiate variables
            searchList.ShowMsgsFromHeaven = true;
            searchList.SearchInTitle = "Messages from Heaven";


            if (!string.IsNullOrWhiteSpace(searchList.SearchFor))
            {
                //Get all prayers
                BaseSearchProvider mySearcher = ExamineManager.Instance.SearchProviderCollection[Common.searchProviders.MessagesSearcher];
                ISearchCriteria criteria = mySearcher.CreateSearchCriteria(BooleanOperation.Or);


                //Setup up search fields by importance
                IBooleanOperation query = criteria.Field(Common.NodeProperties.nodeName, searchList.SearchFor.MultipleCharacterWildcard());
                query.Or().Field(Common.NodeProperties.subtitle, searchList.SearchFor.MultipleCharacterWildcard());
                //IBooleanOperation query = criteria.Field(Common.NodeProperties.nodeName, searchList.SearchFor.Boost(1));
                //query.Or().Field(Common.NodeProperties.subtitle, searchList.SearchFor);


                //Obtain result with query
                ISearchResults searchResults = mySearcher.Search(query.Compile());


                //Get item counts and total experiences.
                searchList.Pagination.totalItems = searchResults.Count();


                //Determine how many pages/items to skip and take, as well as the total page count for the search result.
                if (searchList.Pagination.totalItems > searchList.Pagination.itemsPerPage)
                {
                    searchList.Pagination.totalPages = (int)Math.Ceiling((double)searchList.Pagination.totalItems / (double)searchList.Pagination.itemsPerPage);
                }
                else
                {
                    searchList.Pagination.itemsPerPage = searchList.Pagination.totalItems;
                    searchList.Pagination.totalPages = 1;
                }


                //Determine current page number 
                if (pageNo <= 0 || pageNo > searchList.Pagination.totalPages)
                {
                    pageNo = 1;
                }
                searchList.Pagination.pageNo = pageNo;


                //Determine how many pages/items to skip
                if (searchList.Pagination.totalItems > searchList.Pagination.itemsPerPage)
                {
                    searchList.Pagination.itemsToSkip = searchList.Pagination.itemsPerPage * (pageNo - 1);
                }


                //Convert list of SearchResults to list of classes
                foreach (SearchResult sRecord in searchResults.Skip(searchList.Pagination.itemsToSkip).Take(searchList.Pagination.itemsPerPage))
                {
                    var msgLink = new Models.MsgLink();
                    msgLink.Id = sRecord.Id;
                    msgLink.Title = sRecord.Fields[Common.NodeProperties.nodeName];
                    msgLink.Subtitle = sRecord.Fields[Common.NodeProperties.subtitle];
                    msgLink.Url = Umbraco.NiceUrl(sRecord.Id);
                    msgLink.Date = Convert.ToDateTime(sRecord.Fields[Common.NodeProperties.publishDate]);

                    searchList.lstMsgsFromHeavenLinks.Add(msgLink);
                }
            }

        }
        private void ObtainByPrayerCorner(Models.SearchList searchList, int pageNo)
        {
            //Instantiate variables
            var memberShipHelper = new Umbraco.Web.Security.MembershipHelper(UmbracoContext.Current);
            searchList.ShowPrayers = true;
            searchList.SearchInTitle = "The Prayer Corner";


            if (!string.IsNullOrWhiteSpace(searchList.SearchFor))
            {
                //
                var CmPrayerList = new ContentModels.PrayerList(Umbraco.TypedContent((int)Common.siteNode.ThePrayerCorner));

                //Get all prayers
                BaseSearchProvider mySearcher = ExamineManager.Instance.SearchProviderCollection[Common.searchProviders.PrayersSearcher];
                ISearchCriteria criteria = mySearcher.CreateSearchCriteria(BooleanOperation.Or);


                //Setup up search fields by importance
                IBooleanOperation query = criteria.Field(Common.NodeProperties.prayerTitle, searchList.SearchFor.MultipleCharacterWildcard());
                query.Or().Field(Common.NodeProperties.prayer, searchList.SearchFor.MultipleCharacterWildcard());
                query.Or().Field(Common.NodeProperties.prayerRequestMember, searchList.SearchFor.MultipleCharacterWildcard());
                //IBooleanOperation query = criteria.Field(Common.NodeProperties.prayerTitle, searchList.SearchFor.Boost(2));
                //query.Or().Field(Common.NodeProperties.prayer, searchList.SearchFor.Boost(1));
                //query.Or().Field(Common.NodeProperties.prayerRequestMember, searchList.SearchFor);


                //Obtain result with query
                ISearchResults searchResults = mySearcher.Search(query.Compile());


                //Get total experiences.
                searchList.Pagination.itemsPerPage = 10;
                searchList.Pagination.totalItems = searchResults.Count();


                //Determine how many pages/items to skip and take, as well as the total page count for the search result.
                if (searchList.Pagination.totalItems > searchList.Pagination.itemsPerPage)
                {
                    searchList.Pagination.totalPages = (int)Math.Ceiling((double)searchList.Pagination.totalItems / (double)searchList.Pagination.itemsPerPage);
                }
                else
                {
                    searchList.Pagination.itemsPerPage = searchList.Pagination.totalItems;
                    searchList.Pagination.totalPages = 1;
                }


                //Determine current page number 
                if (pageNo <= 0 || pageNo > searchList.Pagination.totalPages)
                {
                    pageNo = 1;
                }
                searchList.Pagination.pageNo = pageNo;


                //Determine how many pages/items to skip
                if (searchList.Pagination.totalItems > searchList.Pagination.itemsPerPage)
                {
                    searchList.Pagination.itemsToSkip = searchList.Pagination.itemsPerPage * (pageNo - 1);
                }


                //Convert list of SearchResults to list of classes
                foreach (SearchResult sRecord in searchResults.Skip(searchList.Pagination.itemsToSkip).Take(searchList.Pagination.itemsPerPage))
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
                    searchList.lstPrayerLinks.Add(prayerLink);








                    //prayerLink.currentPercentage = int.Parse(sRecord.Fields[Common.NodeProperties.currentPercentage]);
                    //IEnumerable<string> prayerSummary = sRecord.Fields[Common.NodeProperties.prayer].Split().Take(32);
                    //prayerLink.PrayerSummary = string.Join(" ", prayerSummary) + "...";


                    ////Obtain member 
                    //ContentModels.Member CmMember;
                    //int memberId;
                    //if (int.TryParse(sRecord.Fields[Common.NodeProperties.prayerRequestMember], out memberId))
                    //{
                    //    IPublishedContent ipMember = memberShipHelper.GetById(memberId);
                    //    CmMember = new ContentModels.Member(ipMember);
                    //}
                    //else
                    //{
                    //    CmMember = new ContentModels.Member(Udi.Parse(sRecord.Fields[Common.NodeProperties.prayerRequestMember]).ToPublishedContent());
                    //}


                    //StringBuilder sbAuthor = new StringBuilder();
                    //sbAuthor.Append(CmMember.FirstName);
                    //sbAuthor.Append("&nbsp;&nbsp;&nbsp;");
                    //sbAuthor.Append(CmMember.LastName);
                    //sbAuthor.Append(".");
                    //prayerLink.MemberName = sbAuthor.ToString();

                    //searchList.lstPrayerLinks.Add(prayerLink);
                }
            }
        }
        private void ObtainByArticle(Models.SearchList searchList, int pageNo)
        {
            //Instantiate variables
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
            searchList.ShowArticles = true;
            searchList.SearchInTitle = "All Articles";


            if (!string.IsNullOrWhiteSpace(searchList.SearchFor))
            {
                //Get all prayers
                BaseSearchProvider mySearcher = ExamineManager.Instance.SearchProviderCollection[Common.searchProviders.ArticleSearcher];
                ISearchCriteria criteria = mySearcher.CreateSearchCriteria(BooleanOperation.Or);


                //Setup up search fields by importance
                IBooleanOperation query = criteria.Field(Common.NodeProperties.title, searchList.SearchFor.MultipleCharacterWildcard());
                query.Or().Field(Common.NodeProperties.subtitle, searchList.SearchFor.MultipleCharacterWildcard());
                query.Or().Field(Common.NodeProperties.content, searchList.SearchFor.MultipleCharacterWildcard());
                query.Or().Field(Common.NodeProperties.originalSource, searchList.SearchFor.MultipleCharacterWildcard());
                //IBooleanOperation query = criteria.Field(Common.NodeProperties.title, searchList.SearchFor.Boost(3));
                //query.Or().Field(Common.NodeProperties.subtitle, searchList.SearchFor.Boost(2));
                //query.Or().Field(Common.NodeProperties.content, searchList.SearchFor.Boost(1));
                //query.Or().Field(Common.NodeProperties.originalSource, searchList.SearchFor);


                //Obtain result with query
                ISearchResults searchResults = mySearcher.Search(query.Compile());


                //Get item counts and total experiences.
                searchList.Pagination.totalItems = searchResults.Count();


                //Determine how many pages/items to skip and take, as well as the total page count for the search result.
                if (searchList.Pagination.totalItems > searchList.Pagination.itemsPerPage)
                {
                    searchList.Pagination.totalPages = (int)Math.Ceiling((double)searchList.Pagination.totalItems / (double)searchList.Pagination.itemsPerPage);
                }
                else
                {
                    searchList.Pagination.itemsPerPage = searchList.Pagination.totalItems;
                    searchList.Pagination.totalPages = 1;
                }


                //Determine current page number 
                if (pageNo <= 0 || pageNo > searchList.Pagination.totalPages)
                {
                    pageNo = 1;
                }
                searchList.Pagination.pageNo = pageNo;


                //Determine how many pages/items to skip
                if (searchList.Pagination.totalItems > searchList.Pagination.itemsPerPage)
                {
                    searchList.Pagination.itemsToSkip = searchList.Pagination.itemsPerPage * (pageNo - 1);
                }


                //Convert list of SearchResults to list of classes
                foreach (SearchResult sRecord in searchResults.Skip(searchList.Pagination.itemsToSkip).Take(searchList.Pagination.itemsPerPage))
                {
                    var msgLink = new Models.ArticleLink();
                    IPublishedContent ipArticle = umbracoHelper.TypedContent(sRecord.Id);
                    msgLink.Id = ipArticle.Id;
                    msgLink.Url = ipArticle.Url;
                    msgLink.Breadcrumb = GetBreadcrumbForArticle(ipArticle);

                    searchList.lstArticleLinks.Add(msgLink);
                }
            }

        }
        private void ObtainByScripture(Models.SearchList searchList, int pageNo)
        {
            //Instantiate variables
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
            searchList.ShowBible = true;
            searchList.SearchInTitle = "The Scriptures";


            if (!string.IsNullOrWhiteSpace(searchList.SearchFor))
            {
                //Get all prayers
                BaseSearchProvider mySearcher = ExamineManager.Instance.SearchProviderCollection[Common.searchProviders.ScriptureSearcher];
                ISearchCriteria criteria = mySearcher.CreateSearchCriteria(IndexTypes.Content);
                ISearchResults searchResults = mySearcher.Search(searchList.SearchFor, true);


                //Get item counts and total experiences.
                searchList.Pagination.itemsPerPage = 30;
                searchList.Pagination.totalItems = searchResults.Count();


                //Determine how many pages/items to skip and take, as well as the total page count for the search result.
                if (searchList.Pagination.totalItems > searchList.Pagination.itemsPerPage)
                {
                    searchList.Pagination.totalPages = (int)Math.Ceiling((double)searchList.Pagination.totalItems / (double)searchList.Pagination.itemsPerPage);
                }
                else
                {
                    searchList.Pagination.itemsPerPage = searchList.Pagination.totalItems;
                    searchList.Pagination.totalPages = 1;
                }


                //Determine current page number 
                if (pageNo <= 0 || pageNo > searchList.Pagination.totalPages)
                {
                    pageNo = 1;
                }
                searchList.Pagination.pageNo = pageNo;


                //Determine how many pages/items to skip
                if (searchList.Pagination.totalItems > searchList.Pagination.itemsPerPage)
                {
                    searchList.Pagination.itemsToSkip = searchList.Pagination.itemsPerPage * (pageNo - 1);
                }


                //Convert list of SearchResults to list of classes
                foreach (SearchResult sRecord in searchResults.Skip(searchList.Pagination.itemsToSkip).Take(searchList.Pagination.itemsPerPage))
                {
                    var scriptureLink = new Models.ScriptureLink();
                    IPublishedContent ipArticle = umbracoHelper.TypedContent(sRecord.Id);
                    scriptureLink.Id = ipArticle.Id;
                    scriptureLink.Url = ipArticle.Parent.Url + "?chapter=" + ipArticle.Name;
                    scriptureLink.Breadcrumb = GetBreadcrumbForScripture(ipArticle);

                    searchList.lstBibleLinks.Add(scriptureLink);
                }
            }

        }


        //Methods
        private string GetBreadcrumbForArticle(IPublishedContent ip)
        {
            //Obtain the breadcrumb ancestors of the current page.
            var lstBreadcrumbs = ip.Ancestors();
            //bool isFirst = true;
            StringBuilder sb = new StringBuilder();


            if (lstBreadcrumbs.Count() > 1)
            {
                foreach (IPublishedContent ipCrumb in lstBreadcrumbs.OrderBy(x => x.Level).ToList())
                {
                    if (ipCrumb.DocumentTypeAlias != Common.docType.Home)
                    {
                        //
                        sb.Append("<span class='divider'> » </span>");

                        //Add crumb navigation to screen
                        if (ipCrumb.HasProperty(Common.NodeProperties.title))
                        {
                            sb.Append("<span class='breadcrumb'>" + ipCrumb.GetPropertyValue<string>(Common.NodeProperties.title) + "</span>");
                        }
                        else
                        {
                            sb.Append("<span class='breadcrumb'>" + ipCrumb.Name + "</span>");
                        }
                    }
                }
            }


            //
            sb.Append("<span class='divider'> » </span>");

            //Add crumb navigation to screen
            if (ip.HasProperty(Common.NodeProperties.title))
            {
                sb.Append("<span class='breadcrumb'>" + ip.GetPropertyValue<string>(Common.NodeProperties.title) + "</span>");
            }
            else
            {
                sb.Append("<span class='breadcrumb'>" + ip.Name + "</span>");
            }


            return sb.ToString();
        }
        private string GetBreadcrumbForScripture(IPublishedContent ip)
        {
            //Obtain the breadcrumb ancestors of the current page.
            var lstBreadcrumbs = ip.Ancestors();
            //bool isFirst = true;
            StringBuilder sb = new StringBuilder();


            if (lstBreadcrumbs.Count() > 1)
            {
                foreach (IPublishedContent ipCrumb in lstBreadcrumbs.OrderBy(x => x.Level).ToList())
                {
                    if (ipCrumb.DocumentTypeAlias == Common.docType.Scripture)
                    {
                        //
                        sb.Append("<span class='divider'> » </span>");

                        //Add crumb navigation to screen
                        if (ipCrumb.HasProperty(Common.NodeProperties.title))
                        {
                            sb.Append("<span class='breadcrumb'>" + ipCrumb.GetPropertyValue<string>(Common.NodeProperties.title) + "</span>");
                        }
                        else
                        {
                            sb.Append("<span class='breadcrumb'>" + ipCrumb.Name + "</span>");
                        }
                    }
                }
            }


            //
            sb.Append("<span class='divider'> » </span>");

            //Add crumb navigation to screen
            if (ip.HasProperty(Common.NodeProperties.title))
            {
                sb.Append("<span class='breadcrumb'>Chapter " + ip.GetPropertyValue<string>(Common.NodeProperties.title) + "</span>");
            }
            else
            {
                sb.Append("<span class='breadcrumb'>Chapter " + ip.Name + "</span>");
            }


            return sb.ToString();
        }
    }
}