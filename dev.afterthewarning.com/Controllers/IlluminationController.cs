using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Umbraco;
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
using System.Diagnostics;
using Newtonsoft.Json;

namespace Controllers
{
    public class IlluminationController : SurfaceController
    {
        //
        private static Random _random = new Random();


        #region "Renders"
        public ActionResult RenderInstructions()
        {
            return PartialView("~/Views/Partials/IlluminationStories/_illuminationStoryInstructions.cshtml");
        }
        public ActionResult RenderStory(IMember member)
        {
            //Instantiate variables
            illuminationStory illuminationStory = new Models.illuminationStory();


            try
            {
                //Obtain the illumination story
                Udi storyUdi = member.GetValue<Udi>(ContentModels.Member.GetModelPropertyType(x => x.IlluminationStory).PropertyTypeAlias);
                UmbracoHelper umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
                IPublishedContent ipIlluminationStory = umbracoHelper.TypedContent(storyUdi);

                //Obtain the content models
                illuminationStory.CmIpIlluminationStory = new ContentModels.IlluminationStory(ipIlluminationStory);
                illuminationStory.CmMember = new ContentModels.Member(illuminationStory.CmIpIlluminationStory.Member);

                //Add data to story model
                StringBuilder sbAuthor = new StringBuilder();
                sbAuthor.Append(illuminationStory.CmMember.FirstName);
                sbAuthor.Append("&nbsp;&nbsp;&nbsp;");
                sbAuthor.Append(illuminationStory.CmMember.LastName);
                sbAuthor.Append(".");
                illuminationStory.Author = sbAuthor.ToString();


                return PartialView("~/Views/Partials/IlluminationStories/_showIlluminationStory.cshtml", illuminationStory);
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"IlluminationController.cs : RenderStory()");
                sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(illuminationStory));
                sb.AppendLine("member:" + Newtonsoft.Json.JsonConvert.SerializeObject(member));
                Common.SaveErrorMessage(ex, sb, typeof(IlluminationController));

