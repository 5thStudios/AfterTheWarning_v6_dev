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
using Examine;
using UmbracoExamine;
using Examine.Providers;
using Examine.SearchCriteria;
using Newtonsoft.Json;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.Rendering;
using formulate.app.Types;


namespace Controllers
{
    public class IlluminationController : SurfaceController
    {
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
                        case "Native American":
                            illuminationStory.Race = Common.Races.NativeAmerican;
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

                        //Update all statistical data
                        UpdateAllStatistics();

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


                    //Save  Illumination story
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

                        //Update all statistical data
                        UpdateAllStatistics();

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
        public static Models.IlluminationStoryContent ObtainIlluminationStoryContent(UmbracoHelper umbracoHelper, HtmlHelper Html, ContentModels.IlluminationStory cmModel)
        {
            //
            Models.IlluminationStoryContent PgContent = new Models.IlluminationStoryContent();

            //Redirect to home if illumination settings are not active.
            IPublishedContent ipHome = umbracoHelper.TypedContent((int)(Common.siteNode.Home));
            if (ipHome.GetPropertyValue<Boolean>(Common.NodeProperties.activateIlluminationControls) != true)
            {
                PgContent.RedirectHome = true;
            }
            else
            {

                //Obtain page data
                PgContent.Title = cmModel.Title;
                PgContent.ExperienceType = cmModel.ExperienceType;
                PgContent.Story = Html.Raw(umbraco.library.ReplaceLineBreaks(cmModel.Story));

                PgContent.CmMember = new ContentModels.Member(cmModel.Member);

                //Add data to story model
                StringBuilder sbAuthor = new StringBuilder();
                sbAuthor.Append(PgContent.CmMember.FirstName);
                sbAuthor.Append("&nbsp;&nbsp;&nbsp;");
                sbAuthor.Append(PgContent.CmMember.LastName);
                sbAuthor.Append(".");
                PgContent.MemberName = sbAuthor.ToString();

                PgContent.Gender = umbracoHelper.GetPreValueAsString(PgContent.CmMember.Gender);
                PgContent.Religion = PgContent.CmMember.Religion;
                PgContent.Country = PgContent.CmMember.Country;


                //Obtain the form and its view model
                ConfiguredFormInfo pickedForm = cmModel.Parent.GetPropertyValue<ConfiguredFormInfo>("formPicker");
                PgContent.Vm = formulate.api.Rendering.GetFormViewModel(pickedForm.FormId, pickedForm.LayoutId, pickedForm.TemplateId, cmModel);                
            }


            return PgContent;
        }
        public static Models.IlluminationStoryListContent ObtainIlluminationStoryListContent(UmbracoHelper Umbraco, string Url)
        {
            IlluminationStoryListContent illuminationStoryListContent = new IlluminationStoryListContent();

            //build base url for filter links
            illuminationStoryListContent.baseUrl = Url;
            if (illuminationStoryListContent.baseUrl.Contains("?"))
            {
                illuminationStoryListContent.baseUrl = (illuminationStoryListContent.baseUrl.Substring(0, illuminationStoryListContent.baseUrl.IndexOf("?")));
            }

            //Create links for dropdown and buttons
            illuminationStoryListContent.urlViewAll = illuminationStoryListContent.baseUrl;
            illuminationStoryListContent.urlHeavenly = illuminationStoryListContent.baseUrl + "?viewBy=Heavenly";
            illuminationStoryListContent.urlHellish = illuminationStoryListContent.baseUrl + "?viewBy=Hellish";
            illuminationStoryListContent.urlPurgatorial = illuminationStoryListContent.baseUrl + "?viewBy=Purgatorial";
            illuminationStoryListContent.urlOtherUnsure = illuminationStoryListContent.baseUrl + "?viewBy=OtherUnsure";
            illuminationStoryListContent.urlAddEditIllumStory = Umbraco.TypedContent((int)Common.siteNode.AddEditIlluminationStory).Url;
            illuminationStoryListContent.urlViewIllumStatistics = Umbraco.TypedContent((int)Common.siteNode.IlluminationStatistics).Url;
            illuminationStoryListContent.urlDownloadStories = UmbracoContext.Current.PublishedContentRequest.PublishedContent.GetPropertyValue<IPublishedContent>(Common.NodeProperties.compiledStories).Url;


            return illuminationStoryListContent;
        }
        public static Models.AddEditIlluminationStoryContent DoesStoryExist(System.Security.Principal.IPrincipal User)
        {
            //Instantiate variables.
            Models.AddEditIlluminationStoryContent PgContent = new Models.AddEditIlluminationStoryContent();

            //Determine if user already submitted an illumination story
            PgContent.Member = ApplicationContext.Current.Services.MemberService.GetByUsername(User.Identity.Name);
            if (PgContent.Member != null)
            {
                if (PgContent.Member.GetValue(Common.NodeProperties.illuminationStory) != null)
                {
                    PgContent.DoesStoryExist = true;
                }
            }


            return PgContent;
        }


