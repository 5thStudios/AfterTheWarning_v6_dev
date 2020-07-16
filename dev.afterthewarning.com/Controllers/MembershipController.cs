using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Services;
using System.Net.Mail;
using System.Net;
using System.Web.Hosting;
using Umbraco.Web;
using System.Net.Mime;
using System.Collections.Specialized;
using Umbraco.Web.Security;
using umbraco.editorControls.SettingControls.Pickers;

namespace Controllers
{
    public class MembershipController : SurfaceController
    {
        #region "Renders"
        public ActionResult RenderForm()
        {
            return PartialView("~/Views/Partials/ManageAccount/_createAcct.cshtml");
        }
        public ActionResult RenderFormWithData(string loginId)
        {
            try
            {
                _memberships memberships = new _memberships();
                MembershipModel model = memberships.getMemberModel_byEmail(loginId);
                return PartialView("~/Views/Partials/ManageAccount/_editAcct.cshtml", model);
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"MembershipController.cs : RenderFormWithData()");
                sb.AppendLine("loginId:" + loginId);
                Common.SaveErrorMessage(ex, sb, typeof(MembershipController));

                ModelState.AddModelError("", "*An error occured while displaying a form with the user information.");
                return PartialView("~/Views/Partials/ManageAccount/_editAcct.cshtml");
            }
        }
        #endregion

        #region "HttpPosts"
        //Submit form to create a new account
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm_CreateMember(MembershipModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Instantiate variables
                    var membership = new _memberships();

                    //Does member already exist?
                    if (membership.DoesMemberExist_byEmail(model.Email.Trim()))
                    {
                        //Member exists.
                        ModelState.AddModelError("Email", "*Member already exists");
                        return CurrentUmbracoPage();
                    }
                    else
                    {
                        //Submit data to create membership
                        int memberId = membership.CreateMember(model.FirstName.Trim(), model.LastName.Trim(), model.Email.Trim(), model.Password);

                        if (memberId > 0)
                        {
                            //Log member in
                            //membership.logMemberIn(model.Email.Trim(), model.Password);

                            //Email user with verification link.
                            SendVerificationEmail(model, memberId);

                            //Return to page
                            TempData["CreatedSuccessfully"] = true;
                            return RedirectToCurrentUmbracoPage();
                        }
                        else
                        {
                            //
                            ModelState.AddModelError(null, "An error occured while creating your account.");
                            return CurrentUmbracoPage();
                        }
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
                sb.AppendLine(@"MembershipController.cs : CreateMember()");
                sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                Common.SaveErrorMessage(ex, sb, typeof(MembershipController));

                ModelState.AddModelError(null, "An error occured while creating your account.");
                return CurrentUmbracoPage();
            }
        }


