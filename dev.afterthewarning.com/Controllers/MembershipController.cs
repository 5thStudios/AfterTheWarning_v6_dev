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
                Common.saveErrorMessage(ex.ToString(), sb.ToString());

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
                Common.saveErrorMessage(ex.ToString(), sb.ToString());

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
                Common.saveErrorMessage(ex.ToString(), sb.ToString());

                ModelState.AddModelError(null, "An error occured while updating your account.");
                return CurrentUmbracoPage();
            }
        }
        #endregion

        #region "Methods"
        public static string SendUpdatesByEmail()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                //Instantiate variables
                UmbracoHelper umbHelper = new UmbracoHelper(UmbracoContext.Current);
                IMemberService memberService = ApplicationContext.Current.Services.MemberService;
                IEnumerable<IMember> members = memberService.GetAllMembers();
                Boolean isFirst = true;
                //string visionary = "";

                List<latestUpdates> lstLatestUpdates = Controllers.MessageController.ObtainLatestMessages();
                //sb.Append(Newtonsoft.Json.JsonConvert.SerializeObject(lstLatestUpdates));

                string latestUpdates = "<h1 class='date'  style='color:#74010a;font-family:'Helvetica','Arial',sans-serif;font-size:40px;font-weight:400;line-height:1.3;margin:0;padding:0;text-align:center;word-break:normal;'>[DATE]</h1>";
                string strVisionary = "<br /><br /><div class='name' style='font-size:18px;font-weight:900;text-align:center;'><a href='[HREF]' style='color: #f3a42a; text-decoration: none;'>[VISIONARY]</a></div>";
                string strMsg = "<div class='title' style='font-size:20px;text-align:center;'><a href='[HREF]'><span class='cross' style='color: #f3a42a; text-decoration: none;'>&#x271E; </span>[TITLE]</a></div>";


                //<h1 class="date" style="color: #74010a; font-family: 'Helvetica','Arial',sans-serif; font-size: 40px; font-weight: 400; line-height: 1.3; margin: 0; padding: 0; text-align: center; word-break: normal;">September 03, 2019</h1>
                //<br />
                //<br />
                //<div class="name" style="font-size: 18px; font-weight: 900; text-align: center;"><a href="/" style="color: #f3a42a; text-decoration: none;">Children of the Renewal</a></div>
                //<div class="title" style="font-size: 20px; text-align: center;"><a href="/" style="color: #f3a42a; text-decoration: none;"><span class="cross">✞  </span>Recent Messages</a></div>
                //<br />
                //<br /> 



                foreach (latestUpdates latestUpdate in lstLatestUpdates)
                {
                    if (isFirst)
                    {
                        sb.AppendLine(latestUpdates.Replace("[DATE]", latestUpdate.datePublished.ToString("MMMM d, yyyy")));
                        isFirst = false;
                    }
                    foreach (visionary visionary in latestUpdate.lstVisionaries)
                    {
                        sb.AppendLine(strVisionary.Replace("[HREF]", visionary.url).Replace("[VISIONARY]", visionary.name));

                        foreach (message msg in visionary.lstMessages)
                        {
                            sb.AppendLine(strMsg.Replace("[HREF]", msg.url).Replace("[TITLE]", msg.title));
                        }
                    }
                }




                // member.Email
                string tempEmail = "jim.fifth@5thstudios.com";
                string hostUrl = System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Host + "/";

                // set the content by openning the files.
                //string filePath_html = HostingEnvironment.MapPath("~/Emails/RecentUpdates/RecentUpdates-uncompressed.html");
                string filePath_html = HostingEnvironment.MapPath("~/Emails/RecentUpdates/RecentUpdates.html");
                //string filePath_Text = HostingEnvironment.MapPath("~/Emails/ContactUs/ContactUs.txt");
                string emailBody_Html = System.IO.File.ReadAllText(filePath_html);
                //string emailBody_Text = System.IO.File.ReadAllText(filePath_Text);

                // Insert data into page
                emailBody_Html = emailBody_Html.Replace("[INFO]", sb.ToString());
                emailBody_Html = emailBody_Html.Replace("[AFTERTHEWARNING_URL]", hostUrl);




                emailBody_Html = emailBody_Html.Replace("[INFO]", sb.ToString());
                emailBody_Html = emailBody_Html.Replace("[YEAR]", DateTime.Today.Year.ToString());




                //Loop through each member                
                foreach (IMember member in members)
                {


                    SmtpClient smtp = new SmtpClient();


                    //Obtain list of email addresses for forms
                    //IPublishedContent ipHome = Umbraco.TypedContent((int)Common.SiteNode.Home);
                    //List<string> lstEmails = (ipHome.GetPropertyValue<string[]>(Common.NodeProperty.FormEmails)).ToList();
                    //IPublishedContent ipPg = Umbraco.TypedContent((int)model.PgId);
                    //List<string> lstEmails = (ipPg.GetPropertyValue<string[]>(Common.NodeProperty.FormEmails)).ToList();


                    // Create mail message
                    //MailMessage Msg = new MailMessage() { From = new MailAddress(smtpUsername) };
                    //MailMessage Msg = new MailMessage("support@5thstudios.com", "jim.fifth@5thstudios.com");
                    MailMessage Msg = new MailMessage();
                    Msg.To.Add(new MailAddress(tempEmail));
                    Msg.To.Add(new MailAddress("fifthamy@gmail.com"));
                    //foreach (string email in lstEmails)
                    //{
                    //    if (!string.IsNullOrWhiteSpace(email))
                    //    {
                    //        Msg.To.Add(new MailAddress(email));
                    //    }
                    //}
                    Msg.BodyEncoding = Encoding.UTF8;
                    Msg.SubjectEncoding = Encoding.UTF8;
                    Msg.Subject = "Recent Updates | After the Warning";
                    Msg.IsBodyHtml = true;
                    Msg.Body = "";
                    Msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(emailBody_Html, new System.Net.Mime.ContentType(MediaTypeNames.Text.Html)));
                    //Msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(emailBody_Text, new System.Net.Mime.ContentType(MediaTypeNames.Text.Plain)));


                    // Send email
                    smtp.Send(Msg);
                    smtp.ServicePoint.CloseConnectionGroup(smtp.ServicePoint.ConnectionName);



                    //End after test
                    break;
                }


            }
            catch (Exception ex)
            {
                //Save error message to umbraco
                StringBuilder sb2 = new StringBuilder();
                sb2.AppendLine(@"MembershipController.cs : SendUpdatesByEmail()");
                //sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                Common.saveErrorMessage(ex.ToString(), sb2.ToString());

                return ex.ToString();
            }

            return sb.ToString();
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
                Common.saveErrorMessage(ex.ToString(), sb.ToString());
            }

        }
        #endregion
    }
}