        public static Models.LineCharts ObtainStatistics_byAge(IPublishedContent ipByAge)
        {
            //Initialize variables.
            Models.LineCharts statsByAge = new Models.LineCharts();
            statsByAge.LstChartData = new List<ChartDataset>();
            statsByAge.LstAgeRange = new List<string>();
            ChartDataset HeavenlyDataset = new ChartDataset("Heavenly", "Heavenly", "#4f81bc");
            ChartDataset HellishDataset = new ChartDataset("Hellish", "Hellish", "#bf4b49");
            ChartDataset PurgatorialDataset = new ChartDataset("Purgatorial", "Purgatorial", "#9bbb57");
            ChartDataset UnknownDataset = new ChartDataset("Unknown", "Unknown/Unsure", "#7f7f7f");
            statsByAge.TotalEntries = 0;

            //Obtain data from node
            statsByAge.LstAgeStats_Heavenly = JsonConvert.DeserializeObject<List<LineChart>>(ipByAge.GetPropertyValue<string>(Common.NodeProperties.statsHeavenly));
            statsByAge.LstAgeStats_Hellish = JsonConvert.DeserializeObject<List<LineChart>>(ipByAge.GetPropertyValue<string>(Common.NodeProperties.statsHellish));
            statsByAge.LstAgeStats_Purgatorial = JsonConvert.DeserializeObject<List<LineChart>>(ipByAge.GetPropertyValue<string>(Common.NodeProperties.statsPurgatorial));
            statsByAge.LstAgeStats_Unknown = JsonConvert.DeserializeObject<List<LineChart>>(ipByAge.GetPropertyValue<string>(Common.NodeProperties.statsUnknown));

            //Add data to proper datasets
            foreach (LineChart stat in statsByAge.LstAgeStats_Heavenly)
            {
                HeavenlyDataset.LstData.Add(stat.Count); //Add data to list
                statsByAge.TotalEntries += stat.Count; //Increment total entries

                statsByAge.LstAgeRange.Add(stat.AgeRange); //Add text range [all are the same so only need to do this once.]
            }
            foreach (LineChart stat in statsByAge.LstAgeStats_Hellish)
            {
                HellishDataset.LstData.Add(stat.Count);
                statsByAge.TotalEntries += stat.Count;
            }
            foreach (LineChart stat in statsByAge.LstAgeStats_Purgatorial)
            {
                PurgatorialDataset.LstData.Add(stat.Count);
                statsByAge.TotalEntries += stat.Count;
            }
            foreach (LineChart stat in statsByAge.LstAgeStats_Unknown)
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
        public static Models.StackedBarChart ObtainStatistics_byCountry(IPublishedContent ip)
        {
            //Instantiate variables
            Models.StackedBarChart stats = new Models.StackedBarChart();
            List<string> lstLabels = new List<string>();
            List<int> lstValues_Heavenly = new List<int>();
            List<int> lstValues_Hellish = new List<int>();
            List<int> lstValues_Purgatorial = new List<int>();
            List<int> lstValues_Unknown = new List<int>();
            int totalExperiences = 0;
            int totalPercentage = 0;
            int heavenly = 0;
            int hellish = 0;
            int purgatorial = 0;
            int unknown = 0;

            //Obtain data from ip
            var jsonSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            List<Models.ExperienceByCountry> lstExperiences = JsonConvert.DeserializeObject<List<Models.ExperienceByCountry>>(ip.GetPropertyValue<string>(Common.NodeProperties.experiencesByCountry), jsonSettings);

            //Extract data
            foreach (Models.ExperienceByCountry experience in lstExperiences)
            {
                //Add label to list
                lstLabels.Add(experience.Label);

                //get total
                totalExperiences = experience.Heavenly + experience.Hellish + experience.Purgatorial + experience.Other;

                //get total percentages
                heavenly = Convert.ToInt32(Math.Round(((decimal)experience.Heavenly / totalExperiences) * 100, 0));
                hellish = Convert.ToInt32(Math.Round(((decimal)experience.Hellish / totalExperiences) * 100, 0));
                purgatorial = Convert.ToInt32(Math.Round(((decimal)experience.Purgatorial / totalExperiences) * 100, 0));
                unknown = Convert.ToInt32(Math.Round(((decimal)experience.Other / totalExperiences) * 100, 0));

                totalPercentage = heavenly + hellish + purgatorial + unknown;

                //Add percentages to list
                lstValues_Heavenly.Add(heavenly);
                lstValues_Hellish.Add(hellish);
                lstValues_Purgatorial.Add(purgatorial);
                lstValues_Unknown.Add(unknown + (100 - totalPercentage)); //Add remainder to ensure total = 100%
            }

            //Convert extracted data to json
            stats.Labels = Newtonsoft.Json.JsonConvert.SerializeObject(lstLabels);
            stats.Values_Heavenly = Newtonsoft.Json.JsonConvert.SerializeObject(lstValues_Heavenly);
            stats.Values_Hellish = Newtonsoft.Json.JsonConvert.SerializeObject(lstValues_Hellish);
            stats.Values_Purgatorial = Newtonsoft.Json.JsonConvert.SerializeObject(lstValues_Purgatorial);
            stats.Values_Unknown = Newtonsoft.Json.JsonConvert.SerializeObject(lstValues_Unknown);

            //sets the canvas height to increase bar heights
            stats.Height = lstLabels.Count * 20;

            //Return stats
            return stats;
        }
        public static Models.PieChart ObtainStatistics_byExperienceType(IPublishedContent ip)
        {
            //Instantiate stat object
            PieChart stats = new PieChart();

            //Extractor data from ip
            stats.lstValues.Add(ip.GetPropertyValue<int>(Common.NodeProperties.heavenly));
            stats.lstValues.Add(ip.GetPropertyValue<int>(Common.NodeProperties.hellish));
            stats.lstValues.Add(ip.GetPropertyValue<int>(Common.NodeProperties.purgatorial));
            stats.lstValues.Add(ip.GetPropertyValue<int>(Common.NodeProperties.other));

            //Static data
            stats.lstLabels.Add("Heavenly");
            stats.lstLabels.Add("Purgatorial");
            stats.lstLabels.Add("Hellish");
            stats.lstLabels.Add("Unknown/Unsure");

            List<string> lstBgColors = new List<string>();
            lstBgColors.Add("#4f81bc");
            lstBgColors.Add("#9bbb57");
            lstBgColors.Add("#bf4b49");
            lstBgColors.Add("#7f7f7f");

            List<string> lstHoverBgColors = new List<string>();
            lstHoverBgColors.Add("#8CACD3");
            lstHoverBgColors.Add("#BDD391");
            lstHoverBgColors.Add("#D58987");
            lstHoverBgColors.Add("#b9b9b9");

            //Convert extracted data to json
            stats.BgColors = Newtonsoft.Json.JsonConvert.SerializeObject(lstBgColors);
            stats.HoverBgColors = Newtonsoft.Json.JsonConvert.SerializeObject(lstHoverBgColors);
            stats.Labels = Newtonsoft.Json.JsonConvert.SerializeObject(stats.lstLabels);
            stats.Values = Newtonsoft.Json.JsonConvert.SerializeObject(stats.lstValues);

            //
            foreach (int value in stats.lstValues)
            {
                stats.TotalEntries += value;
            }

            //Return stats
            return stats;
        }
        public static Models.BarChart ObtainStatistics_byGender(IPublishedContent ip)
        {
            //Instantiate variables
            Models.BarChart stats = new Models.BarChart();
            List<Models.ExperienceByGender> lstHeavenly;
            List<Models.ExperienceByGender> lstHellish;
            List<Models.ExperienceByGender> lstPurgatorial;
            List<Models.ExperienceByGender> lstUnknown;

            //Obtain data from ip
            lstHeavenly = JsonConvert.DeserializeObject<List<Models.ExperienceByGender>>(ip.GetPropertyValue<string>(Common.NodeProperties.statsHeavenly));
            lstHellish = JsonConvert.DeserializeObject<List<Models.ExperienceByGender>>(ip.GetPropertyValue<string>(Common.NodeProperties.statsHellish));
            lstPurgatorial = JsonConvert.DeserializeObject<List<Models.ExperienceByGender>>(ip.GetPropertyValue<string>(Common.NodeProperties.statsPurgatorial));
            lstUnknown = JsonConvert.DeserializeObject<List<Models.ExperienceByGender>>(ip.GetPropertyValue<string>(Common.NodeProperties.statsUnknown));

            //Extract data | Labels
            foreach (var stat in lstHeavenly)
            {
                stats.lstLabels.Add(stat.AgeRange);
            }

            //Extract data | Heavenly
            foreach (var stat in lstHeavenly)
            {
                stats.lstValues_Heavenly_Males.Add(stat.Males);
                stats.lstValues_Heavenly_Females.Add(stat.Females);
            }

            //Extract data | Hellish
            foreach (var stat in lstHellish)
            {
                stats.lstValues_Hellish_Males.Add(stat.Males);
                stats.lstValues_Hellish_Females.Add(stat.Females);
            }

            //Extract data | Purgatorial
            foreach (var stat in lstPurgatorial)
            {
                stats.lstValues_Purgatorial_Males.Add(stat.Males);
                stats.lstValues_Purgatorial_Females.Add(stat.Females);
            }

            //Extract data | Unknown
            foreach (var stat in lstUnknown)
            {
                stats.lstValues_Other_Males.Add(stat.Males);
                stats.lstValues_Other_Females.Add(stat.Females);
            }

            //Convert to json
            stats.jsonLabels = Newtonsoft.Json.JsonConvert.SerializeObject(stats.lstLabels);
            stats.jsonValues_Heavenly_Males = Newtonsoft.Json.JsonConvert.SerializeObject(stats.lstValues_Heavenly_Males);
            stats.jsonValues_Heavenly_Females = Newtonsoft.Json.JsonConvert.SerializeObject(stats.lstValues_Heavenly_Females);
            stats.jsonValues_Hellish_Males = Newtonsoft.Json.JsonConvert.SerializeObject(stats.lstValues_Hellish_Males);
            stats.jsonValues_Hellish_Females = Newtonsoft.Json.JsonConvert.SerializeObject(stats.lstValues_Hellish_Females);
            stats.jsonValues_Purgatorial_Males = Newtonsoft.Json.JsonConvert.SerializeObject(stats.lstValues_Purgatorial_Males);
            stats.jsonValues_Purgatorial_Females = Newtonsoft.Json.JsonConvert.SerializeObject(stats.lstValues_Purgatorial_Females);
            stats.jsonValues_Other_Males = Newtonsoft.Json.JsonConvert.SerializeObject(stats.lstValues_Other_Males);
            stats.jsonValues_Other_Females = Newtonsoft.Json.JsonConvert.SerializeObject(stats.lstValues_Other_Females);

            //sets the canvas height to increase bar heights
            stats.Height = lstHeavenly.Count * 20;

            return stats;
        }
        public static Models.StackedBarChart ObtainStatistics_byRace(IPublishedContent ip)
        {
            //Instantiate variables
            List<string> lstLabels = new List<string>();
            List<int> lstValues_Heavenly = new List<int>();
            List<int> lstValues_Hellish = new List<int>();
            List<int> lstValues_Purgatorial = new List<int>();
            List<int> lstValues_Unknown = new List<int>();
            int totalExperiences = 0;
            int totalPercentage = 0;
            int heavenly = 0;
            int hellish = 0;
            int purgatorial = 0;
            int unknown = 0;

            //Obtain data from ip
            List<Models.ExperienceByRace> lstExperiences = JsonConvert.DeserializeObject<List<Models.ExperienceByRace>>(ip.GetPropertyValue<string>(Common.NodeProperties.experiencesByRace));

            //Extract data
            foreach (Models.ExperienceByRace experience in lstExperiences)
            {
                //Add label to list
                lstLabels.Add(experience.Label);

                //get total
                totalExperiences = experience.Heavenly + experience.Hellish + experience.Purgatorial + experience.Other;

                //get total percentages
                heavenly = Convert.ToInt32(Math.Round(((decimal)experience.Heavenly / totalExperiences) * 100, 0));
                hellish = Convert.ToInt32(Math.Round(((decimal)experience.Hellish / totalExperiences) * 100, 0));
                purgatorial = Convert.ToInt32(Math.Round(((decimal)experience.Purgatorial / totalExperiences) * 100, 0));
                unknown = Convert.ToInt32(Math.Round(((decimal)experience.Other / totalExperiences) * 100, 0));

                totalPercentage = heavenly + hellish + purgatorial + unknown;

                //Add percentages to list
                lstValues_Heavenly.Add(heavenly);
                lstValues_Hellish.Add(hellish);
                lstValues_Purgatorial.Add(purgatorial);
                lstValues_Unknown.Add(unknown + (100 - totalPercentage)); //Add remainder to ensure total = 100%
            }

            //Convert extracted data to json
            Models.StackedBarChart stats = new Models.StackedBarChart();
            stats.Labels = Newtonsoft.Json.JsonConvert.SerializeObject(lstLabels);
            stats.Values_Heavenly = Newtonsoft.Json.JsonConvert.SerializeObject(lstValues_Heavenly);
            stats.Values_Hellish = Newtonsoft.Json.JsonConvert.SerializeObject(lstValues_Hellish);
            stats.Values_Purgatorial = Newtonsoft.Json.JsonConvert.SerializeObject(lstValues_Purgatorial);
            stats.Values_Unknown = Newtonsoft.Json.JsonConvert.SerializeObject(lstValues_Unknown);

            //sets the canvas height to increase bar heights
            stats.Height = lstLabels.Count * 20;

            //Return stats
            return stats;
        }
        public static Models.StackedBarChart ObtainStatistics_byReligion(IPublishedContent ip)
        {
            //Instantiate variables
            List<string> lstLabels = new List<string>();
            List<int> lstValues_Heavenly = new List<int>();
            List<int> lstValues_Hellish = new List<int>();
            List<int> lstValues_Purgatorial = new List<int>();
            List<int> lstValues_Unknown = new List<int>();
            int totalExperiences = 0;
            int totalPercentage = 0;
            int heavenly = 0;
            int hellish = 0;
            int purgatorial = 0;
            int unknown = 0;

            //Obtain data from ip
            List<Models.ExperienceByReligion> lstExperiences = JsonConvert.DeserializeObject<List<Models.ExperienceByReligion>>(ip.GetPropertyValue<string>(Common.NodeProperties.experiencesByReligion));

            //Extract data
            foreach (Models.ExperienceByReligion experience in lstExperiences)
            {
                //Add label to list
                lstLabels.Add(experience.Label);

                //get total
                totalExperiences = experience.Heavenly + experience.Hellish + experience.Purgatorial + experience.Other;

                //get total percentages
                heavenly = Convert.ToInt32(Math.Round(((decimal)experience.Heavenly / totalExperiences) * 100, 0));
                hellish = Convert.ToInt32(Math.Round(((decimal)experience.Hellish / totalExperiences) * 100, 0));
                purgatorial = Convert.ToInt32(Math.Round(((decimal)experience.Purgatorial / totalExperiences) * 100, 0));
                unknown = Convert.ToInt32(Math.Round(((decimal)experience.Other / totalExperiences) * 100, 0));

                totalPercentage = heavenly + hellish + purgatorial + unknown;

                //Add percentages to list
                lstValues_Heavenly.Add(heavenly);
                lstValues_Hellish.Add(hellish);
                lstValues_Purgatorial.Add(purgatorial);
                lstValues_Unknown.Add(unknown + (100 - totalPercentage)); //Add remainder to ensure total = 100%
            }

            //Convert extracted data to json
            Models.StackedBarChart stats = new Models.StackedBarChart();
            stats.Labels = Newtonsoft.Json.JsonConvert.SerializeObject(lstLabels);
            stats.Values_Heavenly = Newtonsoft.Json.JsonConvert.SerializeObject(lstValues_Heavenly);
            stats.Values_Hellish = Newtonsoft.Json.JsonConvert.SerializeObject(lstValues_Hellish);
            stats.Values_Purgatorial = Newtonsoft.Json.JsonConvert.SerializeObject(lstValues_Purgatorial);
            stats.Values_Unknown = Newtonsoft.Json.JsonConvert.SerializeObject(lstValues_Unknown);

            //sets the canvas height to increase bar heights
            stats.Height = lstLabels.Count * 20;

            //Return stats
            return stats;
        }


        public static string UpdateAllStatistics()
        {
            //Instantiate variables
            Models.IlluminationStats illuminationStats = new Models.IlluminationStats();

            UmbracoHelper umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
            IMemberService memberService = ApplicationContext.Current.Services.MemberService;
            IPublishedContent ipIlluminationStories = umbracoHelper.TypedContent((int)Common.siteNode.IlluminationStories);


            //Loop through all stories
            foreach (IPublishedContent ip in ipIlluminationStories.Children.ToList())
            {
                if (ip.HasValue(Common.NodeProperties.member))
                {
                    //Instantiate variables
                    IPublishedContent ipMember = ip.GetPropertyValue<IPublishedContent>(Common.NodeProperties.member);

                    //Obtain user's experience type
                    illuminationStats.experienceType = ip.GetPropertyValue<string>(Common.NodeProperties.experienceType);

                    //Obtain data for experience types
                    ObtainUpdateStats_forExperienceType(ref illuminationStats);

                    //Consolidate Info: by Country
                    ObtainUpdateStats_forCountry(ref ipMember, ref illuminationStats);


                    //Consolidate Info: by Age
                    ObtainUpdateStats_forAge(ref ipMember, ref illuminationStats);


                    //Consolidate Info: by Gender
                    ObtainUpdateStats_forGender(ref ipMember, ref illuminationStats);


                    //Consolidate Info: by Religion
                    ObtainUpdateStats_forReligion(ref ipMember, ref illuminationStats);


                    //Consolidate Info: by Race
                    ObtainUpdateStats_forRace(ref ipMember, ref illuminationStats);
                }

            }

            //Sort lists
            illuminationStats.LstExperiences_byCountry = illuminationStats.LstExperiences_byCountry.OrderBy(x => x.Label).ToList();

            //Update nodes with new data
            UpdateStatData_byNode(ref illuminationStats);


            //Generate a pdf version of the stories
            GeneratePdf();

            return JsonConvert.SerializeObject(illuminationStats);
        }
        private static void ObtainUpdateStats_forExperienceType(ref Models.IlluminationStats illuminationStats)
        {
            //Consolidate Info: by Experience Type
            switch (illuminationStats.experienceType)
            {
                case Common.ExperienceType.Heavenly:
                    illuminationStats.experienceType_Heavenly++;
                    break;
                case Common.ExperienceType.Hellish:
                    illuminationStats.experienceType_Hellish++;
                    break;
                case Common.ExperienceType.Purgatorial:
                    illuminationStats.experienceType_Purgatorial++;
                    break;
                case Common.ExperienceType.Other:
                    illuminationStats.experienceType_Other++;
                    break;
                default:
                    break;
            }
        }
        private static void ObtainUpdateStats_forCountry(ref IPublishedContent ipMember, ref Models.IlluminationStats illuminationStats)
        {
            //
            illuminationStats.hasCountry = ipMember.HasValue(Common.NodeProperties.country); ;

            //
            if (illuminationStats.hasCountry)
            {
                Models.ExperienceByCountry experienceByCountry = new ExperienceByCountry();
                string _country = ipMember.GetPropertyValue<string>(Common.NodeProperties.country);

                if (illuminationStats.LstExperiences_byCountry.Any(x => x.Label == _country))
                {
                    experienceByCountry = illuminationStats.LstExperiences_byCountry.Where(x => x.Label == _country).FirstOrDefault();
                }
                else
                {
                    illuminationStats.LstExperiences_byCountry.Add(experienceByCountry);
                    experienceByCountry.Label = _country;
                }


                //Consolidate Info: by Experience Type
                switch (illuminationStats.experienceType)
                {
                    case Common.ExperienceType.Heavenly:
                        experienceByCountry.Heavenly++;
                        break;
                    case Common.ExperienceType.Hellish:
                        experienceByCountry.Hellish++;
                        break;
                    case Common.ExperienceType.Purgatorial:
                        experienceByCountry.Purgatorial++;
                        break;
                    case Common.ExperienceType.Other:
                        experienceByCountry.Other++;
                        break;
                    default:
                        break;
                }
            }
        }
        private static void ObtainUpdateStats_forAge(ref IPublishedContent ipMember, ref Models.IlluminationStats illuminationStats)
        {
            illuminationStats.hasAge = ipMember.HasValue(Common.NodeProperties.age);
            if (illuminationStats.hasAge)
            {
                //
                Models.LineChart experienceByAge = new LineChart();
                illuminationStats.age = ipMember.GetPropertyValue<int>(Common.NodeProperties.age);
                string _ageRange = "";

                //
                if (illuminationStats.age >= 0 && illuminationStats.age <= 4) { _ageRange = "0"; }
                else if (illuminationStats.age >= 5 && illuminationStats.age <= 9) { _ageRange = "5"; }
                else if (illuminationStats.age >= 10 && illuminationStats.age <= 14) { _ageRange = "10"; }
                else if (illuminationStats.age >= 15 && illuminationStats.age <= 19) { _ageRange = "15"; }
                else if (illuminationStats.age >= 20 && illuminationStats.age <= 24) { _ageRange = "20"; }
                else if (illuminationStats.age >= 25 && illuminationStats.age <= 29) { _ageRange = "25"; }
                else if (illuminationStats.age >= 30 && illuminationStats.age <= 34) { _ageRange = "30"; }
                else if (illuminationStats.age >= 35 && illuminationStats.age <= 39) { _ageRange = "35"; }
                else if (illuminationStats.age >= 40 && illuminationStats.age <= 44) { _ageRange = "40"; }
                else if (illuminationStats.age >= 45 && illuminationStats.age <= 49) { _ageRange = "45"; }
                else if (illuminationStats.age >= 50 && illuminationStats.age <= 54) { _ageRange = "50"; }
                else if (illuminationStats.age >= 55 && illuminationStats.age <= 59) { _ageRange = "55"; }
                else if (illuminationStats.age >= 60 && illuminationStats.age <= 64) { _ageRange = "60"; }
                else if (illuminationStats.age >= 65 && illuminationStats.age <= 69) { _ageRange = "65"; }
                else if (illuminationStats.age >= 70 && illuminationStats.age <= 74) { _ageRange = "70"; }
                else if (illuminationStats.age >= 75 && illuminationStats.age <= 79) { _ageRange = "75"; }
                else if (illuminationStats.age >= 80 && illuminationStats.age <= 84) { _ageRange = "80"; }
                else if (illuminationStats.age >= 85 && illuminationStats.age <= 89) { _ageRange = "85"; }
                else if (illuminationStats.age >= 90 && illuminationStats.age <= 94) { _ageRange = "90"; }
                else if (illuminationStats.age >= 95 && illuminationStats.age <= 99) { _ageRange = "95"; }
                else if (illuminationStats.age >= 100) { _ageRange = "100+"; }

                //Consolidate Info: by Experience Type
                switch (illuminationStats.experienceType)
                {
                    case Common.ExperienceType.Heavenly:
                        //
                        experienceByAge = illuminationStats.lstAge_Heavenly.Where(x => x.AgeRange == _ageRange).FirstOrDefault();
                        experienceByAge.Count++;
                        break;
                    case Common.ExperienceType.Hellish:
                        //
                        experienceByAge = illuminationStats.lstAge_Hellish.Where(x => x.AgeRange == _ageRange).FirstOrDefault();
                        experienceByAge.Count++;
                        break;
                    case Common.ExperienceType.Purgatorial:
                        //
                        experienceByAge = illuminationStats.lstAge_Purgatorial.Where(x => x.AgeRange == _ageRange).FirstOrDefault();
                        experienceByAge.Count++;
                        break;
                    case Common.ExperienceType.Other:
                        //
                        experienceByAge = illuminationStats.lstAge_Unknown.Where(x => x.AgeRange == _ageRange).FirstOrDefault();
                        experienceByAge.Count++;
                        break;
                    default:
                        break;
                }
            }
        }
        private static void ObtainUpdateStats_forGender(ref IPublishedContent ipMember, ref Models.IlluminationStats illuminationStats)
        {
            //
            if (illuminationStats.hasAge)
            {
                illuminationStats.hasGender = ipMember.HasValue(Common.NodeProperties.gender);
                if (illuminationStats.hasGender)
                {
                    //
                    ExperienceByGender experienceByGender = new ExperienceByGender();
                    string gender = Common.GetPreValueString(ipMember.GetPropertyValue<string>(Common.NodeProperties.gender));
                    string _ageRange = "";

                    //
                    if (illuminationStats.age >= 0 && illuminationStats.age <= 4) { _ageRange = "0-5"; }
                    else if (illuminationStats.age >= 5 && illuminationStats.age <= 9) { _ageRange = "6-10"; }
                    else if (illuminationStats.age >= 10 && illuminationStats.age <= 14) { _ageRange = "11-15"; }
                    else if (illuminationStats.age >= 15 && illuminationStats.age <= 19) { _ageRange = "16-20"; }
                    else if (illuminationStats.age >= 20 && illuminationStats.age <= 24) { _ageRange = "21-25"; }
                    else if (illuminationStats.age >= 25 && illuminationStats.age <= 29) { _ageRange = "26-30"; }
                    else if (illuminationStats.age >= 30 && illuminationStats.age <= 34) { _ageRange = "31-35"; }
                    else if (illuminationStats.age >= 35 && illuminationStats.age <= 39) { _ageRange = "36-40"; }
                    else if (illuminationStats.age >= 40 && illuminationStats.age <= 44) { _ageRange = "41-45"; }
                    else if (illuminationStats.age >= 45 && illuminationStats.age <= 49) { _ageRange = "46-50"; }
                    else if (illuminationStats.age >= 50 && illuminationStats.age <= 54) { _ageRange = "51-55"; }
                    else if (illuminationStats.age >= 55 && illuminationStats.age <= 59) { _ageRange = "56-60"; }
                    else if (illuminationStats.age >= 60 && illuminationStats.age <= 64) { _ageRange = "61-65"; }
                    else if (illuminationStats.age >= 65 && illuminationStats.age <= 69) { _ageRange = "66-70"; }
                    else if (illuminationStats.age >= 70 && illuminationStats.age <= 74) { _ageRange = "71-75"; }
                    else if (illuminationStats.age >= 75 && illuminationStats.age <= 79) { _ageRange = "76-80"; }
                    else if (illuminationStats.age >= 80 && illuminationStats.age <= 84) { _ageRange = "81-85"; }
                    else if (illuminationStats.age >= 85 && illuminationStats.age <= 89) { _ageRange = "86-90"; }
                    else if (illuminationStats.age >= 90 && illuminationStats.age <= 94) { _ageRange = "91-95"; }
                    else if (illuminationStats.age >= 95) { _ageRange = "96-100+"; }

                    //Consolidate Info: by Experience Type
                    switch (illuminationStats.experienceType)
                    {
                        case Common.ExperienceType.Heavenly:
                            //
                            experienceByGender = illuminationStats.lstGender_Heavenly.Where(x => x.AgeRange == _ageRange).FirstOrDefault();

                            //Increment gender
                            if (gender == Common.Gender.Male)
                            {
                                experienceByGender.Males++;
                            }
                            else
                            {
                                experienceByGender.Females++;
                            }

                            break;
                        case Common.ExperienceType.Hellish:
                            //
                            experienceByGender = illuminationStats.lstGender_Hellish.Where(x => x.AgeRange == _ageRange).FirstOrDefault();

                            //Increment gender
                            if (gender == Common.Gender.Male)
                            {
                                experienceByGender.Males++;
                            }
                            else
                            {
                                experienceByGender.Females++;
                            }

                            break;
                        case Common.ExperienceType.Purgatorial:
                            //
                            experienceByGender = illuminationStats.lstGender_Purgatorial.Where(x => x.AgeRange == _ageRange).FirstOrDefault();

                            //Increment gender
                            if (gender == Common.Gender.Male)
                            {
                                experienceByGender.Males++;
                            }
                            else
                            {
                                experienceByGender.Females++;
                            }

                            break;
                        case Common.ExperienceType.Other:
                            //
                            experienceByGender = illuminationStats.lstGender_Unknown.Where(x => x.AgeRange == _ageRange).FirstOrDefault();

                            //Increment gender
                            if (gender == Common.Gender.Male)
                            {
                                experienceByGender.Males++;
                            }
                            else
                            {
                                experienceByGender.Females++;
                            }

                            break;
                        default:
                            break;
                    }
                }
            }
        }
        private static void ObtainUpdateStats_forRace(ref IPublishedContent ipMember, ref Models.IlluminationStats illuminationStats)
        {
            //
            illuminationStats.hasRace = ipMember.HasValue(Common.NodeProperties.race); ;

            //
            if (illuminationStats.hasRace)
            {
                //
                Models.ExperienceByRace experienceByRace;
                illuminationStats.lstRace = ipMember.GetPropertyValue<IEnumerable<string>>(Common.NodeProperties.race).ToList();

                //Consolidate Info: by Race
                foreach (var race in illuminationStats.lstRace)
                {
                    //
                    experienceByRace = illuminationStats.lstRaces.Where(x => x.Label == race).FirstOrDefault();

                    switch (illuminationStats.experienceType)
                    {
                        case Common.ExperienceType.Heavenly:
                            experienceByRace.Heavenly++;
                            break;
                        case Common.ExperienceType.Hellish:
                            experienceByRace.Hellish++;
                            break;
                        case Common.ExperienceType.Purgatorial:
                            experienceByRace.Purgatorial++;
                            break;
                        case Common.ExperienceType.Other:
                            experienceByRace.Other++;
                            break;
                        default:
                            break;
                    }
                }

            }
        }
        private static void ObtainUpdateStats_forReligion(ref IPublishedContent ipMember, ref Models.IlluminationStats illuminationStats)
        {
            //
            illuminationStats.hasReligion = ipMember.HasValue(Common.NodeProperties.religion); ;

            //
            if (illuminationStats.hasReligion)
            {
                //
                Models.ExperienceByReligion experienceByReligion = new ExperienceByReligion();
                string _religion = ipMember.GetPropertyValue<string>(Common.NodeProperties.religion);


                experienceByReligion = illuminationStats.lstReligions.Where(x => x.Label == _religion).FirstOrDefault();


                //Consolidate Info: by Experience Type
                switch (illuminationStats.experienceType)
                {
                    case Common.ExperienceType.Heavenly:
                        experienceByReligion.Heavenly++;
                        break;
                    case Common.ExperienceType.Hellish:
                        experienceByReligion.Hellish++;
                        break;
                    case Common.ExperienceType.Purgatorial:
                        experienceByReligion.Purgatorial++;
                        break;
                    case Common.ExperienceType.Other:
                        experienceByReligion.Other++;
                        break;
                    default:
                        break;
                }
            }
        }
        private static void UpdateStatData_byNode(ref Models.IlluminationStats illuminationStats)
        {
            //Instantiate variables
            IContentService contentService = ApplicationContext.Current.Services.ContentService;
            IContent icByAge = contentService.GetById((int)Common.siteNode.ByAge);
            IContent icByCountry = contentService.GetById((int)Common.siteNode.ByCountry);
            IContent icByExperienceType = contentService.GetById((int)Common.siteNode.ByExperienceType);
            IContent icByGender = contentService.GetById((int)Common.siteNode.ByGender);
            IContent icByRace = contentService.GetById((int)Common.siteNode.ByRace);
            IContent icByReligion = contentService.GetById((int)Common.siteNode.ByReligion);


            //Update Age stats
            icByAge.SetValue(Common.NodeProperties.statsHeavenly, Newtonsoft.Json.JsonConvert.SerializeObject(illuminationStats.lstAge_Heavenly));
            icByAge.SetValue(Common.NodeProperties.statsHellish, Newtonsoft.Json.JsonConvert.SerializeObject(illuminationStats.lstAge_Hellish));
            icByAge.SetValue(Common.NodeProperties.statsPurgatorial, Newtonsoft.Json.JsonConvert.SerializeObject(illuminationStats.lstAge_Purgatorial));
            icByAge.SetValue(Common.NodeProperties.statsUnknown, Newtonsoft.Json.JsonConvert.SerializeObject(illuminationStats.lstAge_Unknown));
            var result = contentService.SaveAndPublishWithStatus(icByAge);


            //Update Country stats
            icByCountry.SetValue(Common.NodeProperties.experiencesByCountry, Newtonsoft.Json.JsonConvert.SerializeObject(illuminationStats.LstExperiences_byCountry));
            result = contentService.SaveAndPublishWithStatus(icByCountry);


            //Update Age stats
            icByExperienceType.SetValue(Common.NodeProperties.heavenly, illuminationStats.experienceType_Heavenly);
            icByExperienceType.SetValue(Common.NodeProperties.hellish, illuminationStats.experienceType_Hellish);
            icByExperienceType.SetValue(Common.NodeProperties.purgatorial, illuminationStats.experienceType_Purgatorial);
            icByExperienceType.SetValue(Common.NodeProperties.other, illuminationStats.experienceType_Other);
            result = contentService.SaveAndPublishWithStatus(icByExperienceType);


            //Update Age stats
            icByGender.SetValue(Common.NodeProperties.statsHeavenly, Newtonsoft.Json.JsonConvert.SerializeObject(illuminationStats.lstGender_Heavenly));
            icByGender.SetValue(Common.NodeProperties.statsHellish, Newtonsoft.Json.JsonConvert.SerializeObject(illuminationStats.lstGender_Hellish));
            icByGender.SetValue(Common.NodeProperties.statsPurgatorial, Newtonsoft.Json.JsonConvert.SerializeObject(illuminationStats.lstGender_Purgatorial));
            icByGender.SetValue(Common.NodeProperties.statsUnknown, Newtonsoft.Json.JsonConvert.SerializeObject(illuminationStats.lstGender_Unknown));
            result = contentService.SaveAndPublishWithStatus(icByGender);


            //Update Country stats
            icByRace.SetValue(Common.NodeProperties.experiencesByRace, Newtonsoft.Json.JsonConvert.SerializeObject(illuminationStats.lstRaces));
            result = contentService.SaveAndPublishWithStatus(icByRace);


            //Update Country stats
            icByReligion.SetValue(Common.NodeProperties.experiencesByReligion, Newtonsoft.Json.JsonConvert.SerializeObject(illuminationStats.lstReligions));
            result = contentService.SaveAndPublishWithStatus(icByReligion);
        }


        public static void GeneratePdf()
        {
            //Obtain the file name and location from Umbraco
            UmbracoHelper umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
            IPublishedContent ipIlluminationStories = umbracoHelper.TypedContent((int)Common.siteNode.IlluminationStories);
            string fileLocation = HttpRuntime.AppDomainAppPath + ipIlluminationStories.GetPropertyValue<IPublishedContent>(Common.NodeProperties.compiledStories).Url.TrimStart('/');
            fileLocation = fileLocation.Replace(@"/", @"\");

            //Create the document
            Document document = CreateDocument();

            //Convert to pdf and save
            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
            renderer.Document = document;
            renderer.RenderDocument();
            renderer.PdfDocument.Save(fileLocation);
        }
        private static Document CreateDocument()
        {
            // Create a new MigraDoc document
            Document document = new Document();
            document.Info.Title = "The Illumination of Conscience";
            document.Info.Subject = "Testimonials of the Warning";
            document.Info.Author = @"Jim Fifth | AfterTheWarning.com";

            DefineStyles(document);
            DefineCover(document);
            DefineContentSection(document);
            AddContent(document);

            return document;
        }
        private static void DefineStyles(Document document)
        {
            // Get the predefined style Normal.
            Style style = document.Styles["Normal"];
            // Because all styles are derived from Normal, the next line changes the 
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            style.Font.Name = "Times New Roman";
            style.ParagraphFormat.SpaceAfter = 3;


            style = document.Styles["Heading1"];
            style.Font.Name = "Tahoma";
            style.Font.Size = 24;
            style.Font.Bold = true;
            style.Font.Color = Colors.Black;
            style.ParagraphFormat.PageBreakBefore = false;
            style.ParagraphFormat.SpaceAfter = 6;


            style = document.Styles["Heading2"];
            style.Font.Size = 18;
            style.Font.Bold = false;
            style.Font.Color = Colors.DarkOrange;
            style.ParagraphFormat.PageBreakBefore = false;
            style.ParagraphFormat.SpaceBefore = 6;
            style.ParagraphFormat.SpaceAfter = 6;


            style = document.Styles["Heading3"];
            style.Font.Size = 10;
            style.Font.Bold = true;
            style.Font.Italic = true;
            style.ParagraphFormat.SpaceBefore = 6;
            style.ParagraphFormat.SpaceAfter = 3;


            style = document.Styles["Heading4"];
            style.Font.Size = 9;
            style.Font.Bold = false;
            style.Font.Italic = false;
            style.Font.Color = Colors.Gray;
            style.ParagraphFormat.SpaceBefore = 6;
            style.ParagraphFormat.SpaceAfter = 6;
            style.ParagraphFormat.Alignment = ParagraphAlignment.Left;


            style = document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);


            style = document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Create a new style called TOC based on style Normal
            style = document.Styles.AddStyle("TOC", "Normal");
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right, TabLeader.Dots);
            style.ParagraphFormat.Font.Color = Colors.Blue;
        }
        private static void DefineCover(Document document)
        {
            MigraDoc.DocumentObjectModel.Section section = document.AddSection();
            section.PageSetup.PageFormat = PageFormat.Letter;

            MigraDoc.DocumentObjectModel.Shapes.Image image = section.AddImage(HttpRuntime.AppDomainAppPath + @"images\PdfCover.jpg");
            image.Width = "8.5in";
            image.Height = "11in";
            image.RelativeHorizontal = RelativeHorizontal.Page;
            image.RelativeVertical = RelativeVertical.Page;

        }
        private static void DefineContentSection(Document document)
        {
            MigraDoc.DocumentObjectModel.Section section = document.AddSection();
            section.PageSetup.StartingNumber = 1;

            // Add paragraph to footer
            Paragraph paragraph = new Paragraph();
            section.Footers.Primary.Add(paragraph);
            section.Footers.EvenPage.Add(paragraph.Clone());

            paragraph.Format.Alignment = ParagraphAlignment.Right;
            paragraph.AddFormattedText(@"AfterTheWarning.com  |  ", TextFormat.Bold | TextFormat.Italic);
            paragraph.AddPageField();
        }
        private static void AddContent(Document document)
        {
            //Instantiate variables
            List<IlluminationPdfStat> lstIlluminationPdfStats = ObtainPdfStats();
            string ExperienceType = "";
            Paragraph paragraph;
            Boolean isFirst = true;


            //Add all stories to pdf
            foreach (IlluminationPdfStat stat in lstIlluminationPdfStats)
            {
                //Determine if a new section is to be created
                if (ExperienceType != stat.ExperienceType)
                {
                    //Add page break between sections
                    if (isFirst)
                    {
                        isFirst = false;
                    }
                    else
                    {
                        document.LastSection.AddPageBreak();
                    }

                    //Add experience type as title
                    ExperienceType = stat.ExperienceType;
                    paragraph = document.LastSection.AddParagraph(ExperienceType + " Stories", "Heading1");
                }

                //Add story
                document.LastSection.AddParagraph(stat.Title, "Heading2");
                document.LastSection.AddParagraph("By " + stat.Author, "Heading3");

                //Add statistics
                document.LastSection.AddParagraph("[", "Heading4");
                if (!string.IsNullOrEmpty(stat.ExperienceType))
                {
                    document.LastSection.LastParagraph.AddFormattedText("Experience: ", TextFormat.Bold);
                    document.LastSection.LastParagraph.AddText(stat.ExperienceType + " | ");
                }
                if (stat.Age > 0)
                {
                    document.LastSection.LastParagraph.AddFormattedText("Age: ", TextFormat.Bold);
                    document.LastSection.LastParagraph.AddText(stat.Age.ToString() + " | ");
                }
                if (!string.IsNullOrEmpty(stat.Gender))
                {
                    document.LastSection.LastParagraph.AddFormattedText("Gender: ", TextFormat.Bold);
                    document.LastSection.LastParagraph.AddText(stat.Gender + " | ");
                }
                if (!string.IsNullOrEmpty(stat.Religion))
                {
                    document.LastSection.LastParagraph.AddFormattedText("Religion: ", TextFormat.Bold);
                    document.LastSection.LastParagraph.AddText(stat.Religion + " | ");
                }
                if (stat.Races.Count > 0)
                {
                    document.LastSection.LastParagraph.AddFormattedText("Race: ", TextFormat.Bold);
                    foreach (string race in stat.Races)
                    {
                        document.LastSection.LastParagraph.AddText(race + " ");
                    }
                    document.LastSection.LastParagraph.AddText("| ");
                }
                if (!string.IsNullOrEmpty(stat.Country))
                {
                    document.LastSection.LastParagraph.AddFormattedText("Country: ", TextFormat.Bold);
                    document.LastSection.LastParagraph.AddText(stat.Country + " | ");
                }

                document.LastSection.LastParagraph.AddFormattedText("Id: ", TextFormat.Bold);
                document.LastSection.LastParagraph.AddText(stat.Id.ToString());
                document.LastSection.LastParagraph.AddText("]");


                //Add Story
                document.LastSection.AddParagraph(stat.Story, "Normal");
            }

        }
        private static List<IlluminationPdfStat> ObtainPdfStats()
        {
            //Instantiate variables
            List<IlluminationPdfStat> lstIlluminationPdfStats = new List<IlluminationPdfStat>();
            UmbracoHelper umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
            IMemberService memberService = ApplicationContext.Current.Services.MemberService;
            IPublishedContent ipIlluminationStories = umbracoHelper.TypedContent((int)Common.siteNode.IlluminationStories);


            //Loop through all stories
            foreach (IPublishedContent ip in ipIlluminationStories.Children.ToList())
            {
                if (ip.HasValue(Common.NodeProperties.member))
                {
                    //Instantiate variables
                    IlluminationPdfStat stat = new IlluminationPdfStat();
                    IPublishedContent ipMember = ip.GetPropertyValue<IPublishedContent>(Common.NodeProperties.member);

                    //Obtain the content models
                    var CmIlluminationStory = new ContentModels.IlluminationStory(ip);
                    var CmMember = new ContentModels.Member(ip.GetPropertyValue<IPublishedContent>(Common.NodeProperties.member));

                    //Obtain the member's name
                    StringBuilder sbAuthor = new StringBuilder();
                    sbAuthor.Append(CmMember.FirstName);
                    sbAuthor.Append("  ");
                    sbAuthor.Append(CmMember.LastName);
                    sbAuthor.Append(".");
                    stat.Author = sbAuthor.ToString();

                    //Obtain the story data
                    stat.Id = CmIlluminationStory.Id;
                    stat.Title = CmIlluminationStory.Title;
                    stat.ExperienceType = CmIlluminationStory.ExperienceType;
                    stat.Story = CmIlluminationStory.Story.Replace("\r\n\r\n", "\r\n").Replace("\r\n\r\n", "\r\n").Replace("\r\n\r\n", "\r\n").Replace("\r\n", "\r\n\r\n");  //set newlines to x2

                    if (CmMember.Age > 0) stat.Age = CmMember.Age;
                    if (CmMember.Gender > 0) stat.Gender = Common.GetPreValueString(CmMember.Gender.ToString());
                    stat.Religion = CmMember.Religion;
                    stat.Races = CmMember.Race.ToList();
                    stat.Country = CmMember.Country;


                    //Add data to list
                    lstIlluminationPdfStats.Add(stat);

                    break;
                }
            }

            //Sort list
            lstIlluminationPdfStats = lstIlluminationPdfStats.OrderBy(x => x.ExperienceType).ThenBy(x => x.Author).ThenBy(x => x.Religion).ThenBy(x => x.Country).ToList();


            return lstIlluminationPdfStats;
        }