                ModelState.AddModelError("", "*An error occured while retrieving your story.");
                return PartialView("~/Views/Partials/IlluminationStories/_showIlluminationStory.cshtml", illuminationStory);
            }
        }
        public ActionResult RenderForm(IMember member, Boolean editMode = false)
        {
            //Instantiate variables
            illuminationStory illuminationStory = new Models.illuminationStory();


            try
            {
                //Use a loop to create an age array from 1 to 120
                illuminationStory.lstAge.Add("");
                int i = 1;
                while (i <= 120)
                {
                    illuminationStory.lstAge.Add(i.ToString());
                    i++;
                }

                //Get member as iPublished
                var memberShipHelper = new Umbraco.Web.Security.MembershipHelper(UmbracoContext.Current);
                var ipMember = memberShipHelper.GetById(member.Id);

                //Add data to story model
                StringBuilder sbAuthor = new StringBuilder();
                sbAuthor.Append(ipMember.GetPropertyValue<String>(ContentModels.Member.GetModelPropertyType(x => x.FirstName).PropertyTypeAlias));
                sbAuthor.Append("&nbsp;&nbsp;&nbsp;");
                sbAuthor.Append(ipMember.GetPropertyValue<String>(ContentModels.Member.GetModelPropertyType(x => x.LastName).PropertyTypeAlias));
                sbAuthor.Append(".");
                illuminationStory.Author = sbAuthor.ToString();

                illuminationStory.memberId = member.Id;

                //Determine which we need to show an edit view or not.
                if (editMode)
                {
                    //Obtain the story from the member
                    Udi storyUdi = member.GetValue<Udi>(ContentModels.Member.GetModelPropertyType(x => x.IlluminationStory).PropertyTypeAlias);
                    UmbracoHelper umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
                    IPublishedContent ipIlluminationStory = umbracoHelper.TypedContent(storyUdi);

                    //Extract data from story & member
                    illuminationStory.storyId = ipIlluminationStory.Id;
                    illuminationStory.Title = ipIlluminationStory.GetPropertyValue<string>(Common.NodeProperties.title);
                    illuminationStory.Story = ipIlluminationStory.GetPropertyValue<string>(Common.NodeProperties.story);
                    if (member.GetValue<string>(Common.NodeProperties.age) != null)
                    {
                        illuminationStory.Age = member.GetValue<int>(Common.NodeProperties.age);
                    }

                    //Set the experience type
                    string experienceType = ipIlluminationStory.GetPropertyValue<string>(Common.NodeProperties.experienceType);
                    illuminationStory.ExperienceType = (Common.ExperienceTypes)System.Enum.Parse(typeof(Common.ExperienceTypes), experienceType.Replace(" ", ""));

                    //Set the gender
                    string gender = member.GetValue<string>(Common.NodeProperties.gender);
                    switch (Common.GetPreValueString(gender))
                    {
                        case "XX [Female]":
                            illuminationStory.Gender = Common.Genders.Female;
                            break;
                        case "XY [Male]":
                            illuminationStory.Gender = Common.Genders.Male;
                            break;
                    }

                    //Set the race
                    string race = member.GetValue<string>(Common.NodeProperties.race);
                    switch (Common.GetPreValueString(race))
                    {
                        case "American Indian or Native American":
                            illuminationStory.Race = Common.Races.AmericanIndianOrNativeAmerican;
                            break;
                        case "Arabic":
                            illuminationStory.Race = Common.Races.Arabic;
                            break;
                        case "Asian":
                            illuminationStory.Race = Common.Races.Asian;
                            break;
                        case "Black or African":
                            illuminationStory.Race = Common.Races.BlackOrAfrican;
                            break;
                        case "Indian":
                            illuminationStory.Race = Common.Races.Indian;
                            break;
                        case "Jewish":
                            illuminationStory.Race = Common.Races.Jewish;
                            break;
                        case "Latin or Hispanic":
                            illuminationStory.Race = Common.Races.LatinOrHispanic;
                            break;
                        case "Other or Keep Private":
                            illuminationStory.Race = Common.Races.OtherOrKeepPrivate;
                            break;
                        case "Pacific Islander":
                            illuminationStory.Race = Common.Races.PacificIslander;
                            break;
                        case "White or Caucasian":
                            illuminationStory.Race = Common.Races.WhiteOrCaucasian;
                            break;
                    }

                    //Set the religion
                    string religion = member.GetValue<string>(Common.NodeProperties.religion);
                    switch (Common.GetPreValueString(religion))
                    {
                        case "Agnostic":
                            illuminationStory.Religion = Common.Religions.Agnostic;
                            break;
                        case "Atheist":
                            illuminationStory.Religion = Common.Religions.Atheist;
                            break;
                        case "Baptist":
                            illuminationStory.Religion = Common.Religions.Baptist;
                            break;
                        case "Buddhist":
                            illuminationStory.Religion = Common.Religions.Buddhist;
                            break;
                        case "Catholic":
                            illuminationStory.Religion = Common.Religions.Catholic;
                            break;
                        case "Evangelical":
                            illuminationStory.Religion = Common.Religions.Evangelical;
                            break;
                        case "Hindu":
                            illuminationStory.Religion = Common.Religions.Hindu;
                            break;
                        case "Lutheran":
                            illuminationStory.Religion = Common.Religions.Lutheran;
                            break;
                        case "Muslim":
                            illuminationStory.Religion = Common.Religions.Muslim;
                            break;
                        case "Other Christian":
                            illuminationStory.Religion = Common.Religions.OtherChristian;
                            break;
                        case "Other or Keep Private":
                            illuminationStory.Religion = Common.Religions.OtherOrKeepPrivate;
                            break;
                        case "Protestant":
                            illuminationStory.Religion = Common.Religions.Protestant;
                            break;
                        case "Satinism":
                            illuminationStory.Religion = Common.Religions.Satinism;
                            break;
                        case "Wiccan or New Age":
                            illuminationStory.Religion = Common.Religions.WiccanOrNewAge;
                            break;
                    }

                    //Set the experience type
                    string country = member.GetValue<string>(Common.NodeProperties.country);
                    if (country != null)
                    {
                        illuminationStory.Country = (Common.Countries)System.Enum.Parse(typeof(Common.Countries), country.Replace(" ", "").Replace(".", ""));
                    }

                    //Add data to partial view
                    return PartialView("~/Views/Partials/IlluminationStories/_editIlluminationStory.cshtml", illuminationStory);
                }
                else
                {
                    //Add data to partial view
                    return PartialView("~/Views/Partials/IlluminationStories/_addIlluminationStory.cshtml", illuminationStory);
                }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"IlluminationController.cs : RenderForm()");
                sb.AppendLine("editMode:" + editMode.ToString());
                sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(illuminationStory));
                sb.AppendLine("member:" + Newtonsoft.Json.JsonConvert.SerializeObject(member));
                Common.SaveErrorMessage(ex, sb, typeof(IlluminationController));


                ModelState.AddModelError("", "*An error occured while creating a form to submit your story.");
                return PartialView("~/Views/Partials/IlluminationStories/_addIlluminationStory.cshtml", illuminationStory);
            }
        }
        public ActionResult RenderList()
        {
            //Instantiate variables
            var illuminationStoryList = new Models.IlluminationStoryList();
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
            var memberShipHelper = new Umbraco.Web.Security.MembershipHelper(UmbracoContext.Current);

            try
            {
                //Determine viewBy settings
                if (!string.IsNullOrEmpty(Request.QueryString[Common.ViewByTypes.ViewBy]))
                {
                    illuminationStoryList.ViewBy = Request.QueryString[Common.ViewByTypes.ViewBy];
                }


                //Get all items
                BaseSearchProvider mySearcher = ExamineManager.Instance.SearchProviderCollection[Common.searchProviders.IlluminationStoriesSearcher];
                ISearchCriteria criteria = mySearcher.CreateSearchCriteria(IndexTypes.Content);
                IBooleanOperation query = criteria.Field(Common.NodeProperties.umbracoNaviHide, "0"); //gets all items when this exists for every record.


                //Obtain result with query
                ISearchResults isResults = mySearcher.Search(query.Compile());


                //Get total experiences.
                foreach (SearchResult sRecord in isResults)
                {
                    switch (sRecord.Fields[Common.NodeProperties.experienceType])
                    {
                        case Common.ExperienceType.Heavenly:
                            illuminationStoryList.HeavenlyExperienceCount += 1;
                            break;
                        case Common.ExperienceType.Hellish:
                            illuminationStoryList.HellishExperienceCount += 1;
                            break;
                        case Common.ExperienceType.Purgatorial:
                            illuminationStoryList.PurgatorialExperienceCount += 1;
                            break;
                        default:
                            illuminationStoryList.OtherExperienceCount += 1;
                            break;
                    }
                }


                //Rebuild results
                if (!string.IsNullOrEmpty(illuminationStoryList.ViewBy))
                {
                    //Restart query
                    query = criteria.Field(Common.NodeProperties.umbracoNaviHide, "0"); //gets all items when this exists for every record.


                    //Add any viewBy parameters
                    switch (illuminationStoryList.ViewBy)
                    {
                        case Common.ViewByTypes.Heavenly:
                            query.And().Field(Common.NodeProperties.experienceType, Common.ExperienceType.Heavenly);
                            break;
                        case Common.ViewByTypes.Hellish:
                            query.And().Field(Common.NodeProperties.experienceType, Common.ExperienceType.Hellish);
                            break;
                        case Common.ViewByTypes.Purgatorial:
                            query.And().Field(Common.NodeProperties.experienceType, Common.ExperienceType.Purgatorial);
                            break;
                        case Common.ViewByTypes.OtherUnsure:
                            query.And().Field(Common.NodeProperties.experienceType, Common.ExperienceType.Other);
                            break;
                        default:
                            break;
                    }


                    //Obtain updated result with query
                    isResults = mySearcher.Search(query.Compile());
                }



                //Get item counts and total experiences.
                illuminationStoryList.Pagination.itemsPerPage = 50;
                illuminationStoryList.Pagination.totalItems = isResults.Count();


                //Determine how many pages/items to skip and take, as well as the total page count for the search result.
                if (illuminationStoryList.Pagination.totalItems > illuminationStoryList.Pagination.itemsPerPage)
                {
                    illuminationStoryList.Pagination.totalPages = (int)Math.Ceiling((double)illuminationStoryList.Pagination.totalItems / (double)illuminationStoryList.Pagination.itemsPerPage);
                }
                else
                {
                    illuminationStoryList.Pagination.itemsPerPage = illuminationStoryList.Pagination.totalItems;
                    illuminationStoryList.Pagination.totalPages = 1;
                }


                //Determine current page number 
                var pageNo = 1;
                if (!string.IsNullOrEmpty(Request.QueryString[Common.miscellaneous.PageNo]))
                {
                    int.TryParse(Request.QueryString[Common.miscellaneous.PageNo], out pageNo);
                    if (pageNo <= 0 || pageNo > illuminationStoryList.Pagination.totalPages)
                    {
                        pageNo = 1;
                    }
                }
                illuminationStoryList.Pagination.pageNo = pageNo;


                //Determine how many pages/items to skip
                if (illuminationStoryList.Pagination.totalItems > illuminationStoryList.Pagination.itemsPerPage)
                {
                    illuminationStoryList.Pagination.itemsToSkip = illuminationStoryList.Pagination.itemsPerPage * (pageNo - 1);
                }


                //Convert list of SearchResults to list of classes
                foreach (SearchResult sRecord in isResults.Skip(illuminationStoryList.Pagination.itemsToSkip).Take(illuminationStoryList.Pagination.itemsPerPage))
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

                    illuminationStoryList.lstStoryLink.Add(storyLink);
                }

            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"IlluminationController.cs : RenderList()");
                sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(illuminationStoryList));
                Common.SaveErrorMessage(ex, sb, typeof(IlluminationController));


                ModelState.AddModelError("", "*An error occured while creating a list of illumination stories.");
                return CurrentUmbracoPage();
            }


            //Return data to partialview
            return PartialView("~/Views/Partials/IlluminationStories/_illuminationStoryList.cshtml", illuminationStoryList);
        }
        #endregion


        #region "Posts"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddIlluminationStory(Models.illuminationStory model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Instantiate variables
                    IContentService contentService = Services.ContentService;
                    IContent icIllumStory = contentService.CreateContent(name: model.Title.Substring(0, 1).ToUpper() + model.Title.Substring(1).ToLower(), parentId: (int)Common.siteNode.IlluminationStories, contentTypeAlias: Common.docType.IlluminationStory);
                    IMember member = Services.MemberService.GetById(model.memberId);

                    icIllumStory.SetValue(ContentModels.IlluminationStory.GetModelPropertyType(x => x.Title).PropertyTypeAlias, model.Title.Substring(0, 1).ToUpper() + model.Title.Substring(1).ToLower());
                    icIllumStory.SetValue(ContentModels.IlluminationStory.GetModelPropertyType(x => x.Story).PropertyTypeAlias, model.Story);
                    icIllumStory.SetValue(ContentModels.IlluminationStory.GetModelPropertyType(x => x.Member).PropertyTypeAlias, model.memberId);
                    //icIllumStory.SetValue(
                    //    ContentModels.IlluminationStory.GetModelPropertyType(x => x.ExperienceType).PropertyTypeAlias,
                    //    Common.GetPrevalueIdForIContent(icIllumStory, ContentModels.IlluminationStory.GetModelPropertyType(x => x.ExperienceType).PropertyTypeAlias, model.ExperienceType.Value.ToString()));

                    string experienceType = model.ExperienceType.Value.ToString();
                    var enumExperienceType = (Common.ExperienceTypes)System.Enum.Parse(typeof(Common.ExperienceTypes), experienceType);
                    if ((Common.ExperienceTypes)System.Enum.Parse(typeof(Common.ExperienceTypes), experienceType) == Common.ExperienceTypes.OtherorUnsure) { experienceType = "Other or Unsure"; }
                    icIllumStory.SetValue(
                        ContentModels.IlluminationStory.GetModelPropertyType(x => x.ExperienceType).PropertyTypeAlias,
                        Common.GetPrevalueIdForIContent(icIllumStory, ContentModels.IlluminationStory.GetModelPropertyType(x => x.ExperienceType).PropertyTypeAlias, experienceType));


                    //Save new Illumination story
                    var result = contentService.SaveAndPublishWithStatus(icIllumStory);


                    if (result.Success)
                    {
                        //Add demographics to member's records
                        if (member == null)
                        {
                            //Save error message to umbraco
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(@"Controllers/IlluminationController.cs : AddIlluminationStory()");
                            sb.AppendLine(@"Member Id returned nothing.  Cannot add illumination story to member.");
                            sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                            Exception ex = new Exception();
                            Common.SaveErrorMessage(ex, sb, typeof(IlluminationController));
                        }
                        else
                        {
                            //Add data to member and save
                            member.SetValue(Common.NodeProperties.age, model.lstAge.FirstOrDefault());
                            member.SetValue(Common.NodeProperties.illuminationStory, new GuidUdi("document", icIllumStory.Key).ToString());

                            if (model.Country == null)
                            {
                                member.SetValue(Common.NodeProperties.country, "");
                            }
                            else
                            {
                                member.SetValue(Common.NodeProperties.country, model.Country.GetType().GetMember(model.Country.ToString()).First().GetCustomAttribute<DisplayAttribute>().GetName());
                            }

                            if (model.Gender == null)
                            {
                                member.SetValue(Common.NodeProperties.gender, "");
                            }
                            else
                            {
                                member.SetValue(
                                Common.NodeProperties.gender,
                                Common.GetPrevalueIdForIMember(member, Common.NodeProperties.gender, model.Gender.GetType().GetMember(model.Gender.ToString()).First().GetCustomAttribute<DisplayAttribute>().GetName()));
                            }

                            if (model.Religion == null)
                            {
                                member.SetValue(Common.NodeProperties.religion, "");
                            }
                            else
                            {
                                member.SetValue(
                                Common.NodeProperties.religion,
                                Common.GetPrevalueIdForIMember(member, Common.NodeProperties.religion, model.Religion.GetType().GetMember(model.Religion.ToString()).First().GetCustomAttribute<DisplayAttribute>().GetName()));
                            }

                            if (model.Race == null)
                            {
                                member.SetValue(Common.NodeProperties.race, "");
                            }
                            else
                            {
                                member.SetValue(
                                Common.NodeProperties.race,
                                Common.GetPrevalueIdForIMember(member, Common.NodeProperties.race, model.Race.GetType().GetMember(model.Race.ToString()).First().GetCustomAttribute<DisplayAttribute>().GetName()));
                            }


                            //Save all changes
                            Services.MemberService.Save(member);
                        }

                        //Return to page
                        TempData["IlluminationStoryAddedSuccessfully"] = true;
                        return RedirectToUmbracoPage((int)(Models.Common.siteNode.AddEditIlluminationStory));
                    }
                    else
                    {
                        //Save error message to umbraco
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(@"Controllers/IlluminationController.cs : AddIlluminationStory()");
                        sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                        sb.AppendLine("result:" + Newtonsoft.Json.JsonConvert.SerializeObject(result));
                        Exception ex = new Exception();
                        Common.SaveErrorMessage(ex, sb, typeof(IlluminationController));

                        //Return with error
                        ModelState.AddModelError(string.Empty, "An error occured while submitting your Illumination story.");
                        TempData["showAddEditPnl"] = true;
                        return CurrentUmbracoPage();
                    }
                }
                else
                {
                    TempData["showAddEditPnl"] = true;
                    return CurrentUmbracoPage();
                }

            }
            catch (Exception ex)
            {
                //Save error message to umbraco
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"IlluminationController.cs : AddIlluminationStory()");
                sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                Common.SaveErrorMessage(ex, sb, typeof(IlluminationController));

                ModelState.AddModelError(string.Empty, "An error occured while adding your story.");
                return CurrentUmbracoPage();
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateIlluminationStory(Models.illuminationStory model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Instantiate variables
                    IContentService contentService = Services.ContentService;
                    IContent icIllumStory = contentService.GetById(model.storyId);
                    IMember member = Services.MemberService.GetById(model.memberId);

                    //Update data
                    icIllumStory.SetValue(ContentModels.IlluminationStory.GetModelPropertyType(x => x.Title).PropertyTypeAlias, model.Title.Substring(0, 1).ToUpper() + model.Title.Substring(1).ToLower());
                    icIllumStory.SetValue(ContentModels.IlluminationStory.GetModelPropertyType(x => x.Story).PropertyTypeAlias, model.Story);
                    icIllumStory.SetValue(ContentModels.IlluminationStory.GetModelPropertyType(x => x.Member).PropertyTypeAlias, model.memberId);

                    string experienceType = model.ExperienceType.Value.ToString();
                    var enumExperienceType = (Common.ExperienceTypes)System.Enum.Parse(typeof(Common.ExperienceTypes), experienceType);
                    if ((Common.ExperienceTypes)System.Enum.Parse(typeof(Common.ExperienceTypes), experienceType) == Common.ExperienceTypes.OtherorUnsure) { experienceType = "Other or Unsure"; }
                    icIllumStory.SetValue(
                        ContentModels.IlluminationStory.GetModelPropertyType(x => x.ExperienceType).PropertyTypeAlias,
                        Common.GetPrevalueIdForIContent(icIllumStory, ContentModels.IlluminationStory.GetModelPropertyType(x => x.ExperienceType).PropertyTypeAlias, experienceType));


                    //Save new Illumination story
                    var result = contentService.SaveAndPublishWithStatus(icIllumStory);


                    if (result.Success)
                    {
                        //Add demographics to member's records
                        if (member == null)
                        {
                            //Save error message to umbraco
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(@"Controllers/IlluminationController.cs : UpdateIlluminationStory()");
                            sb.AppendLine(@"Member Id returned nothing.  Cannot update illumination story to member.");
                            sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                            Exception ex = new Exception();
                            Common.SaveErrorMessage(ex, sb, typeof(IlluminationController));
                        }
                        else
                        {
                            //Add data to member and save
                            member.SetValue(Common.NodeProperties.age, model.Age);

                            if (model.Country == null)
                            {
                                member.SetValue(Common.NodeProperties.country, "");
                            }
                            else
                            {
                                member.SetValue(Common.NodeProperties.country, model.Country.GetType().GetMember(model.Country.ToString()).First().GetCustomAttribute<DisplayAttribute>().GetName());
                            }

                            if (model.Gender == null)
                            {
                                member.SetValue(Common.NodeProperties.gender, "");
                            }
                            else
                            {
                                member.SetValue(
                                Common.NodeProperties.gender,
                                Common.GetPrevalueIdForIMember(member, Common.NodeProperties.gender, model.Gender.GetType().GetMember(model.Gender.ToString()).First().GetCustomAttribute<DisplayAttribute>().GetName()));
                            }

                            if (model.Religion == null)
                            {
                                member.SetValue(Common.NodeProperties.religion, "");
                            }
                            else
                            {
                                member.SetValue(
                                Common.NodeProperties.religion,
                                Common.GetPrevalueIdForIMember(member, Common.NodeProperties.religion, model.Religion.GetType().GetMember(model.Religion.ToString()).First().GetCustomAttribute<DisplayAttribute>().GetName()));
                            }

                            if (model.Race == null)
                            {
                                member.SetValue(Common.NodeProperties.race, "");
                            }
                            else
                            {
                                member.SetValue(
                                Common.NodeProperties.race,
                                Common.GetPrevalueIdForIMember(member, Common.NodeProperties.race, model.Race.GetType().GetMember(model.Race.ToString()).First().GetCustomAttribute<DisplayAttribute>().GetName()));
                            }

                            //Save all changes
                            Services.MemberService.Save(member);
                        }

                        //Return to page
                        TempData["IlluminationStoryUpdatedSuccessfully"] = true;
                        return RedirectToUmbracoPage((int)(Models.Common.siteNode.AddEditIlluminationStory));
                    }
                    else
                    {
                        //Save error message to umbraco
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(@"Controllers/IlluminationController.cs : UpdateIlluminationStory()");
                        sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                        sb.AppendLine("result:" + Newtonsoft.Json.JsonConvert.SerializeObject(result));
                        Exception ex = new Exception();
                        Common.SaveErrorMessage(ex, sb, typeof(IlluminationController));

                        //Return with error
                        ModelState.AddModelError(string.Empty, "An error occured while updating your Illumination story.");
                        TempData["showAddEditPnl"] = true;
                        return CurrentUmbracoPage();
                    }
                }
                else
                {
                    TempData["showAddEditPnl"] = true;
                    return CurrentUmbracoPage();
                }
            }
            catch (Exception ex)
            {
                //Save error message to umbraco
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"IlluminationController.cs : UpdateIlluminationStory()");
                sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                Common.SaveErrorMessage(ex, sb, typeof(IlluminationController));

                ModelState.AddModelError(string.Empty, "An error occured while updating your story.");
                return CurrentUmbracoPage();
            }
        }
        #endregion




        #region "Methods"
        public static Models.StatsByAge ObtainStatistics_byAge(IPublishedContent ipByAge)
        {
            //Initialize variables.
            Models.StatsByAge statsByAge = new Models.StatsByAge();
            statsByAge.LstChartData = new List<ChartDataset>();
            statsByAge.LstAgeRange = new List<string>();
            ChartDataset HeavenlyDataset = new ChartDataset("Heavenly", "Heavenly", "#4f81bc");
            ChartDataset HellishDataset = new ChartDataset("Hellish", "Hellish", "#bf4b49");
            ChartDataset PurgatorialDataset = new ChartDataset("Purgatorial", "Purgatorial", "#9bbb57");
            ChartDataset UnknownDataset = new ChartDataset("Unknown", "Unknown/Unsure", "#21bea6");
            statsByAge.TotalEntries = 0;

            //Obtain data from node
            statsByAge.LstAgeStats_Heavenly = JsonConvert.DeserializeObject<List<Stat_AgeRange>>(ipByAge.GetPropertyValue<string>(Common.NodeProperties.statsHeavenly));
            statsByAge.LstAgeStats_Hellish = JsonConvert.DeserializeObject<List<Stat_AgeRange>>(ipByAge.GetPropertyValue<string>(Common.NodeProperties.statsHellish));
            statsByAge.LstAgeStats_Purgatorial = JsonConvert.DeserializeObject<List<Stat_AgeRange>>(ipByAge.GetPropertyValue<string>(Common.NodeProperties.statsPurgatorial));
            statsByAge.LstAgeStats_Unknown = JsonConvert.DeserializeObject<List<Stat_AgeRange>>(ipByAge.GetPropertyValue<string>(Common.NodeProperties.statsUnknown));

            //Add data to proper datasets
            foreach (Stat_AgeRange stat in statsByAge.LstAgeStats_Heavenly)
            {
                HeavenlyDataset.LstData.Add(stat.Count); //Add data to list
                statsByAge.TotalEntries += stat.Count; //Increment total entries

                statsByAge.LstAgeRange.Add(stat.AgeRange); //Add text range [all are the same so only need to do this once.]
            }
            foreach (Stat_AgeRange stat in statsByAge.LstAgeStats_Hellish)
            {
                HellishDataset.LstData.Add(stat.Count);
                statsByAge.TotalEntries += stat.Count;
            }
            foreach (Stat_AgeRange stat in statsByAge.LstAgeStats_Purgatorial)
            {
                PurgatorialDataset.LstData.Add(stat.Count);
                statsByAge.TotalEntries += stat.Count;
            }
            foreach (Stat_AgeRange stat in statsByAge.LstAgeStats_Unknown)
            {
                UnknownDataset.LstData.Add(stat.Count);
                statsByAge.TotalEntries += stat.Count;
            }

            //Stringify lists
            statsByAge.jsonAgeRange = Newtonsoft.Json.JsonConvert.SerializeObject(statsByAge.LstAgeRange);
            HeavenlyDataset.JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(HeavenlyDataset.LstData);
            HellishDataset.JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(HellishDataset.LstData);
            PurgatorialDataset.JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(PurgatorialDataset.LstData);
            UnknownDataset.JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(UnknownDataset.LstData);

            //Add Charts to list.
            statsByAge.LstChartData.Add(HeavenlyDataset);
            statsByAge.LstChartData.Add(HellishDataset);
            statsByAge.LstChartData.Add(PurgatorialDataset);
            statsByAge.LstChartData.Add(UnknownDataset);

            //Return stats
            return statsByAge;
        }

        public static StatsByExperienceTypes ObtainStatistics_byExperienceType(IPublishedContent ip)
        {
            List<string> lstLabels = new List<string>();
            lstLabels.Add("Heavenly");
            lstLabels.Add("Purgatorial");
            lstLabels.Add("Hellish");
            lstLabels.Add("Unknown/Unsure");

            List<int> lstValues = new List<int>();
            lstValues.Add(25);
            lstValues.Add(15);
            lstValues.Add(35);
            lstValues.Add(25);

            List<string> lstBgColors = new List<string>();
            lstBgColors.Add("#4f81bc");
            lstBgColors.Add("#9bbb57");
            lstBgColors.Add("#bf4b49");
            lstBgColors.Add("#21bea6");

            List<string> lstHoverBgColors = new List<string>();
            lstHoverBgColors.Add("#8CACD3");
            lstHoverBgColors.Add("#BDD391");
            lstHoverBgColors.Add("#D58987");
            lstHoverBgColors.Add("#6DD4C4");


            string jsonLabels = Newtonsoft.Json.JsonConvert.SerializeObject(lstLabels);
            string jsonValues = Newtonsoft.Json.JsonConvert.SerializeObject(lstValues);
            string jsonBgColors = Newtonsoft.Json.JsonConvert.SerializeObject(lstBgColors);
            string jsonHoverBgColors = Newtonsoft.Json.JsonConvert.SerializeObject(lstHoverBgColors);


            StatsByExperienceTypes stats = new StatsByExperienceTypes();
            stats.BgColors = jsonBgColors;
            stats.HoverBgColors = jsonHoverBgColors;
            stats.Labels = jsonLabels;
            stats.Values = jsonValues;


            //Return stats
            return stats;
        }
        #endregion



        #region "Test Stories"
        public static string GenerateTestStories()
        {
            //Instantiate variables
            Stopwatch sw = new Stopwatch();
            sw.Start();
            TempResults results = new TempResults();

            try
            {
                int entryCount = 15000;
                _memberships membership = new _memberships();
                IMemberService memberService = ApplicationContext.Current.Services.MemberService;

                Array lstCountries = Enum.GetValues(typeof(Common.Countries));
                Array lstGenders = Enum.GetValues(typeof(Common.Genders));
                Array lstRaces = Enum.GetValues(typeof(Common.Races));
                Array lstReligions = Enum.GetValues(typeof(Common.Religions));
                Array lstExperienceTypes = Enum.GetValues(typeof(Common.ExperienceTypes));

                List<MembershipModel> lstMembers = new List<MembershipModel>();
                List<illuminationStory> lstIlluminationStories = new List<illuminationStory>();


                //Create story array
                List<string> lstStory = new List<string>();
                lstStory.Add("Acid effects antimagic bonus cold domain cure spell fire subtype glamer subschool melee move action strength domain suffocation.");
                lstStory.Add("Ability damage character dying energy damage inherent bonus portal domain ranged attack roll scrying subschool speed suffocation water dangers. Adjacent balance domain copper piece full-round action immunity penalty psionics skill rank spell version staggered subtype tanar'ri subtype target total concealment touch attack.");
                lstStory.Add("Aberration type calling subschool coup de grace domain spell entangled ethereal plane fear cone improved grab initiative count light weapon living luck domain melee mentalism domain monstrous humanoid type morale bonus orc domain paladin petrified ranged attack rounding scribe shield bonus spell level square subtype threaten tyranny domain.");
                lstStory.Add("Abjuration acid effects antimagic archon subtype change shape cowering damage reduction evil domain experience points extraplanar immunity lava effects law domain luck domain pinned player character size modifier spell resistance spell trigger item stable trade domain transmutation.");
                lstStory.Add("5-foot step arcane spell failure automatic hit base save bonus command undead conjuration enhancement bonus evasion extraplanar subtype fatigued force damage gold piece incorporeal mundane nauseated plant domain ranged attack roll rogue size modifier special quality spell preparation tanar'ri subtype threat threatened square turning damage water subtype.");
                lstStory.Add("Charm climb critical hit dazed disabled earth domain energy drain fear effect grapple check grappling hit points immediate action incorporeal subtype manufactured weapons medium natural reach paralyzed planning domain prerequisite profane bonus psionics reflex save stack strength summoning subschool swim tanar'ri subtype teleportation subschool treasure unarmed strike.");
                lstStory.Add("Abjuration action cantrip catching on fire character compulsion subschool darkness domain fate domain fine giant type knocked down modifier mundane negative level nonlethal damage paralyzed pounce spell descriptor spell resistance suffering domain summoning subschool undeath domain water dangers.");
                lstStory.Add("Animal domain attack attack roll base attack bonus charm domain checked critical roll cure spell death attack difficult terrain disease elf domain evocation falling fear aura force damage gnome domain good domain magic domain melee attack natural weapon one-handed weapon panicked party plant domain retribution domain shaken water domain.");
                lstStory.Add("Ability damaged angel subtype copper piece creation subschool creature type drow domain flight hardness natural reach negative energy pounce reaction rebuke undead rend subschool transmutation turning check.");
                lstStory.Add("Abjuration acid effects antimagic archon subtype change shape cowering damage reduction evil domain experience points extraplanar immunity lava effects law domain luck domain pinned player character size modifier spell resistance spell trigger item stable trade domain transmutation.");



                //Create all test members and stories
                for (int i = 0; i < entryCount; i++)
                {
                    //Create a test member
                    MembershipModel memberModel = new MembershipModel();
                    memberModel.FirstName = GetRandomLetter(i);
                    memberModel.LastName = GetRandomLetter(i + 1);
                    memberModel.Password = "Pa55word";
                    memberModel.Email = "JF_" + String.Format("{0:00000}", i) + "@noemail.com";
                    lstMembers.Add(memberModel);


                    //Create a test story
                    illuminationStory story = new illuminationStory();
                    story.Age = _random.Next(1, 121);
                    story.Country = (Common.Countries)lstCountries.GetValue(_random.Next(lstCountries.Length));
                    story.ExperienceType = (Common.ExperienceTypes)lstExperienceTypes.GetValue(_random.Next(lstExperienceTypes.Length));
                    story.Gender = (Common.Genders)lstGenders.GetValue(_random.Next(lstGenders.Length));
                    story.Race = (Common.Races)lstRaces.GetValue(_random.Next(lstRaces.Length));
                    story.Religion = (Common.Religions)lstReligions.GetValue(_random.Next(lstReligions.Length));
                    story.Author = memberModel.FirstName + " " + memberModel.LastName;
                    story.Title = "Illumination Sample " + String.Format("{0:00000}", i);
                    int length = _random.Next(5, 10);
                    StringBuilder sbStory = new StringBuilder();
                    for (int j = 0; j < length; j++)
                    {
                        sbStory.AppendLine(lstStory[j]);
                    }
                    story.Story = sbStory.ToString();
                    lstIlluminationStories.Add(story);
                }


                //Add each member and story 
                for (int k = 0; k < entryCount; k++)
                {
                    //Create membership
                    int memberId = membership.CreateMember(lstMembers[k].FirstName, lstMembers[k].LastName, lstMembers[k].Email, lstMembers[k].Password);
                    lstIlluminationStories[k].memberId = memberId;

                    membership.MakeAcctActive(memberId);

                    // Expose the custom properties for the member
                    IMember member = memberService.GetById(memberId);

                    //Add data to member and save
                    member.SetValue(Common.NodeProperties.age, lstIlluminationStories[k].Age);
                    member.SetValue(Common.NodeProperties.country, lstIlluminationStories[k].Country.GetType().GetMember(lstIlluminationStories[k].Country.ToString()).First().GetCustomAttribute<DisplayAttribute>().GetName());
                    member.SetValue(Common.NodeProperties.gender, Common.GetPrevalueIdForIMember(member, Common.NodeProperties.gender, lstIlluminationStories[k].Gender.GetType().GetMember(lstIlluminationStories[k].Gender.ToString()).First().GetCustomAttribute<DisplayAttribute>().GetName()));
                    member.SetValue(Common.NodeProperties.religion, Common.GetPrevalueIdForIMember(member, Common.NodeProperties.religion, lstIlluminationStories[k].Religion.GetType().GetMember(lstIlluminationStories[k].Religion.ToString()).First().GetCustomAttribute<DisplayAttribute>().GetName()));
                    member.SetValue(Common.NodeProperties.race, Common.GetPrevalueIdForIMember(member, Common.NodeProperties.race, lstIlluminationStories[k].Race.GetType().GetMember(lstIlluminationStories[k].Race.ToString()).First().GetCustomAttribute<DisplayAttribute>().GetName()));

                    //Save all changes
                    memberService.Save(member);


                    //Instantiate variables
                    IContentService contentService = ApplicationContext.Current.Services.ContentService;
                    IContent icIllumStory = contentService.CreateContent(name: lstIlluminationStories[k].Title.Substring(0, 1).ToUpper() + lstIlluminationStories[k].Title.Substring(1).ToLower(), parentId: (int)Common.siteNode.IlluminationStories, contentTypeAlias: Common.docType.IlluminationStory);

                    icIllumStory.SetValue(ContentModels.IlluminationStory.GetModelPropertyType(x => x.Title).PropertyTypeAlias, lstIlluminationStories[k].Title.Substring(0, 1).ToUpper() + lstIlluminationStories[k].Title.Substring(1).ToLower());
                    icIllumStory.SetValue(ContentModels.IlluminationStory.GetModelPropertyType(x => x.Story).PropertyTypeAlias, lstIlluminationStories[k].Story);
                    icIllumStory.SetValue(ContentModels.IlluminationStory.GetModelPropertyType(x => x.Member).PropertyTypeAlias, memberId);

                    string experienceType = lstIlluminationStories[k].ExperienceType.Value.ToString();
                    var enumExperienceType = (Common.ExperienceTypes)System.Enum.Parse(typeof(Common.ExperienceTypes), experienceType);
                    if ((Common.ExperienceTypes)System.Enum.Parse(typeof(Common.ExperienceTypes), experienceType) == Common.ExperienceTypes.OtherorUnsure) { experienceType = "Other or Unsure"; }
                    icIllumStory.SetValue(
                        ContentModels.IlluminationStory.GetModelPropertyType(x => x.ExperienceType).PropertyTypeAlias,
                        Common.GetPrevalueIdForIContent(icIllumStory, ContentModels.IlluminationStory.GetModelPropertyType(x => x.ExperienceType).PropertyTypeAlias, experienceType));


                    //Save new Illumination story
                    var result = contentService.SaveAndPublishWithStatus(icIllumStory);


                    if (result.Success)
                    {
                        //Save all changes
                        member.SetValue(Common.NodeProperties.illuminationStory, new GuidUdi("document", icIllumStory.Key).ToString());
                        memberService.Save(member);

                    }
                }


                results.memberCount = lstMembers.Count;
                results.avgAge = Convert.ToInt32(lstIlluminationStories.Average(x => x.Age));
                results.xx = lstIlluminationStories.Count(x => x.Gender.Value == Common.Genders.Male);
                results.xy = lstIlluminationStories.Count(x => x.Gender.Value == Common.Genders.Female);
                results.heavenly = lstIlluminationStories.Count(x => x.ExperienceType.Value == Common.ExperienceTypes.Heavenly);
                results.hellish = lstIlluminationStories.Count(x => x.ExperienceType.Value == Common.ExperienceTypes.Hellish);
                results.purgatorial = lstIlluminationStories.Count(x => x.ExperienceType.Value == Common.ExperienceTypes.Purgatorial);
                results.other = lstIlluminationStories.Count(x => x.ExperienceType.Value == Common.ExperienceTypes.OtherorUnsure);
                results.american = lstIlluminationStories.Count(x => x.Country.Value == Common.Countries.UnitedStates);
                results.catholic = lstIlluminationStories.Count(x => x.Religion.Value == Common.Religions.Catholic);
                results.satanism = lstIlluminationStories.Count(x => x.Religion.Value == Common.Religions.Satinism);
            }
            catch (Exception ex)
            {
                //results.resultMsg = ex.ToString();
                results.errorMsg = ex.ToString();
            }

            //Display time lapsed
            sw.Stop();
            results.elapsedTime = sw.Elapsed.TotalSeconds.ToString();


            return Newtonsoft.Json.JsonConvert.SerializeObject(results);
            //return results;
        }
        public static string GetRandomLetter(int seed)
        {
            // This method returns a random letter between 'A' and 'Z'.
            int num = _random.Next(0, 26); // Zero to 25
            char let = (char)('a' + num);
            return let.ToString().ToUpper();
        }

        //private void IncrementAgeLst(ref List<int> LstAge, int Age)
        //{
        //    //Ensure a max age
        //    if (Age > 100) Age = 100;

        //    //Determine the age index to increment
        //    int ageGrp = 5 * (int)Math.Round(Age / 5.0);
        //    int index = ageGrp / 5;

        //    LstAge[index] += 1;
        //}
        //private void GetAgeAvg(ref List<int> LstAge_Heavenly, ref List<int> LstAge_Hellish, ref List<int> LstAge_Purgatorial, ref List<int> LstAge_Unknown, int index)
        //{
        //    //
        //    int Heavenly = LstAge_Heavenly[index];
        //    int Hellish = LstAge_Hellish[index];
        //    int Purgatorial = LstAge_Purgatorial[index];
        //    int Unknown = LstAge_Unknown[index];

        //    int total = Heavenly + Hellish + Purgatorial + Unknown;

        //    LstAge_Heavenly[index] = (int)(((double)Heavenly / total) * 100);
        //    LstAge_Hellish[index] = (int)(((double)Hellish / total) * 100);
        //    LstAge_Purgatorial[index] = (int)(((double)Purgatorial / total) * 100);
        //    LstAge_Unknown[index] = (int)(((double)Unknown / total) * 100);
        //}

        #endregion
    }
}