using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Security;
using System.Text;
using System.Web.Security;
using Umbraco.Core.Models;
using Umbraco.Core;
using Umbraco.Core.Services;
using Models;


namespace Models
{
    public class _memberships
    {
        #region LearnMore
        //  https://our.umbraco.com/documentation/Reference/Management/Services/MemberService/
        //  https://our.umbraco.com/documentation/Reference/Querying/MemberShipHelper/
        #endregion


        #region Properties
        private MembershipHelper membershipHelper = new MembershipHelper(UmbracoContext.Current);
        #endregion


        #region Selects
        public LoginStatusModel GetCurrentLoginStatus()
        {
            return membershipHelper.GetCurrentLoginStatus();
        }
        public IPublishedContent GetCurrentMember()
        {
            return membershipHelper.GetCurrentMember();

        }
        public ProfileModel GetProfileModel()
        {
            return membershipHelper.GetCurrentMemberProfileModel();
        }
        public int GetCurrentMemberId()
        {
            return membershipHelper.GetCurrentMemberId();
        }
        public string GetCurrentMemberName()
        {
            return membershipHelper.GetCurrentMember().Name;
        }
        public string GetCurrentMembersFirstName()
        {
            try
            {
                IMember member = ApplicationContext.Current.Services.MemberService.GetById(GetCurrentMemberId());
                if (member != null)
                    return member.GetValue<string>("nodeProperties.firstName");
                else
                    return string.Empty;
            }
            catch (Exception ex)
            {
                //StringBuilder sb = new StringBuilder();
                //sb.AppendLine(@"\App_Code\linqQueries\linqMembers.vb : GetCurrentMembersFirstName()");
                //// sb.AppendLine("mediaId:" & mediaId)

                //saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
                // Return "Error: " & ex.ToString  'String.Empty
                return string.Empty;
            }
        }
        public int? getMemberId_byEmail(string _email)
        {
            // Return id
            try
            {
                IMember member = ApplicationContext.Current.Services.MemberService.GetByEmail(_email);
                return member.Id;
            }
            catch (Exception ex)
            {
                //StringBuilder sb = new StringBuilder();
                //sb.AppendLine(@"\App_Code\linqQueries\linqMembers.vb : getMemberId_byEmail()");
                //sb.AppendLine("_email:" + _email);

                //saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
                return -1;
            }
        }
        public MembershipModel getMemberModel_byEmail(string _email)
        {
            // Return id
            try
            {
                IMember member = ApplicationContext.Current.Services.MemberService.GetByEmail(_email);
                MembershipModel membership = new MembershipModel();

                membership.Email = member.Email;
                membership.ConfirmEmail = member.Email;
                membership.FirstName = member.GetValue<string>(Common.NodeProperties.firstName);
                membership.LastName = member.GetValue<string>(Common.NodeProperties.lastName);
                membership.Password = string.Empty;
                membership.ConfirmPassword = string.Empty;
                membership.ValidPassword = false;
                membership.memberId = member.Id;

                return membership;
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"_membership.cs : getMemberModel_byEmail()");
                sb.AppendLine("_email:" + _email);
                Common.SaveErrorMessage(ex, sb, typeof(_memberships));

                return null;
            }
        }
        public string getMemberName_byId(int _id)
        {
            // Return id
            try
            {
                IMember member = ApplicationContext.Current.Services.MemberService.GetById(_id);
                if (member != null)
                    return member.Name;
                else
                    return string.Empty;
            }
            catch (Exception ex)
            {
                //StringBuilder sb = new StringBuilder();
                //sb.AppendLine(@"\App_Code\linqQueries\linqMembers.vb : getMemberName_byId()");
                //sb.AppendLine("_id:" + _id);

                //saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
                return string.Empty;
            }
        }
        public string getMemberName_byGuid(Guid _id)
        {
            // Return id
            try
            {
                IMember member = ApplicationContext.Current.Services.MemberService.GetByKey(_id);
                if (member != null)
                    return member.Name;
                else
                    return string.Empty;
            }
            catch (Exception ex)
            {
                //StringBuilder sb = new StringBuilder();
                //sb.AppendLine(@"\App_Code\linqQueries\linqMembers.vb : getMemberName_byGuid()");
                //sb.AppendLine("_id:" + _id.ToString());

                //saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
                return string.Empty;
            }
        }
        public string getMemberEmail_byId(int _id)
        {
            try
            {
                IMember member = ApplicationContext.Current.Services.MemberService.GetById(_id);
                return member.Email;
            }
            catch (Exception ex)
            {
                //StringBuilder sb = new StringBuilder();
                //sb.AppendLine(@"\App_Code\linqQueries\linqMembers.vb : getMemberEmail_byId()");
                //sb.AppendLine("_id:" + _id);

                //saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
                return string.Empty;
            }
        }
        //public BusinessReturn getMemberDemographics_byId(int _id, bool _getDemographics = false, bool _getBillingInfo = false, bool _getShippingInfo = false, bool _getMemberProperties = false, bool _getPledgeProperties = false, bool _getStripeIDs = false)
        //{