        public static Boolean areIlluminationControlsActivated(UmbracoHelper Umbraco)
        {
            //Are Illumination Controls Active
            IPublishedContent ipHome = Umbraco.TypedContent((int)(Common.siteNode.Home));
            return ipHome.GetPropertyValue<Boolean>(Common.NodeProperties.activateIlluminationControls);
        }
        #endregion
    }
}







//#region "Test Stories"
//public static string GenerateTestStories()
//{
//    //Instantiate variables
//    Stopwatch sw = new Stopwatch();
//    sw.Start();
//    TempResults results = new TempResults();

//    try
//    {
//        int entryCount = 15000;
//        _memberships membership = new _memberships();
//        IMemberService memberService = ApplicationContext.Current.Services.MemberService;

//        Array lstCountries = Enum.GetValues(typeof(Common.Countries));
//        Array lstGenders = Enum.GetValues(typeof(Common.Genders));
//        Array lstRaces = Enum.GetValues(typeof(Common.Races));
//        Array lstReligions = Enum.GetValues(typeof(Common.Religions));
//        Array lstExperienceTypes = Enum.GetValues(typeof(Common.ExperienceTypes));

//        List<MembershipModel> lstMembers = new List<MembershipModel>();
//        List<illuminationStory> lstIlluminationStories = new List<illuminationStory>();


