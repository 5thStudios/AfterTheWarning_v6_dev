using Models;
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web.Hosting;
using System.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Mvc;


namespace Controllers
{
    public class ForgotPasswordController : SurfaceController
    {

        //Render Form
        public ActionResult RenderForm()
        {
            return PartialView("~/Views/Partials/ManageAccount/_forgotPassword.cshtml");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ForgotPasswordModel model)
        {
            try
            {                
                if (ModelState.IsValid)
                {
                    //Attempt to obtain member by email or user name
                    IMember member = Services.MemberService.GetByEmail(model.Email);
                    if (member == null)
                        member = Services.MemberService.GetByUsername(model.Email);

                    //String Msg = "";

                    if (member != null)
                    {
                        string email = member.Email;
                        if (!string.IsNullOrEmpty(email))
                        {
                            //Instantiate login to obtain url
                            IPublishedContent ipLogin = Umbraco.TypedContent((int)(Common.siteNode.Login));

                            //Generate random password and save to member.
                            string passwd = System.Web.Security.Membership.GeneratePassword(6, 0);
                            Services.MemberService.SavePassword(member, passwd);
                            
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
                            string filePath_html = HostingEnvironment.MapPath("~/Emails/ResetPassword/ResetPassword.html");
                            string filePath_Text = HostingEnvironment.MapPath("~/Emails/ResetPassword/ResetPassword.txt");
                            string emailBody_Html_original = System.IO.File.ReadAllText(filePath_html);
                            string emailBody_Text_original = System.IO.File.ReadAllText(filePath_Text);
                            
                            // Create new version of files
                            string emailBody_Html = emailBody_Html_original;
                            string emailBody_Text = emailBody_Text_original;

                            // Insert data into page
                            emailBody_Html = emailBody_Html.Replace("[PASSWORD]", passwd);
                            emailBody_Text = emailBody_Text.Replace("[PASSWORD]", passwd);

                            emailBody_Html = emailBody_Html.Replace("[LINK]", ipLogin.UrlAbsolute());
                            emailBody_Text = emailBody_Text.Replace("[LINK]", ipLogin.UrlAbsolute());

                            emailBody_Html = emailBody_Html.Replace("[YEAR]", DateTime.Today.Year.ToString());
                            emailBody_Text = emailBody_Text.Replace("[YEAR]", DateTime.Today.Year.ToString());

                            // Create mail message
                            MailMessage Msg = new MailMessage() { From = new MailAddress(smtpUsername) };
                            Msg.To.Add(new MailAddress(model.Email));

                            // Set email parameters
                            Msg.BodyEncoding = Encoding.UTF8;
                            Msg.SubjectEncoding = Encoding.UTF8;
                            Msg.Subject = "My Window of Opportunity- Reset Password";
                            Msg.IsBodyHtml = true;
                            Msg.Body = "";

                            AlternateView alternateHtml = AlternateView.CreateAlternateViewFromString(emailBody_Html, new System.Net.Mime.ContentType(MediaTypeNames.Text.Html));
                            AlternateView alternateText = AlternateView.CreateAlternateViewFromString(emailBody_Text, new System.Net.Mime.ContentType(MediaTypeNames.Text.Plain));

                            Msg.AlternateViews.Add(alternateText);
                            Msg.AlternateViews.Add(alternateHtml);

                            // Send email
                            smtp.Send(Msg);

                            //=====================================================================================================









                            //
                            TempData.Add("PwResetSuccessfully", true);

                            //
                            return RedirectToCurrentUmbracoPage();
                        }
                        else
                        {
                            //
                            ModelState.AddModelError("", "*Error msg to go here");
                            return CurrentUmbracoPage();
                        }
                    }
                    else
                    {
                        //
                        ModelState.AddModelError("", "*This email address is not associated with an account.");
                        return CurrentUmbracoPage();
                    }
                }
                else
                {
                    //The modelstate is invalid.
                    ModelState.AddModelError("", "*An error occured.  Please refresh this page and try again.");
                    return CurrentUmbracoPage();
                }
            }
            catch (Exception ex)
            {
                //Save error message to umbraco
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"ForgotPasswordController.cs : ResetPassword()");
                sb.AppendLine("model:" + Newtonsoft.Json.JsonConvert.SerializeObject(model));
                Common.saveErrorMessage(ex.ToString(), sb.ToString());

                ModelState.AddModelError("", "*An error occured while resetting the password.  We will look into this issue to correct it.  Please refresh the page and try again.  Or contact us for further assistance.");
                return CurrentUmbracoPage();
            }
        }

    }
}
