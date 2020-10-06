using formulate.app.Types;
using formulate.core.Models;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using ContentModels = Umbraco.Web.PublishedContentModels;


namespace Controllers
{
    public class GeneralController : SurfaceController
    {
        public static Models.TopLevelContent ObtainTopLevelData(IPublishedContent ipModel, string url)
        {
            //Instantiate variables
            Models.TopLevelContent PgContent = new TopLevelContent();

            //Show analytics code if not a dev site.
            if (url.Contains("dev.")) { PgContent.ShowAnalytics = false; }
            else if (url.Contains("staging.")) { PgContent.ShowAnalytics = false; }
            if (url.Contains("dev7.")) { PgContent.ShowAnalytics = false; }
            else if (url.Contains("staging7.")) { PgContent.ShowAnalytics = false; }

            //Obtain meta data from SeoChecker
            PgContent.Meta = ipModel.GetPropertyValue<SEOChecker.MVC.MetaData>("seoChecker");


            return PgContent;
        }
        public static Models.ContactUsContent ObtainContactUsContent(ContentModels.ContactUs cmModel, HtmlHelper Html)
        {
            //Instantiate variables
            Models.ContactUsContent PgContent = new ContactUsContent();

            //Obtain page summary
            PgContent.ContactSummary = Html.Raw(umbraco.library.ReplaceLineBreaks(cmModel.ContactSummary));

            //Obtain the form and its view model
            ConfiguredFormInfo pickedForm = cmModel.GetPropertyValue<ConfiguredFormInfo>("formPicker");
            PgContent.Vm = formulate.api.Rendering.GetFormViewModel(pickedForm.FormId, pickedForm.LayoutId, pickedForm.TemplateId, cmModel);


            return PgContent;
        }
        public static ManageAcctContent ObtainManageAcctContent(System.Security.Principal.IPrincipal User, UmbracoHelper Umbraco, IPublishedContent ipCurrentPg)
        {
            //Instantiate variables
            Models.ManageAcctContent PgContent = new ManageAcctContent();
            PgContent.Inactive = "inactive";

            if (!User.Identity.IsAuthenticated)
            {
                //Redirect to login page.
                PgContent.Redirect = true;
                PgContent.RedirectTo = Umbraco.TypedContent((int)(Models.Common.siteNode.Login)).Url;
            }
            else if (ipCurrentPg.DocumentTypeAlias == Common.docType.ManageAccount)
            {
                PgContent.Redirect = true;
                PgContent.RedirectTo = ipCurrentPg.Children.First().Url;
            }
            else
            {
                //Instantiate variables.
                IPublishedContent ipHome = Umbraco.TypedContent((int)(Common.siteNode.Home));
                PgContent.CredentialsUrl = Umbraco.TypedContent((int)(Models.Common.siteNode.EditAccount)).Url;
                PgContent.IlluminationStoryUrl = Umbraco.TypedContent((int)(Models.Common.siteNode.AddEditIlluminationStory)).Url;

                //Make fields active if Illumination has occured.
                if (ipHome.GetPropertyValue<Boolean>(Common.NodeProperties.activateIlluminationControls) == true) { PgContent.Inactive = string.Empty; }
            }

            PgContent.IsManageAcctPg = (ipCurrentPg.DocumentTypeAlias == Common.docType.ManageAccount);


            return PgContent;
        }
        public static MainNavContent ObtainMainNavContent(System.Security.Principal.IPrincipal User, UmbracoHelper Umbraco)
        {
            //Instantiate variables
            MainNavContent mainNavContent = new MainNavContent();

            //
            mainNavContent.ipHome = Umbraco.AssignedContentItem.Site();
            mainNavContent.isLoggedIn = User.Identity.IsAuthenticated;

            //Obtain the url to the search page
            mainNavContent.searchUrl = Umbraco.TypedContent((int)Common.siteNode.Search).Url;

            //Make fields active if Illumination has occured.
            mainNavContent.activateIlluminationControls = mainNavContent.ipHome.GetPropertyValue<Boolean>(Common.NodeProperties.activateIlluminationControls);

            //Get main navigation
            foreach (var ipLvl1 in mainNavContent.ipHome.Children.Where(x => x.IsVisible()))
            {
                if (!mainNavContent.activateIlluminationControls)
                { if (ipLvl1.DocumentTypeAlias == Common.docType.IlluminationStoryList) { continue; } }


                //Instantiate variable
                navigationLink lnkLvl1 = new navigationLink();

                //Add data to link record
                lnkLvl1.id = ipLvl1.Id;
                lnkLvl1.level = 1;
                lnkLvl1.name = ipLvl1.Name;
                lnkLvl1.url = ipLvl1.Url;

                //Add level 2 nav
                if (ipLvl1.GetPropertyValue<Boolean>(Common.NodeProperties.hideChildrenFromNavigation) == false)
                {
                    if (ipLvl1.Children.Any(x => x.IsVisible()))
                    {
                        foreach (var ipLvl2 in ipLvl1.Children.Where(x => x.IsVisible()))
                        {
                            //Instantiate variable
                            navigationLink lnkLvl2 = new navigationLink();

                            //Add data to link record
                            lnkLvl2.id = ipLvl2.Id;
                            lnkLvl2.level = 2;
                            lnkLvl2.name = ipLvl2.Name;
                            lnkLvl2.url = ipLvl2.Url;

                            //Add level 3 nav
                            if (ipLvl2.GetPropertyValue<Boolean>(Common.NodeProperties.hideChildrenFromNavigation) == false)
                            {
                                if (ipLvl2.Children.Any(x => x.IsVisible()))
                                {
                                    foreach (var ipLvl3 in ipLvl2.Children.Where(x => x.IsVisible()))
                                    {
                                        //Skip the following doctypes
                                        if (ipLvl3.DocumentTypeAlias == Common.docType.UDateFoldersyFolderYear) { continue; }
                                        //Instantiate variable
                                        navigationLink lnkLvl3 = new navigationLink();

                                        //Add data to link record
                                        lnkLvl3.id = ipLvl3.Id;
                                        lnkLvl3.level = 3;
                                        lnkLvl3.name = ipLvl3.Name;
                                        lnkLvl3.url = ipLvl3.Url;

                                        //Add records to list
                                        lnkLvl2.lstChildLinks.Add(lnkLvl3);
                                    }
                                }
                            }

                            //Add records to list
                            lnkLvl1.lstChildLinks.Add(lnkLvl2);
                        }
                    }
                }

                //Add records to list
                mainNavContent.lstMainNavlinks.Add(lnkLvl1);
            }

            //Get minor navigation
            foreach (var ipLvl1 in mainNavContent.ipHome.Children.Where(x => x.GetPropertyValue<bool>(Common.NodeProperties.showInMinorNavigation) == true))
            {
                //Skip the following if the following exists.
                if (mainNavContent.isLoggedIn)
                {
                    if (ipLvl1.DocumentTypeAlias == Common.docType.Login) { continue; }
                    if (ipLvl1.DocumentTypeAlias == Common.docType.CreateAccount) { continue; }
                }
                else
                {
                    if (ipLvl1.DocumentTypeAlias == Common.docType.Logout) { continue; }
                    if (ipLvl1.DocumentTypeAlias == Common.docType.ManageAccount) { continue; }
                }


                //Instantiate variable
                navigationLink lnkLvl1 = new navigationLink();

                //Add data to link record
                lnkLvl1.id = ipLvl1.Id;
                lnkLvl1.level = 1;
                lnkLvl1.name = ipLvl1.Name;
                lnkLvl1.url = ipLvl1.Url;

                //Add level 2 nav
                if (ipLvl1.GetPropertyValue<Boolean>(Common.NodeProperties.hideChildrenFromNavigation) == false)
                {
                    foreach (var ipLvl2 in ipLvl1.Children.Where(x => x.GetPropertyValue<bool>(Common.NodeProperties.showInMinorNavigation) == true))
                    {

                        if (!mainNavContent.activateIlluminationControls)
                        {
                            if (ipLvl2.DocumentTypeAlias == Common.docType.AddEditIlluminationStory) { continue; }
                        }

                        //Instantiate variable
                        navigationLink lnkLvl2 = new navigationLink();

                        //Add data to link record
                        lnkLvl2.id = ipLvl2.Id;
                        lnkLvl2.level = 2;
                        lnkLvl2.name = ipLvl2.Name;
                        lnkLvl2.url = ipLvl2.Url;

                        //Add level 3 nav
                        if (ipLvl2.GetPropertyValue<Boolean>(Common.NodeProperties.hideChildrenFromNavigation) == false)
                        {
                            foreach (var ipLvl3 in ipLvl2.Children.Where(x => x.GetPropertyValue<bool>(Common.NodeProperties.showInMinorNavigation) == true))
                            {
                                //Instantiate variable
                                navigationLink lnkLvl3 = new navigationLink();

                                //Add data to link record
                                lnkLvl3.id = ipLvl3.Id;
                                lnkLvl3.level = 3;
                                lnkLvl3.name = ipLvl3.Name;
                                lnkLvl3.url = ipLvl3.Url;

                                //Add records to list
                                lnkLvl2.lstChildLinks.Add(lnkLvl3);
                            }
                        }

                        //Add records to list
                        lnkLvl1.lstChildLinks.Add(lnkLvl2);
                    }
                }

                //Add records to list
                mainNavContent.lstMinorNavlinks.Add(lnkLvl1);
            }


            return mainNavContent;
        }
        public static PaginationContent ObtainPaginationContent(Models.Pagination Model, string url)
        {
            //
            PaginationContent paginationContent = new PaginationContent();
            int index = 1;

            //build base url for navigation links
            paginationContent.baseUri = new Uri(url);

            // this gets all the query string key value pairs as a collection
            paginationContent.queryString = HttpUtility.ParseQueryString(paginationContent.baseUri.Query);

            // this removes the key if exists
            paginationContent.queryString.Remove("pageNo");
            paginationContent.queryString.Add("pageNo", "");

            // this gets the page path from root without QueryString
            paginationContent.baseUrl = paginationContent.baseUri.GetLeftPart(UriPartial.Path);
            paginationContent.baseUrl = paginationContent.queryString.Count > 0 ? String.Format("{0}?{1}", paginationContent.baseUrl, paginationContent.queryString) : paginationContent.baseUrl;

            //Set values for page indexes
            paginationContent.previous = Model.pageNo - 1;
            paginationContent.next = Model.pageNo + 1;

            //Determine if prev/next values are correct.
            if (paginationContent.previous < 1) { paginationContent.previous = 1; }
            if (paginationContent.next > Model.totalPages) { paginationContent.next = Model.totalPages; }


            return paginationContent;
        }
        public static PersonalAccountsContent ObtainPersonalAcctContent(UmbracoHelper Umbraco)
        {
            //Instantiate variables
            PersonalAccountsContent personalAccountsContent = new PersonalAccountsContent();

            //
            IPublishedContent ipHome = Umbraco.TypedContent((int)(Common.siteNode.Home));
            IPublishedContent ipImg = Umbraco.TypedMedia(ipHome.GetPropertyValue<int>(Common.NodeProperties.personalPhoto));

            personalAccountsContent.imgUrl = ipImg.GetCropUrl(Common.crop.Square_500x500);
            personalAccountsContent.lstArticles = ipHome.GetPropertyValue<List<IPublishedContent>>(Common.NodeProperties.personalArticles);

            return personalAccountsContent;
        }
        public static SupportPnlContent ObtainSupportPnlContent(UmbracoHelper Umbraco)
        {
            //Instantiate variables
            SupportPnlContent supportPnlContent = new SupportPnlContent();

            IPublishedContent currentNode = UmbracoContext.Current.PublishedContentRequest.PublishedContent;
            supportPnlContent.supportUsUrl = "";
            supportPnlContent.showPnl = (currentNode.Id == (int)Common.siteNode.Home);

            //Only retrieve data if the panel can be shown
            if (supportPnlContent.showPnl)
            {
                //Get url to donation page
                supportPnlContent.supportUsUrl = Umbraco.TypedContent((int)Common.siteNode.Donate).Url;
            }

            return supportPnlContent;
        }
    }
}