//        //Create story array
//        List<string> lstStory = new List<string>();
//        lstStory.Add("Acid effects antimagic bonus cold domain cure spell fire subtype glamer subschool melee move action strength domain suffocation.");
//        lstStory.Add("Ability damage character dying energy damage inherent bonus portal domain ranged attack roll scrying subschool speed suffocation water dangers. Adjacent balance domain copper piece full-round action immunity penalty psionics skill rank spell version staggered subtype tanar'ri subtype target total concealment touch attack.");
//        lstStory.Add("Aberration type calling subschool coup de grace domain spell entangled ethereal plane fear cone improved grab initiative count light weapon living luck domain melee mentalism domain monstrous humanoid type morale bonus orc domain paladin petrified ranged attack rounding scribe shield bonus spell level square subtype threaten tyranny domain.");
//        lstStory.Add("Abjuration acid effects antimagic archon subtype change shape cowering damage reduction evil domain experience points extraplanar immunity lava effects law domain luck domain pinned player character size modifier spell resistance spell trigger item stable trade domain transmutation.");
//        lstStory.Add("5-foot step arcane spell failure automatic hit base save bonus command undead conjuration enhancement bonus evasion extraplanar subtype fatigued force damage gold piece incorporeal mundane nauseated plant domain ranged attack roll rogue size modifier special quality spell preparation tanar'ri subtype threat threatened square turning damage water subtype.");
//        lstStory.Add("Charm climb critical hit dazed disabled earth domain energy drain fear effect grapple check grappling hit points immediate action incorporeal subtype manufactured weapons medium natural reach paralyzed planning domain prerequisite profane bonus psionics reflex save stack strength summoning subschool swim tanar'ri subtype teleportation subschool treasure unarmed strike.");
//        lstStory.Add("Abjuration action cantrip catching on fire character compulsion subschool darkness domain fate domain fine giant type knocked down modifier mundane negative level nonlethal damage paralyzed pounce spell descriptor spell resistance suffering domain summoning subschool undeath domain water dangers.");
//        lstStory.Add("Animal domain attack attack roll base attack bonus charm domain checked critical roll cure spell death attack difficult terrain disease elf domain evocation falling fear aura force damage gnome domain good domain magic domain melee attack natural weapon one-handed weapon panicked party plant domain retribution domain shaken water domain.");
//        lstStory.Add("Ability damaged angel subtype copper piece creation subschool creature type drow domain flight hardness natural reach negative energy pounce reaction rebuke undead rend subschool transmutation turning check.");
//        lstStory.Add("Abjuration acid effects antimagic archon subtype change shape cowering damage reduction evil domain experience points extraplanar immunity lava effects law domain luck domain pinned player character size modifier spell resistance spell trigger item stable trade domain transmutation.");