        //    // 
        //    BusinessReturn BusinessReturn = new BusinessReturn();

        //    try
        //    {
        //        if (_id > 0)
        //        {
        //            // Instantiate variables
        //            IMember member = ApplicationContext.Current.Services.MemberService.GetById(_id);
        //            Member clsMember = new Member();

        //            if (!IsNothing(member))
        //            {
        //                // Obtain demographics
        //                if (_getDemographics)
        //                {
        //                    if (member.HasProperty(nodeProperties.firstName))
        //                        clsMember.Demographics.firstName = member.GetValue<string>(nodeProperties.firstName);
        //                    if (member.HasProperty(nodeProperties.lastName))
        //                        clsMember.Demographics.lastName = member.GetValue<string>(nodeProperties.lastName);
        //                    if (member.HasProperty(nodeProperties.photo) && !IsNothing(member.GetValue(nodeProperties.photo)))
        //                    {
        //                        clsMember.Demographics.photo = member.GetValue<int>(nodeProperties.photo);
        //                        clsMember.Demographics.photoUrl = getMediaURL(member.GetValue<int>(nodeProperties.photo), Crops.members);
        //                    }
        //                    else
        //                    {
        //                        clsMember.Demographics.photo = mediaNodes.defaultProfileImg;
        //                        clsMember.Demographics.photoUrl = getMediaURL(mediaNodes.defaultProfileImg, Crops.members);
        //                    }
        //                    clsMember.Demographics.briefDescription = member.GetValue<string>(nodeProperties.briefDescription);
        //                }

        //                // Obtain billing info
        //                if (_getBillingInfo)
        //                {
        //                    clsMember.BillingInfo.address01 = member.GetValue<string>(nodeProperties.address01_Billing);
        //                    clsMember.BillingInfo.address02 = member.GetValue<string>(nodeProperties.address02_Billing);
        //                    clsMember.BillingInfo.city = member.GetValue<string>(nodeProperties.city_Billing);
        //                    clsMember.BillingInfo.stateProvidence = member.GetValue<string>(nodeProperties.stateprovidence_Billing);
        //                    clsMember.BillingInfo.postalCode = member.GetValue<string>(nodeProperties.postalCode_Billing);
        //                }

        //                // Obtain shipping info
        //                if (_getShippingInfo)
        //                {
        //                    clsMember.ShippingInfo.address01 = member.GetValue<string>(nodeProperties.address01_Shipping);
        //                    clsMember.ShippingInfo.address02 = member.GetValue<string>(nodeProperties.address02_Shipping);
        //                    clsMember.ShippingInfo.city = member.GetValue<string>(nodeProperties.city_Shipping);
        //                    clsMember.ShippingInfo.stateProvidence = member.GetValue<string>(nodeProperties.stateprovidence_Shipping);
        //                    clsMember.ShippingInfo.postalCode = member.GetValue<string>(nodeProperties.postalCode_Shipping);
        //                }

        //                // Obtain member properties
        //                if (_getMemberProperties)
        //                {
        //                    clsMember.MembershipProperties.userId = _id;
        //                    clsMember.MembershipProperties.nodeName = member.Name;
        //                    clsMember.MembershipProperties.loginName = member.Username;
        //                    clsMember.MembershipProperties.email = member.Email;
        //                    clsMember.MembershipProperties.altEmail = member.GetValue<string>(nodeProperties.alternativeEmail);
        //                    clsMember.MembershipProperties.isFacebookAcct = member.GetValue<bool>(nodeProperties.isFacebookAcct);
        //                    clsMember.MembershipProperties.isLinkedInAcct = member.GetValue<bool>(nodeProperties.isLinkedInAcct);
        //                    clsMember.MembershipProperties.isTwitterAcct = member.GetValue<bool>(nodeProperties.isTwitterAcct);
        //                }