        //Submit form to create a new account
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm_UpdateMember(MembershipModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //
                    //IMember member;
                    _memberships membershipHelper = new _memberships();

                    // Obtain current member
                    int? memberId = membershipHelper.getMemberId_byEmail(model.Email);
                    if (memberId != null)
                    {
                        // Create new instance of "MemberService"
                        var memberService = Services.MemberService;

                        // Expose the custom properties for the member
                        var member = memberService.GetByEmail(model.Email);

                        //Update password
                        memberService.SavePassword(member, model.Password);

                        // Update the member properties
                        member.SetValue(Common.NodeProperties.firstName, model.FirstName);
                        member.SetValue(Common.NodeProperties.lastName, model.LastName.ToUpper());

                        // Save the object
                        memberService.Save(member);

                        TempData["UpdatedSuccessfully"] = true;
                        return RedirectToCurrentUmbracoPage();
                    }

                    ModelState.AddModelError(null, "An error occured while updating your account.");
                    return CurrentUmbracoPage();
                }
                else
                {
                    ModelState.AddModelError(null, "An error occured while updating your account.");
                    return CurrentUmbracoPage();
                }
            }
            catch (Exception ex)
            {
                //Save error message to umbraco
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"MembershipController.cs : UpdateMember()");
                sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                Common.SaveErrorMessage(ex, sb, typeof(MembershipController));

                ModelState.AddModelError(null, "An error occured while updating your account.");
                return CurrentUmbracoPage();
            }
        }
        #endregion

        #region "Methods"
        public static Dictionary<string, int> SendUpdatesByEmail()
        {
            //Scope variables
            Dictionary<string, int> dict = new Dictionary<string, int>();
            List<latestUpdates> lstLatestUpdates = new List<latestUpdates>();
            StringBuilder sbHtmlList = new StringBuilder();
            StringBuilder sbTextList = new StringBuilder();
            string emailBody_Html = string.Empty;
            string emailBody_Text = string.Empty;
            Boolean errored = false;
            int totalSuccessful = 0;
            int totalFailed = 0;
            DateTime datePublished = DateTime.Today;


            //Obtain list of most recent messages
            try
            {
                lstLatestUpdates = Controllers.MessageController.ObtainLatestMessages();
            }
            catch (Exception ex)
            {
                //Save error message to umbraco
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"MembershipController.cs : SendUpdatesByEmail()");
                sb.AppendLine("Error retrieving latest messages.");
                Common.SaveErrorMessage(ex, sb, typeof(MembershipController));

                errored = true;
            }


            //Convert List to Html
            if (!errored)
            {
                try
                {
                    //Instantiate variables
                    Boolean isFirst = true;
                    string lastVisionary = "";
                    const string strVisionary = "<br /><br /><div class='name' style='font-size:18px;font-weight:900;text-align:center;'><a href='[HREF]' style='color: #f3a42a; text-decoration: none;'>[VISIONARY]</a></div>";
                    const string strMsg = "<div class='title' style='font-size:20px;text-align:center;'><a href='[HREF]' style='color: #f3a42a; text-decoration: none;'><span class='cross' style='color: #f3a42a; text-decoration: none;'>&#x271E; </span>[TITLE]</a></div>";


                    //Convert list to html
                    foreach (latestUpdates latestUpdate in lstLatestUpdates)
                    {
                        if (isFirst)
                        {
                            //Obtain date published
                            datePublished = latestUpdate.datePublished;
                            isFirst = false;
                        }

                        foreach (visionary visionary in latestUpdate.lstVisionaries)
                        {

                            if (lastVisionary != visionary.name)
                            {
                                sbHtmlList.AppendLine(strVisionary.Replace("[HREF]", visionary.url).Replace("[VISIONARY]", visionary.name));
                                lastVisionary = visionary.name;
                            }

                            foreach (message msg in visionary.lstMessages)
                            {
                                sbHtmlList.AppendLine(strMsg.Replace("[HREF]", msg.url).Replace("[TITLE]", msg.title));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Save error message to umbraco
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(@"MembershipController.cs : SendUpdatesByEmail()");
                    sb.AppendLine("Error converting list to html:");
                    sb.AppendLine(Newtonsoft.Json.JsonConvert.SerializeObject(lstLatestUpdates));
                    Common.SaveErrorMessage(ex, sb, typeof(MembershipController));

                    errored = true;
                }
            }


            //Convert List to Text
            if (!errored)
            {
                try
                {
                    //Instantiate variables
                    Boolean isFirst = true;
                    string lastVisionary = "";

                    //Convert list to html
                    foreach (latestUpdates latestUpdate in lstLatestUpdates)
                    {
                        foreach (visionary visionary in latestUpdate.lstVisionaries)
                        {
                            if (lastVisionary != visionary.name)
                            {
                                sbTextList.AppendLine(" ");
                                sbTextList.AppendLine(" ");
                                sbTextList.AppendLine(visionary.name);
                                lastVisionary = visionary.name;
                            }

                            foreach (message msg in visionary.lstMessages)
                            {
                                sbTextList.AppendLine(msg.title + "  |  " + msg.url);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Save error message to umbraco
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(@"MembershipController.cs : SendUpdatesByEmail()");
                    sb.AppendLine("Error converting list to text:");
                    sb.AppendLine(Newtonsoft.Json.JsonConvert.SerializeObject(lstLatestUpdates));
                    Common.SaveErrorMessage(ex, sb, typeof(MembershipController));

                    errored = true;
                }
            }


            //Create email files
            if (!errored)
            {
                try
                {
                    //Obtain host url
                    string hostUrl = System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Host + "/";

                    //Extract templates from email files
                    //string filePath_html = HostingEnvironment.MapPath("~/Emails/RecentUpdates/RecentUpdates-uncompressed.html");
                    string filePath_html = HostingEnvironment.MapPath("~/Emails/RecentUpdates/RecentUpdates.html");
                    string filePath_Text = HostingEnvironment.MapPath("~/Emails/RecentUpdates/RecentUpdates.txt");
                    emailBody_Html = System.IO.File.ReadAllText(filePath_html);
                    emailBody_Text = System.IO.File.ReadAllText(filePath_Text);

                    // Insert data into page
                    emailBody_Html = emailBody_Html.Replace("[AFTERTHEWARNING_URL]", hostUrl);
                    emailBody_Html = emailBody_Html.Replace("[INFO]", sbHtmlList.ToString());
                    emailBody_Html = emailBody_Html.Replace("[DATE]", datePublished.ToString("MMMM d, yyyy"));
                    emailBody_Html = emailBody_Html.Replace("[YEAR]", DateTime.Today.Year.ToString());

                    emailBody_Text = emailBody_Text.Replace("[AFTERTHEWARNING_URL]", hostUrl);
                    emailBody_Text = emailBody_Text.Replace("[INFO]", sbTextList.ToString());
                    emailBody_Text = emailBody_Text.Replace("[DATE]", datePublished.ToString("MMMM d, yyyy"));
                    emailBody_Text = emailBody_Text.Replace("[YEAR]", DateTime.Today.Year.ToString());
                }
                catch (Exception ex)
                {
                    //Save error message to umbraco
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(@"MembershipController.cs : SendUpdatesByEmail()");
                    sb.AppendLine("Error creating email files");
                    Common.SaveErrorMessage(ex, sb, typeof(MembershipController));

                    errored = true;
                }
            }


            //Send email files to each member via email.
            if (!errored)
            {
                try
                {
                    //Instantiate variables
                    UmbracoHelper umbHelper = new UmbracoHelper(UmbracoContext.Current);
                    IMemberService memberService = ApplicationContext.Current.Services.MemberService;
                    IEnumerable<IMember> members = memberService.GetAllMembers();
                    SmtpClient smtp = new SmtpClient();
                    MailMessage Msg = new MailMessage();



                    ////CREATE TEST MEMBER LIST
                    //IMember tempMember1 = memberService.GetByEmail("jim.fifth@5thstudios.com");
                    //IMember tempMember2 = memberService.GetByEmail("jfifth1976@gmail.com");
                    //List<IMember> members = new List<IMember>();
                    //members.Add(tempMember1);
                    //members.Add(tempMember2);


                    //Create email message
                    Msg.BodyEncoding = Encoding.UTF8;
                    Msg.SubjectEncoding = Encoding.UTF8;
                    Msg.Subject = "Recent Updates | After the Warning";
                    Msg.IsBodyHtml = true;
                    Msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(emailBody_Text, Encoding.UTF8, MediaTypeNames.Text.Plain));
                    Msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(emailBody_Html, Encoding.UTF8, MediaTypeNames.Text.Html));


                    //Loop through each member                
                    foreach (IMember member in members)
                    {
                        try
                        {
                            //Clear email list and add new member email
                            Msg.To.Clear();
                            Msg.To.Add(new MailAddress(member.Email));

                            // Send email
                            smtp.Send(Msg);

                            //Increment successful tally
                            totalSuccessful++;
                        }
                        catch (Exception ex)
                        {
                            //Save error message to umbraco
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(@"MembershipController.cs : SendUpdatesByEmail()");
                            sb.AppendLine(@"Error sending email to:" + member.Name);
                            Common.SaveErrorMessage(ex, sb, typeof(MembershipController));

                            //Increment failed tally
                            totalFailed++;
                        }
                    }

                    //Close connection after emails have been sent.
                    smtp.ServicePoint.CloseConnectionGroup(smtp.ServicePoint.ConnectionName);
                }
                catch (Exception ex)
                {
                    //Save error message to umbraco
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(@"MembershipController.cs : SendUpdatesByEmail()");
                    sb.AppendLine(@"Error creating smtpClient and/or list of members.");
                    Common.SaveErrorMessage(ex, sb, typeof(MembershipController));
                }
            }


            //Return results
            dict.Add("successful", totalSuccessful);
            dict.Add("failed", totalFailed);
            return dict;
        }


        private void SendVerificationEmail(MembershipModel model, int memberId)
        {
            try
            {
                //Create Url Links
                var urlHome = Umbraco.TypedContent((int)Common.siteNode.Home).UrlAbsolute();
                var urlCreateAcct = Umbraco.TypedContent((int)Common.siteNode.Login).UrlAbsolute() + "?" + Common.miscellaneous.Validate + "=" + memberId.ToString("X");

                // Obtain smtp
                string smtpHost = System.Configuration.ConfigurationManager.AppSettings["smtpHost"].ToString();
                string smtpPort = System.Configuration.ConfigurationManager.AppSettings["smtpPort"].ToString();
                string smtpUsername = System.Configuration.ConfigurationManager.AppSettings["smtpUsername"].ToString();
                string smtpPassword = System.Configuration.ConfigurationManager.AppSettings["smtpPassword"].ToString();

                //Create a new smtp client
                SmtpClient smtp = new SmtpClient(smtpHost, Convert.ToInt32(smtpPort))
                {
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword)
                };

                // set the content by openning the files.
                string filePath_html = HostingEnvironment.MapPath("~/Emails/VerifyAccount/VerifyAcct.html");
                string filePath_Text = HostingEnvironment.MapPath("~/Emails/VerifyAccount/VerifyAcct.txt");
                string emailBody_Html_original = System.IO.File.ReadAllText(filePath_html);
                string emailBody_Text_original = System.IO.File.ReadAllText(filePath_Text);

                // Create new version of files
                string emailBody_Html = emailBody_Html_original;
                string emailBody_Text = emailBody_Text_original;

                // Insert data into page
                emailBody_Html = emailBody_Html.Replace("[LINK]", urlCreateAcct);
                emailBody_Text = emailBody_Text.Replace("[LINK]", urlCreateAcct);

                emailBody_Html = emailBody_Html.Replace("[YEAR]", DateTime.Today.Year.ToString());
                emailBody_Text = emailBody_Text.Replace("[YEAR]", DateTime.Today.Year.ToString());

                emailBody_Html = emailBody_Html.Replace("[AFTERTHEWARNING_URL]", urlHome);
                emailBody_Text = emailBody_Text.Replace("[AFTERTHEWARNING_URL]", urlHome);

                emailBody_Html = emailBody_Html.Replace("[5THSTUDIOS_URL]", "http://5thstudios.com");
                emailBody_Text = emailBody_Text.Replace("[5THSTUDIOS_URL]", "http://5thstudios.com");

                // Create mail message
                MailMessage Msg = new MailMessage() { From = new MailAddress(smtpUsername) };
                Msg.To.Add(new MailAddress(model.Email));

                // Set email parameters
                Msg.BodyEncoding = Encoding.UTF8;
                Msg.SubjectEncoding = Encoding.UTF8;
                Msg.Subject = "Account Verification | After the Warning";
                Msg.IsBodyHtml = true;
                Msg.Body = "";

                AlternateView alternateHtml = AlternateView.CreateAlternateViewFromString(emailBody_Html, new System.Net.Mime.ContentType(MediaTypeNames.Text.Html));
                AlternateView alternateText = AlternateView.CreateAlternateViewFromString(emailBody_Text, new System.Net.Mime.ContentType(MediaTypeNames.Text.Plain));

                Msg.AlternateViews.Add(alternateText);
                Msg.AlternateViews.Add(alternateHtml);

                // Send email
                smtp.Send(Msg);
            }
            catch (Exception ex)
            {
                //Save error message to umbraco
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"MembershipController.cs : SendVerificationEmail()");
                sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                Common.SaveErrorMessage(ex, sb, typeof(MembershipController));
            }

        }
        #endregion
    }
}
