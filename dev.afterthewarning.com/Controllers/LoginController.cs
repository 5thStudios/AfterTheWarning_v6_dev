using Models;
using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using Umbraco.Web;
using Umbraco.Web.Mvc;


namespace Controllers
{
    public class LoginController : SurfaceController
    {
        //Render Form
        public ActionResult RenderForm()
        {
            try
            {
                //Determine if user is valid (linked from email)
                if (!string.IsNullOrEmpty(Request.QueryString[Common.miscellaneous.Validate]))
                {
                    //Get hex value from querystring and convert to user id
                    string hexId = Request.QueryString[Common.miscellaneous.Validate];
                    int userId = int.Parse(hexId, System.Globalization.NumberStyles.HexNumber);

                    //Instantiate variables
                    var membership = new _memberships();
                    if (membership.MakeAcctActive(userId))
                    {
                        //Valid user id.  user can log in
                        TempData["ValidatedSuccessfully"] = true;
                    }
                    else
                    {
                        //Invalid user id.  user cannot log in
                        TempData["ValidatedSuccessfully"] = false;
                    }
                }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"LoginController.cs : RenderForm()");
                Common.SaveErrorMessage(ex, sb, typeof(LoginController));

                ModelState.AddModelError("", "*An error occured while validating your account.");
            }

            //
            return PartialView("~/Views/Partials/ManageAccount/_login.cshtml");
        }


        //Log Member In
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogMemberIn(LoginModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Instantiate variables
                    var membership = new _memberships();

                    //Attempt to log member in.
                    if (membership.logMemberIn(model.LoginId.Trim(), model.Password.Trim()))
                    {
                        return RedirectToCurrentUmbracoPage();
                    }
                    else
                    {
                        ModelState.AddModelError("", "*Invalid User Id or Password.  Please try again.");
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
                sb.AppendLine(@"LoginController.cs : LogMemberIn()");
                sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                Common.SaveErrorMessage(ex, sb, typeof(LoginController));

                ModelState.AddModelError("", "An error occured while logging into your account.");
                return CurrentUmbracoPage();
            }
        }



        //Log Member Out
        [HttpPost]
        public ActionResult LogMemberOut()
        {
            try
            {
                //Instantiate variables
                var membership = new _memberships();
                membership.logMemberOut();
                TempData.Clear();
                Session.Clear();
                Session.Abandon();
                Roles.DeleteCookie();
                FormsAuthentication.SignOut();
                return RedirectToUmbracoPage(Umbraco.AssignedContentItem.Site());
                //return RedirectToCurrentUmbracoPage();
            }
            catch (Exception ex)
            {
                //Save error message to umbraco
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"LoginController.cs : LogMemberOut()");
                Common.SaveErrorMessage(ex, sb, typeof(LoginController));

                ModelState.AddModelError(null, "An error occured while logging out of your account.");
                return CurrentUmbracoPage();
            }
        }
    }
}