        //                // Obtain pledges
        //                if (_getPledgeProperties)
        //                {
        //                    // Obtain member's pledges as csv list
        //                    string pledges = member.GetValue<string>(nodeProperties.pledges);

        //                    if (pledges != null)
        //                    {
        //                        // Instantiate variables
        //                        // Dim lstPledges As New List(Of CampaignPledge)
        //                        List<string> pledgeIdList;

        //                        // Split list of pledge IDs
        //                        pledgeIdList = pledges.Split(",").ToList();

        //                        // Loop thru all IDs
        //                        foreach (string pledgeId in pledgeIdList)
        //                        {
        //                            // Instantiate pledge IPublishedContent
        //                            IPublishedContent pledgeNode = _uHelper.Get_IPublishedContentByID(pledgeId);

        //                            if (!IsNothing(pledgeNode))
        //                            {
        //                                if (pledgeNode.DocumentTypeAlias == docTypes.Pledges)
        //                                {
        //                                    // Instantiate new class object
        //                                    // Add data to object
        //                                    var campaignPledge = new CampaignPledge()
        //                                    {
        //                                        pledgeDate = pledgeNode.GetPropertyValue<DateTime>(nodeProperties.pledgeDate),
        //                                        pledgeAmount = pledgeNode.GetPropertyValue<decimal>(nodeProperties.pledgeAmount),
        //                                        campaignName = pledgeNode.Parent.Parent.Parent.Name,
        //                                        showAsAnonymous = pledgeNode.GetPropertyValue<bool>(nodeProperties.showAsAnonymous),
        //                                        fulfilled = pledgeNode.GetPropertyValue<bool>(nodeProperties.fulfilled),
        //                                        canceled = pledgeNode.GetPropertyValue<bool>(nodeProperties.canceled),
        //                                        transactionDeclined = pledgeNode.GetPropertyValue<bool>(nodeProperties.transactionDeclined),
        //                                        reimbursed = pledgeNode.GetPropertyValue<bool>(nodeProperties.reimbursed),
        //                                        campaignUrl = pledgeNode.Parent.Parent.Parent.Url
        //                                    };

        //                                    // Add object to class
        //                                    clsMember.PledgeList.Add(campaignPledge);
        //                                }
        //                            }
        //                        }
        //                    }
        //                }

        //                // Obtain Stripe IDs
        //                if (_getStripeIDs)
        //                {
        //                    clsMember.StripeIDs.customerId = member.GetValue<string>(nodeProperties.customerId);
        //                    // clsMember.StripeIDs.bankAcctId = member.GetValue(Of String)(nodeProperties.bankAccountId)
        //                    // clsMember.StripeIDs.bankAcctToken = member.GetValue(Of String)(nodeProperties.bankAccountToken)
        //                    // clsMember.StripeIDs.campaignAcctId = member.GetValue(Of String)(nodeProperties.campaignAccountId)
        //                    // clsMember.StripeIDs.fileUploadId = member.GetValue(Of String)(nodeProperties.fileUploadId)
        //                    clsMember.StripeIDs.creditCardId = member.GetValue<string>(nodeProperties.creditCardId);
        //                    clsMember.StripeIDs.creditCardToken = member.GetValue<string>(nodeProperties.creditCardToken);
        //                }

        //                // Return class within businessreturn
        //                BusinessReturn.DataContainer.Add(clsMember);
        //            }
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        sb.AppendLine(@"\App_Code\linqQueries\linqMembers.vb : getMemberDemographics_byId()");
        //        sb.AppendLine("_id:" + _id);
        //        sb.AppendLine("_getDemographics:" + _getDemographics);
        //        sb.AppendLine("_getBillingInfo:" + _getBillingInfo);
        //        sb.AppendLine("_getShippingInfo:" + _getShippingInfo);
        //        sb.AppendLine("_getMemberProperties:" + _getMemberProperties);
        //        sb.AppendLine("_getPledgeProperties:" + _getPledgeProperties);
        //        sb.AppendLine("_getStripeIDs:" + _getStripeIDs);

        //        saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
        //        BusinessReturn.ExceptionMessage = ex.ToString();
        //    }

        //    return BusinessReturn;
        //}
        #endregion