//        //Create all test members and stories
//        for (int i = 0; i < entryCount; i++)
//        {
//            //Create a test member
//            MembershipModel memberModel = new MembershipModel();
//            memberModel.FirstName = GetRandomLetter(i);
//            memberModel.LastName = GetRandomLetter(i + 1);
//            memberModel.Password = "Pa55word";
//            memberModel.Email = "JF_" + String.Format("{0:00000}", i) + "@noemail.com";
//            lstMembers.Add(memberModel);


//            //Create a test story
//            illuminationStory story = new illuminationStory();
//            story.Age = _random.Next(1, 121);
//            story.Country = (Common.Countries)lstCountries.GetValue(_random.Next(lstCountries.Length));
//            story.ExperienceType = (Common.ExperienceTypes)lstExperienceTypes.GetValue(_random.Next(lstExperienceTypes.Length));
//            story.Gender = (Common.Genders)lstGenders.GetValue(_random.Next(lstGenders.Length));
//            story.Race = (Common.Races)lstRaces.GetValue(_random.Next(lstRaces.Length));
//            story.Religion = (Common.Religions)lstReligions.GetValue(_random.Next(lstReligions.Length));
//            story.Author = memberModel.FirstName + " " + memberModel.LastName;
//            story.Title = "Illumination Sample " + String.Format("{0:00000}", i);
//            int length = _random.Next(5, 10);
//            StringBuilder sbStory = new StringBuilder();
//            for (int j = 0; j < length; j++)
//            {
//                sbStory.AppendLine(lstStory[j]);
//            }
//            story.Story = sbStory.ToString();
//            lstIlluminationStories.Add(story);
//        }


