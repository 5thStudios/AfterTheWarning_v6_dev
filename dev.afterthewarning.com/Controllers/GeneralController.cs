using formulate.app.Types;
using formulate.core.Models;
using Models;
using System;
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
    }
}