        #region Inserts
        public int CreateMember(string firstName, string lastName, string email, string password)
        {
            try
            {
                // Create member
                IMemberService MemberService = ApplicationContext.Current.Services.MemberService;
                IMember newMember = MemberService.CreateMemberWithIdentity(email, email, email, "Member");
                newMember.IsApproved = false;

                // Set member values
                newMember.SetValue("firstName", firstName);
                newMember.SetValue("lastName", lastName);

                // Save new member
                MemberService.Save(newMember);
                MemberService.SavePassword(newMember, password);


                //StringBuilder sb = new StringBuilder();
                //sb.AppendLine(@"Models/_membership.cs : CreateMember()");
                //sb.AppendLine("firstName:" + firstName);
                //sb.AppendLine("lastName:" + lastName);
                //sb.AppendLine("email:" + email);
                //sb.AppendLine("password:" + password);
                //sb.AppendLine("newMember.Id:" + newMember.Id);
                //Common.saveErrorMessage("Test error msg", sb.ToString());


                return newMember.Id;
            }

            catch (Exception ex)
            {
                //Save error message to umbraco
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"Models/_membership.cs : CreateMember()");
                sb.AppendLine("firstName:" + firstName);
                sb.AppendLine("lastName:" + lastName);
                sb.AppendLine("email:" + email);
                sb.AppendLine("password:" + password);
                Common.SaveErrorMessage(ex, sb, typeof(_memberships));

                return -1;
            }
        }
        #endregion


        #region Updates
        //public void Update(Member _member)
        //{
        //    // Instantiate variables
        //    //BusinessReturn ValidationReturn = new BusinessReturn();
        //    IMember member;

        //    try
        //    {
        //        // Obtain current member
        //        member = ApplicationContext.Current.Services.MemberService.GetById(_member.MembershipProperties.userId);

        //        // Set values of member
        //        member.SetValue(nodeProperties.firstName, _member.Demographics.firstName);
        //        member.SetValue(nodeProperties.lastName, _member.Demographics.lastName);
        //        member.SetValue(nodeProperties.briefDescription, _member.Demographics.briefDescription);
        //        // member.SetValue(nodeProperties._umb_email, _member.MembershipProperties.email)
        //        if (!string.IsNullOrEmpty(_member.MembershipProperties.email))
        //            member.Email = _member.MembershipProperties.email;
        //        if (!string.IsNullOrEmpty(_member.MembershipProperties.loginName))
        //            member.Username = _member.MembershipProperties.loginName;
        //        member.SetValue(nodeProperties.alternativeEmail, _member.MembershipProperties.altEmail);

        //        member.SetValue(nodeProperties.address01_Billing, _member.BillingInfo.address01);
        //        member.SetValue(nodeProperties.address02_Billing, _member.BillingInfo.address02);
        //        member.SetValue(nodeProperties.city_Billing, _member.BillingInfo.city);
        //        member.SetValue(nodeProperties.stateprovidence_Billing, _member.BillingInfo.stateProvidence);
        //        member.SetValue(nodeProperties.postalCode_Billing, _member.BillingInfo.postalCode);

        //        member.SetValue(nodeProperties.address01_Shipping, _member.ShippingInfo.address01);
        //        member.SetValue(nodeProperties.address02_Shipping, _member.ShippingInfo.address02);
        //        member.SetValue(nodeProperties.city_Shipping, _member.ShippingInfo.city);
        //        member.SetValue(nodeProperties.stateprovidence_Shipping, _member.ShippingInfo.stateProvidence);
        //        member.SetValue(nodeProperties.postalCode_Shipping, _member.ShippingInfo.postalCode);

        //        // Save data to member.
        //        ApplicationContext.Current.Services.MemberService.Save(member);

        //        // 
        //        if (!string.IsNullOrEmpty(_member.MembershipProperties.password))
        //        {
        //            // Create memberservice
        //            IMemberService MemberService = ApplicationContext.Current.Services.MemberService;
        //            MemberService.SavePassword(member, _member.MembershipProperties.password);
        //            // Save member's password
        //            MemberService.Save(member);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        sb.AppendLine(@"\App_Code\linqQueries\linqMembers.vb : Update()");
        //        sb.AppendLine("_member:" + _member.ToString());

        //        saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
        //        ValidationReturn.ExceptionMessage = ex.ToString();
        //    }

        //    return ValidationReturn;
        //}
        //public BusinessReturn updatePhoto(int _memberId, int _imediaId)
        //{
        //    // Instantiate variables
        //    BusinessReturn ValidationReturn = new BusinessReturn();
        //    IMember member;

        //    try
        //    {
        //        // Obtain member
        //        member = ApplicationContext.Current.Services.MemberService.GetById(_memberId);

        //        // Set values of member
        //        member.SetValue(nodeProperties.photo, _imediaId);

        //        // Save data to member.
        //        ApplicationContext.Current.Services.MemberService.Save(member);
        //    }
        //    catch (Exception ex)
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        sb.AppendLine(@"\App_Code\linqQueries\linqMembers.vb : updatePhoto()");
        //        sb.AppendLine("_memberId:" + _memberId);

        //        saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
        //        ValidationReturn.ExceptionMessage = ex.ToString();
        //    }

        //    return ValidationReturn;
        //}
        //public BusinessReturn UpdateReviews(int _memberId, int _reviewId)
        //{
        //    // Instantiate variables
        //    BusinessReturn ValidationReturn = new BusinessReturn();
        //    IMember member;
        //    // Dim _HoldReviewIDs As String = String.Empty
        //    string locaUdi = string.Empty;

        //    try
        //    {
        //        // Obtain current member
        //        var IPublishedContent = ApplicationContext.Current.Services.ContentService.GetById(_reviewId);
        //        locaUdi = Udi.Create(Constants.UdiEntityType.Document, IPublishedContent.Key).ToString();
        //        member = ApplicationContext.Current.Services.MemberService.GetById(_memberId);
        //        if (member.GetValue(nodeProperties.reviews) != null)
        //            locaUdi = member.GetValue(nodeProperties.reviews).ToString() + "," + locaUdi;
        //        member.SetValue(nodeProperties.reviews, locaUdi.ToString());

        //        // Save data to member.
        //        ApplicationContext.Current.Services.MemberService.Save(member);

        //        return ValidationReturn;
        //    }
        //    catch (Exception ex)
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        sb.AppendLine("linqMembers.vb : UpdateReviews()");
        //        sb.AppendLine("_memberId: " + _memberId);
        //        sb.AppendLine("_reviewId: " + _reviewId);

        //        saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
        //        ValidationReturn.ExceptionMessage = ex.ToString();

        //        return ValidationReturn;
        //    }
        //}
        public bool MakeAcctActive(int memberId)
        {
            // Instantiate variables
            IMember member;

            try
            {
                //Obtain member by Id
                member = ApplicationContext.Current.Services.MemberService.GetById(memberId);

                //Set member as approved to log in
                member.IsApproved = true;

                //Save data to member.
                ApplicationContext.Current.Services.MemberService.Save(member);


                return true;
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("_memberships.cs : MakeAcctActive()");
                sb.AppendLine("memberId:" + memberId);
                Common.SaveErrorMessage(ex, sb, typeof(_memberships));

                return false;
            }
        }
        #endregion


        #region Deletes
        #endregion


        #region Log In/Out
        public bool isMemberLoggedIn()
        {
            return membershipHelper.IsLoggedIn();
        }
        public bool logMemberIn(string loginId, string password)
        {
            try
            {
                if (membershipHelper.Login(loginId, password))
                {
                    // Set cookie
                    FormsAuthentication.SetAuthCookie(loginId, false);

                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                //Save error message to umbraco
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"Models/_membership.cs : logMemberIn()");
                sb.AppendLine("loginId:" + loginId);
                sb.AppendLine("password:" + password);
                Common.SaveErrorMessage(ex, sb, typeof(_memberships));

                return false;
            }
        }
        public bool externallogMemberIn(string _userName)
        {
            try
            {
                FormsAuthentication.SetAuthCookie(_userName, false);
                return true;
            }
            catch (Exception ex)
            {
                //saveErrorMessage(getLoggedInMember, ex.ToString(), @"\App_Code\linqQueries\linqMembers.vb : externallogMemberIn()");
                return false;
            }
        }
        public void logMemberOut()
        {
            // Log member out
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
            Roles.DeleteCookie();
            FormsAuthentication.SignOut();
        }
        #endregion


        #region Methods
        public bool doesMemberExist_byUserId(string _userId)
        {
            // Return if exists
            return ApplicationContext.Current.Services.MemberService.Exists(_userId);
        }
        public bool DoesMemberExist_byEmail(string _email)
        {
            // Return if exists
            IMember member = ApplicationContext.Current.Services.MemberService.GetByEmail(_email);
            return (member != null);
        }
        #endregion
    }
}