//        //Add each member and story 
//        for (int k = 0; k < entryCount; k++)
//        {
//            //Create membership
//            int memberId = membership.CreateMember(lstMembers[k].FirstName, lstMembers[k].LastName, lstMembers[k].Email, lstMembers[k].Password);
//            lstIlluminationStories[k].memberId = memberId;

//            membership.MakeAcctActive(memberId);

//            // Expose the custom properties for the member
//            IMember member = memberService.GetById(memberId);

//            //Add data to member and save
//            member.SetValue(Common.NodeProperties.age, lstIlluminationStories[k].Age);
//            member.SetValue(Common.NodeProperties.country, lstIlluminationStories[k].Country.GetType().GetMember(lstIlluminationStories[k].Country.ToString()).First().GetCustomAttribute<DisplayAttribute>().GetName());
//            member.SetValue(Common.NodeProperties.gender, Common.GetPrevalueIdForIMember(member, Common.NodeProperties.gender, lstIlluminationStories[k].Gender.GetType().GetMember(lstIlluminationStories[k].Gender.ToString()).First().GetCustomAttribute<DisplayAttribute>().GetName()));
//            member.SetValue(Common.NodeProperties.religion, Common.GetPrevalueIdForIMember(member, Common.NodeProperties.religion, lstIlluminationStories[k].Religion.GetType().GetMember(lstIlluminationStories[k].Religion.ToString()).First().GetCustomAttribute<DisplayAttribute>().GetName()));
//            member.SetValue(Common.NodeProperties.race, Common.GetPrevalueIdForIMember(member, Common.NodeProperties.race, lstIlluminationStories[k].Race.GetType().GetMember(lstIlluminationStories[k].Race.ToString()).First().GetCustomAttribute<DisplayAttribute>().GetName()));

//            //Save all changes
//            memberService.Save(member);


//            //Instantiate variables
//            IContentService contentService = ApplicationContext.Current.Services.ContentService;
//            IContent icIllumStory = contentService.CreateContent(name: lstIlluminationStories[k].Title.Substring(0, 1).ToUpper() + lstIlluminationStories[k].Title.Substring(1).ToLower(), parentId: (int)Common.siteNode.IlluminationStories, contentTypeAlias: Common.docType.IlluminationStory);

//            icIllumStory.SetValue(ContentModels.IlluminationStory.GetModelPropertyType(x => x.Title).PropertyTypeAlias, lstIlluminationStories[k].Title.Substring(0, 1).ToUpper() + lstIlluminationStories[k].Title.Substring(1).ToLower());
//            icIllumStory.SetValue(ContentModels.IlluminationStory.GetModelPropertyType(x => x.Story).PropertyTypeAlias, lstIlluminationStories[k].Story);
//            icIllumStory.SetValue(ContentModels.IlluminationStory.GetModelPropertyType(x => x.Member).PropertyTypeAlias, memberId);

//            string experienceType = lstIlluminationStories[k].ExperienceType.Value.ToString();
//            var enumExperienceType = (Common.ExperienceTypes)System.Enum.Parse(typeof(Common.ExperienceTypes), experienceType);
//            if ((Common.ExperienceTypes)System.Enum.Parse(typeof(Common.ExperienceTypes), experienceType) == Common.ExperienceTypes.OtherorUnsure) { experienceType = "Other or Unsure"; }
//            icIllumStory.SetValue(
//                ContentModels.IlluminationStory.GetModelPropertyType(x => x.ExperienceType).PropertyTypeAlias,
//                Common.GetPrevalueIdForIContent(icIllumStory, ContentModels.IlluminationStory.GetModelPropertyType(x => x.ExperienceType).PropertyTypeAlias, experienceType));


//            //Save new Illumination story
//            var result = contentService.SaveAndPublishWithStatus(icIllumStory);


//            if (result.Success)
//            {
//                //Save all changes
//                member.SetValue(Common.NodeProperties.illuminationStory, new GuidUdi("document", icIllumStory.Key).ToString());
//                memberService.Save(member);

//            }
//        }


//        results.memberCount = lstMembers.Count;
//        results.avgAge = Convert.ToInt32(lstIlluminationStories.Average(x => x.Age));
//        results.xx = lstIlluminationStories.Count(x => x.Gender.Value == Common.Genders.Male);
//        results.xy = lstIlluminationStories.Count(x => x.Gender.Value == Common.Genders.Female);
//        results.heavenly = lstIlluminationStories.Count(x => x.ExperienceType.Value == Common.ExperienceTypes.Heavenly);
//        results.hellish = lstIlluminationStories.Count(x => x.ExperienceType.Value == Common.ExperienceTypes.Hellish);
//        results.purgatorial = lstIlluminationStories.Count(x => x.ExperienceType.Value == Common.ExperienceTypes.Purgatorial);
//        results.other = lstIlluminationStories.Count(x => x.ExperienceType.Value == Common.ExperienceTypes.OtherorUnsure);
//        results.american = lstIlluminationStories.Count(x => x.Country.Value == Common.Countries.UnitedStates);
//        results.catholic = lstIlluminationStories.Count(x => x.Religion.Value == Common.Religions.Catholic);
//        results.satanism = lstIlluminationStories.Count(x => x.Religion.Value == Common.Religions.Satinism);
//    }
//    catch (Exception ex)
//    {
//        //results.resultMsg = ex.ToString();
//        results.errorMsg = ex.ToString();
//    }

//    //Display time lapsed
//    sw.Stop();
//    results.elapsedTime = sw.Elapsed.TotalSeconds.ToString();


//    return Newtonsoft.Json.JsonConvert.SerializeObject(results);
//    //return results;
//}
//public static string GetRandomLetter(int seed)
//{
//    // This method returns a random letter between 'A' and 'Z'.
//    int num = _random.Next(0, 26); // Zero to 25
//    char let = (char)('a' + num);
//    return let.ToString().ToUpper();
//}

//#endregion