using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using umbraco;
using umbraco.Core;
using umbraco.Core.Models;
using umbraco.Core.Services;
using Common;
using System.Web.HttpContext;
using Stripe;
using Newtonsoft.Json;
using umbraco.Web;
using Umbraco.Core.Models;

public class _membersFromMWoO
{
    #region Properties
    private linq2SqlDataContext linq2Db = new linq2SqlDataContext(ConfigurationManager.ConnectionStrings("umbracoDbDSN").ToString);
    // Umbraco Membership Helper classes | https://our.umbraco.org/documentation/Reference/Querying/MemberShipHelper/
    private umbraco.Web.Security.MembershipHelper memberShipHelper = new umbraco.Web.Security.MembershipHelper(umbraco.Web.UmbracoContext.Current);
    private Uhelper _uHelper = new Uhelper();
    #endregion


    #region Selects
    public System.Web.Models.LoginStatusModel getCurrentLoginStatus()
    {
        return memberShipHelper.GetCurrentLoginStatus();
    }
    public IPublishedContent GetCurrentMember()
    {
        return memberShipHelper.GetCurrentMember;
    }
    public int? GetCurrentMemberId()
    {
        return memberShipHelper.GetCurrentMemberId();
    }
    public string GetCurrentMemberName()
    {
        return memberShipHelper.GetCurrentMember.Name;
    }
    public string GetCurrentMembersFirstName()
    {
        try
        {
            // Dim member As IMember = ApplicationContext.Current.Services.MemberService.GetById(memberShipHelper.GetCurrentMember.Id)
            IMember member = ApplicationContext.Current.Services.MemberService.GetById(GetCurrentMemberId());
            if (!IsNothing(member))
                return member.GetValue<string>(nodeProperties.firstName);
            else
                return string.Empty;
        }
        catch (Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"\App_Code\linqQueries\linqMembers.vb : GetCurrentMembersFirstName()");
            // sb.AppendLine("mediaId:" & mediaId)

            saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
            // Return "Error: " & ex.ToString  'String.Empty
            return string.Empty;
        }
    }
    public string GetCurrentMemberRole(int _campaignId)
    {
        try
        {
            // Instantiate variables
            IPublishedContent campaignNode = _uHelper.Get_IPublishedContentByID(_campaignId);
            if (!IsNothing(campaignNode))
            {
                IPublishedContent campaignMemberFolder;
                int currentMemberId = GetCurrentMemberId();
                List<string> lstTeamAdministrators = new List<string>();

                if (campaignNode.Parent.HasValue(nodeProperties.teamAdministrators))
                {
                    foreach (string id in campaignNode.Parent.GetPropertyValue<string>(nodeProperties.teamAdministrators).Split(","))
                        lstTeamAdministrators.Add(id);

                    // Determine if member is a team admin
                    if (lstTeamAdministrators.Contains(currentMemberId.ToString()))
                        return memberRole.TeamAdministrator;
                    else
                    {
                        // Obtain campaign's member folder
                        campaignMemberFolder = getCampaignMemberFolder(campaignNode);
                        if (string.IsNullOrEmpty(campaignMemberFolder.Name))
                            // No campaign member folder exists.
                            return string.Empty;
                        else
                        {
                            // Loop thru member nodes
                            foreach (IPublishedContent childNode in campaignMemberFolder.Children)
                            {
                                // Determine if childnode = current member
                                if (childNode.HasProperty(nodeProperties.campaignMember))
                                {
                                    if (childNode.GetPropertyValue<string>(nodeProperties.campaignMember) == currentMemberId)
                                    {
                                        // Return wether user is a member or admin
                                        if (childNode.GetPropertyValue<bool>(nodeProperties.campaignManager) == true)
                                            return memberRole.CampaignAdministrator;
                                        else
                                            return memberRole.CampaignMember;
                                    }
                                }
                            }

                            // If no member IPublishedContent matches, return empty string
                            return string.Empty;
                        }
                    }
                }
                else
                    return string.Empty;
            }
            else
                return string.Empty;
        }
        catch (Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("linqMembers.vb : GetCurrentMemberRole()");
            sb.AppendLine("_campaignId:" + _campaignId);

            saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
            return string.Empty;
        }
    }
    public string GetCampaignMembersRole_byMemberId(int _memberId, int _campaignId)
    {
        try
        {
            // Instantiate variables
            IPublishedContent campaignNode = _uHelper.Get_IPublishedContentByID(_campaignId);
            IPublishedContent campaignMemberFolder;

            List<string> lstTeamAdministrators = new List<string>();
            foreach (string id in campaignNode.Parent.GetPropertyValue<string>(nodeProperties.teamAdministrators).Split(","))
                lstTeamAdministrators.Add(id);


            // Determine if member is a team admin
            if (lstTeamAdministrators.Contains(_memberId.ToString()))
                return memberRole.TeamAdministrator;
            else
            {
                // Obtain campaign's member folder
                campaignMemberFolder = getCampaignMemberFolder(campaignNode);
                if (string.IsNullOrEmpty(campaignMemberFolder.Name))
                    // No campaign member folder exists.
                    return string.Empty;
                else
                {
                    // Loop thru member nodes
                    foreach (IPublishedContent childNode in campaignMemberFolder.Children)
                    {
                        // Determine if childnode = current member
                        if (childNode.HasProperty(nodeProperties.campaignMember))
                        {
                            if (childNode.GetPropertyValue<string>(nodeProperties.campaignMember) == _memberId)
                            {
                                // Return wether user is a member or admin
                                if (childNode.GetPropertyValue<bool>(nodeProperties.campaignManager) == true)
                                    return memberRole.CampaignAdministrator;
                                else
                                    return memberRole.CampaignMember;
                            }
                        }
                    }

                    // If no member IPublishedContent matches, return empty string
                    return string.Empty;
                }
            }
        }
        catch (Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"\App_Code\linqQueries\linqMembers.vb : GetCampaignMembersRole_byMemberId()");
            sb.AppendLine("_memberId:" + _memberId);
            sb.AppendLine("_campaignId:" + _campaignId);

            saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
            // Return String.Empty
            return "Error Linq: " + ex.ToString();
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
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"\App_Code\linqQueries\linqMembers.vb : getMemberId_byEmail()");
            sb.AppendLine("_email:" + _email);

            saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
            return -1;
        }
    }
    public string getMemberName_byId(int _id)
    {
        // Return id
        try
        {
            IMember member = ApplicationContext.Current.Services.MemberService.GetById(_id);
            if (!IsNothing(member))
                return member.Name;
            else
                return string.Empty;
        }
        catch (Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"\App_Code\linqQueries\linqMembers.vb : getMemberName_byId()");
            sb.AppendLine("_id:" + _id);

            saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
            return string.Empty;
        }
    }
    public string getMemberName_byGuid(Guid _id)
    {
        // Return id
        try
        {
            IMember member = ApplicationContext.Current.Services.MemberService.GetByKey(_id);
            if (!IsNothing(member))
                return member.Name;
            else
                return string.Empty;
        }
        catch (Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"\App_Code\linqQueries\linqMembers.vb : getMemberName_byGuid()");
            sb.AppendLine("_id:" + _id.ToString());

            saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
            return string.Empty;
        }
    }
    public string getMemberEmail_byId(int _id)
    {
        // Return id
        try
        {
            IMember member = ApplicationContext.Current.Services.MemberService.GetById(_id);
            return member.Email;
        }
        catch (Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"\App_Code\linqQueries\linqMembers.vb : getMemberEmail_byId()");
            sb.AppendLine("_id:" + _id);

            saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
            return string.Empty;
        }
    }
    public string getCurrentUsersAltEmail()
    {
        // Return id
        try
        {
            IMember member = ApplicationContext.Current.Services.MemberService.GetByEmail(memberShipHelper.CurrentUserName);
            return member.GetValue<string>(nodeProperties.alternativeEmail);
        }
        catch (Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("linqMembers.vb : getAltEmail_byId()");

            saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
            return string.Empty;
        }
    }
    public string getUsersAltEmail_byId(int _id)
    {
        // Return id
        try
        {
            IMember member = ApplicationContext.Current.Services.MemberService.GetById(_id);
            return member.GetValue<string>(nodeProperties.alternativeEmail);
        }
        catch (Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("linqMembers.vb : getAltEmail_byId()");

            saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
            return string.Empty;
        }
    }
    public BusinessReturn getMemberDemographics_byId(int _id, bool _getDemographics = false, bool _getBillingInfo = false, bool _getShippingInfo = false, bool _getMemberProperties = false, bool _getPledgeProperties = false, bool _getStripeIDs = false)
    {

        // 
        BusinessReturn BusinessReturn = new BusinessReturn();

        try
        {
            if (_id > 0)
            {
                // Instantiate variables
                IMember member = ApplicationContext.Current.Services.MemberService.GetById(_id);
                Member clsMember = new Member();

                if (!IsNothing(member))
                {
                    // Obtain demographics
                    if (_getDemographics)
                    {
                        if (member.HasProperty(nodeProperties.firstName))
                            clsMember.Demographics.firstName = member.GetValue<string>(nodeProperties.firstName);
                        if (member.HasProperty(nodeProperties.lastName))
                            clsMember.Demographics.lastName = member.GetValue<string>(nodeProperties.lastName);
                        if (member.HasProperty(nodeProperties.photo) && !IsNothing(member.GetValue(nodeProperties.photo)))
                        {
                            clsMember.Demographics.photo = member.GetValue<int>(nodeProperties.photo);
                            clsMember.Demographics.photoUrl = getMediaURL(member.GetValue<int>(nodeProperties.photo), Crops.members);
                        }
                        else
                        {
                            clsMember.Demographics.photo = mediaNodes.defaultProfileImg;
                            clsMember.Demographics.photoUrl = getMediaURL(mediaNodes.defaultProfileImg, Crops.members);
                        }
                        clsMember.Demographics.briefDescription = member.GetValue<string>(nodeProperties.briefDescription);
                    }

                    // Obtain billing info
                    if (_getBillingInfo)
                    {
                        clsMember.BillingInfo.address01 = member.GetValue<string>(nodeProperties.address01_Billing);
                        clsMember.BillingInfo.address02 = member.GetValue<string>(nodeProperties.address02_Billing);
                        clsMember.BillingInfo.city = member.GetValue<string>(nodeProperties.city_Billing);
                        clsMember.BillingInfo.stateProvidence = member.GetValue<string>(nodeProperties.stateprovidence_Billing);
                        clsMember.BillingInfo.postalCode = member.GetValue<string>(nodeProperties.postalCode_Billing);
                    }

                    // Obtain shipping info
                    if (_getShippingInfo)
                    {
                        clsMember.ShippingInfo.address01 = member.GetValue<string>(nodeProperties.address01_Shipping);
                        clsMember.ShippingInfo.address02 = member.GetValue<string>(nodeProperties.address02_Shipping);
                        clsMember.ShippingInfo.city = member.GetValue<string>(nodeProperties.city_Shipping);
                        clsMember.ShippingInfo.stateProvidence = member.GetValue<string>(nodeProperties.stateprovidence_Shipping);
                        clsMember.ShippingInfo.postalCode = member.GetValue<string>(nodeProperties.postalCode_Shipping);
                    }

                    // Obtain member properties
                    if (_getMemberProperties)
                    {
                        clsMember.MembershipProperties.userId = _id;
                        clsMember.MembershipProperties.nodeName = member.Name;
                        clsMember.MembershipProperties.loginName = member.Username;
                        clsMember.MembershipProperties.email = member.Email;
                        clsMember.MembershipProperties.altEmail = member.GetValue<string>(nodeProperties.alternativeEmail);
                        clsMember.MembershipProperties.isFacebookAcct = member.GetValue<bool>(nodeProperties.isFacebookAcct);
                        clsMember.MembershipProperties.isLinkedInAcct = member.GetValue<bool>(nodeProperties.isLinkedInAcct);
                        clsMember.MembershipProperties.isTwitterAcct = member.GetValue<bool>(nodeProperties.isTwitterAcct);
                    }

                    // Obtain pledges
                    if (_getPledgeProperties)
                    {
                        // Obtain member's pledges as csv list
                        string pledges = member.GetValue<string>(nodeProperties.pledges);

                        if (pledges != null)
                        {
                            // Instantiate variables
                            // Dim lstPledges As New List(Of CampaignPledge)
                            List<string> pledgeIdList;

                            // Split list of pledge IDs
                            pledgeIdList = pledges.Split(",").ToList();

                            // Loop thru all IDs
                            foreach (string pledgeId in pledgeIdList)
                            {
                                // Instantiate pledge IPublishedContent
                                IPublishedContent pledgeNode = _uHelper.Get_IPublishedContentByID(pledgeId);

                                if (!IsNothing(pledgeNode))
                                {
                                    if (pledgeNode.DocumentTypeAlias == docTypes.Pledges)
                                    {
                                        // Instantiate new class object
                                        // Add data to object
                                        var campaignPledge = new CampaignPledge()
                                        {
                                            pledgeDate = pledgeNode.GetPropertyValue<DateTime>(nodeProperties.pledgeDate),
                                            pledgeAmount = pledgeNode.GetPropertyValue<decimal>(nodeProperties.pledgeAmount),
                                            campaignName = pledgeNode.Parent.Parent.Parent.Name,
                                            showAsAnonymous = pledgeNode.GetPropertyValue<bool>(nodeProperties.showAsAnonymous),
                                            fulfilled = pledgeNode.GetPropertyValue<bool>(nodeProperties.fulfilled),
                                            canceled = pledgeNode.GetPropertyValue<bool>(nodeProperties.canceled),
                                            transactionDeclined = pledgeNode.GetPropertyValue<bool>(nodeProperties.transactionDeclined),
                                            reimbursed = pledgeNode.GetPropertyValue<bool>(nodeProperties.reimbursed),
                                            campaignUrl = pledgeNode.Parent.Parent.Parent.Url
                                        };

                                        // Add object to class
                                        clsMember.PledgeList.Add(campaignPledge);
                                    }
                                }
                            }
                        }
                    }

                    // Obtain Stripe IDs
                    if (_getStripeIDs)
                    {
                        clsMember.StripeIDs.customerId = member.GetValue<string>(nodeProperties.customerId);
                        // clsMember.StripeIDs.bankAcctId = member.GetValue(Of String)(nodeProperties.bankAccountId)
                        // clsMember.StripeIDs.bankAcctToken = member.GetValue(Of String)(nodeProperties.bankAccountToken)
                        // clsMember.StripeIDs.campaignAcctId = member.GetValue(Of String)(nodeProperties.campaignAccountId)
                        // clsMember.StripeIDs.fileUploadId = member.GetValue(Of String)(nodeProperties.fileUploadId)
                        clsMember.StripeIDs.creditCardId = member.GetValue<string>(nodeProperties.creditCardId);
                        clsMember.StripeIDs.creditCardToken = member.GetValue<string>(nodeProperties.creditCardToken);
                    }

                    // Return class within businessreturn
                    BusinessReturn.DataContainer.Add(clsMember);
                }
            }
        }

        catch (Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"\App_Code\linqQueries\linqMembers.vb : getMemberDemographics_byId()");
            sb.AppendLine("_id:" + _id);
            sb.AppendLine("_getDemographics:" + _getDemographics);
            sb.AppendLine("_getBillingInfo:" + _getBillingInfo);
            sb.AppendLine("_getShippingInfo:" + _getShippingInfo);
            sb.AppendLine("_getMemberProperties:" + _getMemberProperties);
            sb.AppendLine("_getPledgeProperties:" + _getPledgeProperties);
            sb.AppendLine("_getStripeIDs:" + _getStripeIDs);

            saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
            BusinessReturn.ExceptionMessage = ex.ToString();
        }

        return BusinessReturn;
    }
    public string getPledgeCampaign(IPublishedContent thisNode)
    {
        try
        {
            if (thisNode.DocumentTypeAlias == docTypes.Campaign)
                return thisNode.Name;
            else
            {
                IPublishedContent childNode = thisNode.Parent;
                return getPledgeCampaign(childNode);
            }
        }
        catch (Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"\App_Code\linqQueries\linqMembers.vb : getPledgeCampaign()");
            sb.AppendLine("thisNode:" + thisNode.ToString());
            saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
            return "";
        }
    }
    public string getMemberPhoto_byId(int _memberId, bool _giveDefault)
    {
        try
        {
            // Instantiate variables.
            string imgUrl = string.Empty;
            IMember member = ApplicationContext.Current.Services.MemberService.GetById(_memberId);

            // Check if image exists for user.
            if ((member.HasProperty(nodeProperties.photo)) && (!string.IsNullOrWhiteSpace(member.GetValue<string>(nodeProperties.photo))))
                // Obtain user image
                imgUrl = getMediaURL(member.GetValue<int>(nodeProperties.photo), Crops.members);
            else if (_giveDefault)
                // if no image exists and user wants a default image, provide default image.
                imgUrl = getMediaURL(mediaNodes.defaultProfileImg, Crops.members);

            return imgUrl;
        }
        catch (Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"\App_Code\linqQueries\linqMembers.vb : getMemberPhoto_byId()");
            sb.AppendLine("_memberId:" + _memberId);
            sb.AppendLine("_giveDefault:" + _giveDefault);
            saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
            // Return "error: " & ex.ToString
            return string.Empty;
        }
    }
    public int? getMemberPhotoNodeId_byId(int _memberId)
    {
        try
        {
            // Instantiate variables.
            int imgNodeId = 0;
            IMember member = ApplicationContext.Current.Services.MemberService.GetById(_memberId);

            // Check if image exists for user.
            if ((member.HasProperty(nodeProperties.photo)) && (!string.IsNullOrWhiteSpace(member.GetValue<string>(nodeProperties.photo))))
                // Obtain user image
                imgNodeId = member.GetValue<int>(nodeProperties.photo);

            return imgNodeId;
        }
        catch (Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"\App_Code\linqQueries\linqMembers.vb : getMemberPhotoNodeId_byId()");
            sb.AppendLine("_memberId:" + _memberId);

            saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
            // Return "error: " & ex.ToString
            return 0;
        }
    }
    #endregion


    #region Inserts
    public BusinessReturn Insert(Dictionary<string, string> _valueDictionary)
    {
        // Instantiate variables
        // Set default msg for memberId
        BusinessReturn ValidationReturn = new BusinessReturn()
        {
            ReturnMessage = 0
        };

        try
        {
            // Obtain all needed values for creating member
            string userName = _valueDictionary.Item[queryParameters.firstName] + " " + _valueDictionary.Item[queryParameters.lastName];
            string userId = _valueDictionary.Item[queryParameters.email]; // _valueDictionary.Item(queryParameters.userId)
            string email = _valueDictionary.Item[queryParameters.email];
            string password = _valueDictionary.Item[queryParameters.password];
            string memberTypeAlias = memberRole.member;

            // Create member
            IMemberService MemberService = ApplicationContext.Current.Services.MemberService;
            IMemberGroupService MemberGroupService = ApplicationContext.Current.Services.MemberGroupService;
            IMember newMember = MemberService.CreateMemberWithIdentity(userId, email, userName, memberTypeAlias);
            newMember.IsApproved = true;
            MemberService.SavePassword(newMember, password);

            // Set member values
            newMember.SetValue("firstName", _valueDictionary.Item[queryParameters.firstName]);
            newMember.SetValue("lastName", _valueDictionary.Item[queryParameters.lastName]);

            // Set if member is using social media to login
            if (_valueDictionary.ContainsKey(nodeProperties.isFacebookAcct))
                newMember.SetValue(nodeProperties.isFacebookAcct, true);
            if (_valueDictionary.ContainsKey(nodeProperties.isLinkedInAcct))
                newMember.SetValue(nodeProperties.isFacebookAcct, true);
            if (_valueDictionary.ContainsKey(nodeProperties.isTwitterAcct))
                newMember.SetValue(nodeProperties.isFacebookAcct, true);

            // Save new member
            MemberService.Save(newMember);

            // Save new member id
            ValidationReturn.ReturnMessage = newMember.Id;

            // Create 
            CreateStripeCustomer(newMember.Id, userName, email);

            // Return successful
            return ValidationReturn;
        }

        // 'Log member in
        // Return logMemberIn(newMember.Username, newMember.RawPasswordValue)
        catch (Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"\App_Code\linqQueries\linqMembers.vb : Insert()");
            sb.AppendLine("_valueDictionary:" + _valueDictionary.ToString());

            saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
            // Throw
            ValidationReturn.ExceptionMessage = ex.ToString();
            return ValidationReturn;
        }
    }
    public BusinessReturn Insert_byPreAcct(PreAccountmembers _preAccountData)
    {
        // Instantiate variables
        BusinessReturn ValidationReturn = new BusinessReturn();
        blPreAcctData blPreAcctData = new blPreAcctData();

        // Set default msg for memberId
        ValidationReturn.ReturnMessage = 0;

        try
        {
            // Obtain all needed values for creating member
            string userName = _preAccountData.firstName + " " + _preAccountData.lastName;
            string userId = _preAccountData.email;
            string email = _preAccountData.email;
            string password = _preAccountData.password;
            string memberTypeAlias = memberRole.member;
            DateTime dob = DateTime.Parse(_preAccountData.dob);

            // Create member
            IMemberService MemberService = ApplicationContext.Current.Services.MemberService;
            IMemberGroupService MemberGroupService = ApplicationContext.Current.Services.MemberGroupService;
            IMember newMember = MemberService.CreateMemberWithIdentity(userId, email, userName, memberTypeAlias);
            newMember.IsApproved = true;
            MemberService.SavePassword(newMember, password);

            // Set member values
            // newMember.SetValue("firstName", _preAccountData.firstName)
            // newMember.SetValue("lastName", _preAccountData.lastName)
            newMember.SetValue(nodeProperties.firstName, _preAccountData.firstName);
            newMember.SetValue(nodeProperties.lastName, _preAccountData.lastName);
            newMember.SetValue(nodeProperties.dateOfBirth, dob);

            // Save new member
            MemberService.Save(newMember);

            // Save new member id
            ValidationReturn.ReturnMessage = newMember.Id;

            // Create member in stripe
            CreateStripeCustomer(newMember.Id, userName, email);

            // Log Member in
            logMemberIn(userId, password);

            // Return successful
            return ValidationReturn;
        }
        catch (Exception ex)
        {
            // Throw
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"\App_Code\linqQueries\linqMembers.vb : Insert_byPreAcct()");
            sb.AppendLine("_preAccountData:" + _preAccountData.ToString());

            saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
            ValidationReturn.ExceptionMessage = ex.ToString();
            return ValidationReturn;
        }
    }
    // Public Function Insert_byPreAcctTwitter(ByVal _preAccountData As tblPreAccountData) As BusinessReturn
    // 'Instantiate variables
    // Dim ValidationReturn As BusinessReturn = New BusinessReturn
    // Dim blPreAcctData As blPreAcctData = New blPreAcctData

    // 'Set default msg for memberId
    // ValidationReturn.ReturnMessage = 0

    // Try
    // 'Obtain all needed values for creating member
    // Dim userName As String = _preAccountData.firstName & " " & _preAccountData.lastName
    // Dim userId As String = _preAccountData.email
    // Dim email As String = ""
    // Dim password As String = _preAccountData.password
    // Dim memberTypeAlias As String = memberRole.member
    // Dim dob As Date = Date.Parse(_preAccountData.dob)

    // 'Create member
    // Dim MemberService As IMemberService = ApplicationContext.Current.Services.MemberService
    // Dim MemberGroupService As IMemberGroupService = ApplicationContext.Current.Services.MemberGroupService
    // Dim newMember As IMember = MemberService.CreateMemberWithIdentity(userId, email, userName, memberTypeAlias)
    // newMember.IsApproved = True
    // MemberService.SavePassword(newMember, password)

    // 'Set member values
    // 'newMember.SetValue("firstName", _preAccountData.firstName)
    // 'newMember.SetValue("lastName", _preAccountData.lastName)
    // newMember.SetValue(nodeProperties.firstName, _preAccountData.firstName)
    // newMember.SetValue(nodeProperties.lastName, _preAccountData.lastName)
    // newMember.SetValue(nodeProperties.dateOfBirth, dob)

    // 'Save new member
    // MemberService.Save(newMember)

    // 'Save new member id
    // ValidationReturn.ReturnMessage = newMember.Id

    // 'Create member in stripe
    // CreateStripeCustomer(newMember.Id, userName, email)

    // 'Log Member in
    // logMemberIn(userId, password)

    // 'Mark the pre-acct as created in db.
    // blPreAcctData.markPreAcctAsCreated(_preAccountData.preAcctId)

    // 'Return successful
    // Return ValidationReturn

    // Catch ex As Exception
    // Dim sb As New StringBuilder()
    // sb.AppendLine("\App_Code\linqQueries\linqMembers.vb : Insert_byPreAcctTwitter()")
    // sb.AppendLine("_preAccountData:" & _preAccountData.ToString())

    // saveErrorMessage(getLoggedInMember, ex.ToString, sb.ToString())
    // 'Throw
    // ValidationReturn.ExceptionMessage = ex.ToString
    // Return ValidationReturn
    // End Try
    // End Function
    // Public Function Insert_byPreAcctSocailMedia(ByVal _preAccountData As tblPreAccountData) As BusinessReturn
    // 'Instantiate variables
    // Dim ValidationReturn As BusinessReturn = New BusinessReturn
    // Dim blPreAcctData As blPreAcctData = New blPreAcctData

    // 'Set default msg for memberId
    // ValidationReturn.ReturnMessage = 0

    // Try
    // 'Obtain all needed values for creating member
    // Dim userName As String = _preAccountData.firstName & " " & _preAccountData.lastName
    // Dim userId As String = _preAccountData.email
    // Dim email As String = _preAccountData.email
    // Dim password As String = _preAccountData.password
    // Dim memberTypeAlias As String = memberRole.member
    // Dim dob As Date = Date.Parse(_preAccountData.dob)

    // 'Create member
    // Dim MemberService As IMemberService = ApplicationContext.Current.Services.MemberService
    // Dim MemberGroupService As IMemberGroupService = ApplicationContext.Current.Services.MemberGroupService
    // Dim newMember As IMember = MemberService.CreateMemberWithIdentity(userId, email, userName, memberTypeAlias)
    // newMember.IsApproved = True
    // MemberService.SavePassword(newMember, password)

    // 'Set member values
    // 'newMember.SetValue("firstName", _preAccountData.firstName)
    // 'newMember.SetValue("lastName", _preAccountData.lastName)
    // newMember.SetValue(nodeProperties.firstName, _preAccountData.firstName)
    // newMember.SetValue(nodeProperties.lastName, _preAccountData.lastName)
    // newMember.SetValue(nodeProperties.dateOfBirth, dob)

    // 'Save new member
    // MemberService.Save(newMember)

    // 'Save new member id
    // ValidationReturn.ReturnMessage = newMember.Id

    // 'Create member in stripe
    // CreateStripeCustomer(newMember.Id, userName, email)

    // 'Log Member in
    // logMemberIn(userId, password)

    // 'Mark the pre-acct as created in db.
    // blPreAcctData.markPreAcctAsCreated(_preAccountData.preAcctId)

    // 'Return successful
    // Return ValidationReturn

    // Catch ex As Exception
    // Dim sb As New StringBuilder()
    // sb.AppendLine("\App_Code\linqQueries\linqMembers.vb : Insert_byPreAcctSocailMedia()")
    // sb.AppendLine("_preAccountData:" & _preAccountData.ToString())

    // saveErrorMessage(getLoggedInMember, ex.ToString, sb.ToString())
    // 'Throw
    // ValidationReturn.ExceptionMessage = ex.ToString
    // Return ValidationReturn
    // End Try
    // End Function
    public void CreateStripeCustomer(int _userId, string _name, string _email)
    {
        // Instantiate variables
        StripeCustomerCreateOptions customerCreateOptions = new StripeCustomerCreateOptions();
        var customerService = new StripeCustomerService(ConfigurationManager.AppSettings(Miscellaneous.StripeApiKey));
        StripeCustomer StripeCustomer;
        BusinessReturn returnResult;
        Member member = new Member();

        try
        {
            // Add values for creating customer.
            customerCreateOptions.Description = _name + " [" + _userId.ToString() + "]";
            customerCreateOptions.Email = _email;

            // Create customer in stripe
            StripeCustomer = customerService.Create(customerCreateOptions);

            // Add customer Id to member class
            member.StripeIDs.customerId = StripeCustomer.Id;
            member.MembershipProperties.userId = _userId;

            // Save to umbraco
            returnResult = InsertStripeIDs(member);

            if (!returnResult.isValid)
                saveErrorMessage(_userId, returnResult.ExceptionMessage, "");
        }
        catch (Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"\App_Code\linqQueries\linqMembers.vb : CreateStripeCustomer()");
            sb.AppendLine("_userId:" + _userId);
            sb.AppendLine("_name:" + _name);
            sb.AppendLine("_email:" + _email);
            saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
            saveErrorMessage(_userId, ex.ToString(), "");
        }
    }
    public BusinessReturn InsertStripeIDs(Member _member)
    {
        // Instantiate variables
        BusinessReturn returnResult = new BusinessReturn();

        try
        {
            // Instantiate variables
            IMemberService MemberService = ApplicationContext.Current.Services.MemberService;
            IMember member = ApplicationContext.Current.Services.MemberService.GetById(_member.MembershipProperties.userId);

            // Update the Notes field with existing card details before updating stripe information in member backoffice
            MoveStripeDataToNotes(_member.MembershipProperties.userId.ToString(), true);

            // Set campaign acct id.
            if (!string.IsNullOrWhiteSpace(_member.StripeIDs.customerId))
                member.SetValue(nodeProperties.customerId, _member.StripeIDs.customerId);
            // If Not String.IsNullOrWhiteSpace(_member.StripeIDs.campaignAcctId) Then
            // member.SetValue(nodeProperties.campaignAccountId, _member.StripeIDs.campaignAcctId)
            // End If
            // If Not String.IsNullOrWhiteSpace(_member.StripeIDs.fileUploadId) Then
            // member.SetValue(nodeProperties.fileUploadId, _member.StripeIDs.fileUploadId)
            // End If
            // If Not String.IsNullOrWhiteSpace(_member.StripeIDs.bankAcctToken) Then
            // member.SetValue(nodeProperties.bankAccountToken, _member.StripeIDs.bankAcctToken)
            // End If
            // If Not String.IsNullOrWhiteSpace(_member.StripeIDs.bankAcctId) Then
            // member.SetValue(nodeProperties.bankAccountId, _member.StripeIDs.bankAcctId)
            // End If
            if (!string.IsNullOrWhiteSpace(_member.StripeIDs.creditCardToken))
                member.SetValue(nodeProperties.creditCardToken, _member.StripeIDs.creditCardToken);
            if (!string.IsNullOrWhiteSpace(_member.StripeIDs.creditCardId))
                member.SetValue(nodeProperties.creditCardId, _member.StripeIDs.creditCardId);

            // Save new member
            MemberService.Save(member);

            return returnResult;
        }
        catch (Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"\App_Code\linqQueries\linqMembers.vb : InsertStripeIDs()");
            sb.AppendLine("_member:" + _member.ToString());

            saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
            returnResult.ExceptionMessage = "Error: " + ex.ToString();
            return returnResult;
        }
    }
    public Int16? CreateCampaignMembersFolder(Int16 parentNodeId)
    {
        try  // Create a new campaign member folder IPublishedContent
        {
            IContentService cs = ApplicationContext.Current.Services.ContentService;
            IContent campaign = cs.GetById(parentNodeId);
            IContent campaignMembers = cs.CreateContentWithIdentity(Miscellaneous.campaignMembers, campaign, docTypes.campaignMembers);
            cs.SaveAndPublishWithStatus(campaignMembers);
            // Return new IPublishedContent's Id
            return campaignMembers.Id;
        }
        catch (Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"\App_Code\linqQueries\linqMembers.vb : CreateCampaignMembersFolder()");
            sb.AppendLine("parentNodeId:" + parentNodeId);
            saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
            return default(Short?);
        }
    }
    #endregion


    #region Updates
    public BusinessReturn Update(Member _member)
    {
        // Instantiate variables
        BusinessReturn ValidationReturn = new BusinessReturn();
        IMember member;

        try
        {
            // Obtain current member
            member = ApplicationContext.Current.Services.MemberService.GetById(_member.MembershipProperties.userId);

            // Set values of member
            member.SetValue(nodeProperties.firstName, _member.Demographics.firstName);
            member.SetValue(nodeProperties.lastName, _member.Demographics.lastName);
            member.SetValue(nodeProperties.briefDescription, _member.Demographics.briefDescription);
            // member.SetValue(nodeProperties._umb_email, _member.MembershipProperties.email)
            if (!string.IsNullOrEmpty(_member.MembershipProperties.email))
                member.Email = _member.MembershipProperties.email;
            if (!string.IsNullOrEmpty(_member.MembershipProperties.loginName))
                member.Username = _member.MembershipProperties.loginName;
            member.SetValue(nodeProperties.alternativeEmail, _member.MembershipProperties.altEmail);

            member.SetValue(nodeProperties.address01_Billing, _member.BillingInfo.address01);
            member.SetValue(nodeProperties.address02_Billing, _member.BillingInfo.address02);
            member.SetValue(nodeProperties.city_Billing, _member.BillingInfo.city);
            member.SetValue(nodeProperties.stateprovidence_Billing, _member.BillingInfo.stateProvidence);
            member.SetValue(nodeProperties.postalCode_Billing, _member.BillingInfo.postalCode);

            member.SetValue(nodeProperties.address01_Shipping, _member.ShippingInfo.address01);
            member.SetValue(nodeProperties.address02_Shipping, _member.ShippingInfo.address02);
            member.SetValue(nodeProperties.city_Shipping, _member.ShippingInfo.city);
            member.SetValue(nodeProperties.stateprovidence_Shipping, _member.ShippingInfo.stateProvidence);
            member.SetValue(nodeProperties.postalCode_Shipping, _member.ShippingInfo.postalCode);

            // Save data to member.
            ApplicationContext.Current.Services.MemberService.Save(member);

            // 
            if (!string.IsNullOrEmpty(_member.MembershipProperties.password))
            {
                // Create memberservice
                IMemberService MemberService = ApplicationContext.Current.Services.MemberService;
                MemberService.SavePassword(member, _member.MembershipProperties.password);
                // Save member's password
                MemberService.Save(member);
            }
        }
        catch (Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"\App_Code\linqQueries\linqMembers.vb : Update()");
            sb.AppendLine("_member:" + _member.ToString());

            saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
            ValidationReturn.ExceptionMessage = ex.ToString();
        }

        return ValidationReturn;
    }
    public BusinessReturn updatePhoto(int _memberId, int _imediaId)
    {
        // Instantiate variables
        BusinessReturn ValidationReturn = new BusinessReturn();
        IMember member;

        try
        {
            // Obtain member
            member = ApplicationContext.Current.Services.MemberService.GetById(_memberId);

            // Set values of member
            member.SetValue(nodeProperties.photo, _imediaId);

            // Save data to member.
            ApplicationContext.Current.Services.MemberService.Save(member);
        }
        catch (Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"\App_Code\linqQueries\linqMembers.vb : updatePhoto()");
            sb.AppendLine("_memberId:" + _memberId);

            saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
            ValidationReturn.ExceptionMessage = ex.ToString();
        }

        return ValidationReturn;
    }
    public BusinessReturn UpdateAtCheckout(Member _member)
    {
        // Instantiate variables
        BusinessReturn ValidationReturn = new BusinessReturn();
        IMember member;

        try
        {
            // Obtain current member
            member = ApplicationContext.Current.Services.MemberService.GetById(_member.MembershipProperties.userId);

            // Set values of member
            // member.SetValue(nodeProperties.firstName, _member.Demographics.firstName)
            // member.SetValue(nodeProperties.lastName, _member.Demographics.lastName)
            // member.SetValue(nodeProperties.briefDescription, _member.Demographics.briefDescription)
            // member.SetValue(nodeProperties._umb_email, _member.MembershipProperties.email)
            // If Not String.IsNullOrEmpty(_member.MembershipProperties.email) Then member.Email = _member.MembershipProperties.email
            // If Not String.IsNullOrEmpty(_member.MembershipProperties.loginName) Then member.Username = _member.MembershipProperties.loginName

            member.SetValue(nodeProperties.address01_Billing, _member.BillingInfo.address01);
            member.SetValue(nodeProperties.address02_Billing, _member.BillingInfo.address02);
            member.SetValue(nodeProperties.city_Billing, _member.BillingInfo.city);
            member.SetValue(nodeProperties.stateprovidence_Billing, _member.BillingInfo.stateProvidence);
            member.SetValue(nodeProperties.postalCode_Billing, _member.BillingInfo.postalCode);

            member.SetValue(nodeProperties.address01_Shipping, _member.ShippingInfo.address01);
            member.SetValue(nodeProperties.address02_Shipping, _member.ShippingInfo.address02);
            member.SetValue(nodeProperties.city_Shipping, _member.ShippingInfo.city);
            member.SetValue(nodeProperties.stateprovidence_Shipping, _member.ShippingInfo.stateProvidence);
            member.SetValue(nodeProperties.postalCode_Shipping, _member.ShippingInfo.postalCode);

            if (!string.IsNullOrEmpty(_member.MembershipProperties.altEmail))
                member.SetValue(nodeProperties.alternativeEmail, _member.MembershipProperties.altEmail);

            // Save data to member.
            ApplicationContext.Current.Services.MemberService.Save(member);

            // '
            // If Not String.IsNullOrEmpty(_member.MembershipProperties.password) Then
            // 'Create memberservice
            // Dim MemberService As IMemberService = ApplicationContext.Current.Services.MemberService
            // MemberService.SavePassword(member, _member.MembershipProperties.password)
            // 'Save member's password
            // MemberService.Save(member)
            // End If

            ValidationReturn.ReturnMessage = "Umbraco Updated Successfully";
            return ValidationReturn;
        }
        catch (Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"\App_Code\linqQueries\linqMembers.vb : UpdateAtCheckout()");
            sb.AppendLine("Member: " + JsonConvert.SerializeObject(_member));

            saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
            ValidationReturn.ExceptionMessage = ex.ToString();
        }

        return ValidationReturn;
    }
    public BusinessReturn UpdatePledges(int _memberId, int _pledgeId)
    {
        // Instantiate variables
        BusinessReturn ValidationReturn = new BusinessReturn();
        IMember member;
        string _HoldPledgeId = null;


        try
        {
            // Obtain current member
            member = ApplicationContext.Current.Services.MemberService.GetById(_memberId);

            if (member.HasProperty(nodeProperties.pledges) && !IsNothing(member.GetValue(nodeProperties.pledges)))
                _HoldPledgeId = member.GetValue(nodeProperties.pledges).ToString();

            if (Information.IsNothing(_HoldPledgeId) | string.IsNullOrEmpty(_HoldPledgeId))
                _HoldPledgeId += _pledgeId.ToString();
            else
                _HoldPledgeId += "," + _pledgeId.ToString();

            member.SetValue(nodeProperties.pledges, _HoldPledgeId);

            // Save data to member.
            ApplicationContext.Current.Services.MemberService.Save(member);

            return ValidationReturn;
        }
        catch (Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("linqMembers.vb : UpdatePledges()");
            sb.AppendLine("_memberId: " + _memberId);
            sb.AppendLine("_pledgeId: " + _pledgeId);

            saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
            ValidationReturn.ExceptionMessage = ex.ToString();

            return ValidationReturn;
        }
    }
    // This function is used to moving the invaild or updated stripe information data to notes section in memeber back office
    public BusinessReturn MoveStripeDataToNotes(string _memberId, bool IsFinancialHandler = false)
    {
        // Instantiate variables
        BusinessReturn ValidationReturn = new BusinessReturn();
        IMember member;
        string _HoldStripeData;
        StringBuilder cb = new StringBuilder();
        try
        {
            // Obtain current member
            member = ApplicationContext.Current.Services.MemberService.GetById(System.Convert.ToInt32(_memberId));

            if (member.HasProperty(nodeProperties.notes) && !IsNothing(member.GetValue(nodeProperties.notes)))
            {
                _HoldStripeData = member.GetValue(nodeProperties.notes).ToString();
                cb.AppendLine(" ");
                cb.AppendLine("---------------------" + DateTime.Now.ToLongTimeString() + "---------------------------");
                cb.AppendLine(_HoldStripeData);
                cb.AppendLine(" ");
            }
            else
            {
                cb.AppendLine("---------------------" + DateTime.Now.ToLongTimeString() + "---------------------------");
                cb.AppendLine("Invalid Stripe informations : ");
                cb.AppendLine(" ");
            }

            if (member.HasProperty(nodeProperties.customerId) && !IsNothing(member.GetValue(nodeProperties.customerId)))
                cb.AppendLine("CustomerID : " + member.GetValue(nodeProperties.customerId).ToString());

            if (member.HasProperty(nodeProperties.creditCardId) && !IsNothing(member.GetValue(nodeProperties.creditCardId)))
                cb.AppendLine("CreditCardId : " + member.GetValue(nodeProperties.creditCardId).ToString());

            if (member.HasProperty(nodeProperties.creditCardToken) && !IsNothing(member.GetValue(nodeProperties.creditCardToken)))
                cb.AppendLine("CreditCardToken : " + member.GetValue(nodeProperties.creditCardToken).ToString());

            member.SetValue(nodeProperties.notes, cb.ToString());

            // Clear field only if the request not made from the Financial Handler, if request made from the Financial Handler then do nothing 
            if (IsFinancialHandler == false)
            {
                member.SetValue(nodeProperties.creditCardToken, string.Empty);
                member.SetValue(nodeProperties.customerId, string.Empty);
                member.SetValue(nodeProperties.creditCardId, string.Empty);
            }

            // Save data to member.
            ApplicationContext.Current.Services.MemberService.Save(member);

            return ValidationReturn;
        }
        catch (Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("linqMembers.vb : MoveStripeDataToNotes()");
            sb.AppendLine("_member: " + _memberId);
            saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
            ValidationReturn.ExceptionMessage = ex.ToString();

            return ValidationReturn;
        }
    }
    // This function is used to insert the stripe customer id  in memeber back office
    public BusinessReturn InsertStripeCustomerID(string _memberId, string newCustomerId)
    {
        // Instantiate variables
        BusinessReturn ValidationReturn = new BusinessReturn();
        IMember member;
        try
        {
            // Obtain current member
            member = ApplicationContext.Current.Services.MemberService.GetById(System.Convert.ToInt32(_memberId));
            member.SetValue(nodeProperties.customerId, newCustomerId);
            // Save data to member.
            ApplicationContext.Current.Services.MemberService.Save(member);

            return ValidationReturn;
        }
        catch (Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("linqMembers.vb : InsertStripeCustomerID()");
            sb.AppendLine("_member: " + _memberId);
            saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
            ValidationReturn.ExceptionMessage = ex.ToString();

            return ValidationReturn;
        }
    }
    public BusinessReturn UpdateReviews(int _memberId, int _reviewId)
    {
        // Instantiate variables
        BusinessReturn ValidationReturn = new BusinessReturn();
        IMember member;
        // Dim _HoldReviewIDs As String = String.Empty
        string locaUdi = string.Empty;

        try
        {
            // Obtain current member
            var IPublishedContent = ApplicationContext.Current.Services.ContentService.GetById(_reviewId);
            locaUdi = Udi.Create(Constants.UdiEntityType.Document, IPublishedContent.Key).ToString();
            member = ApplicationContext.Current.Services.MemberService.GetById(_memberId);
            if (member.GetValue(nodeProperties.reviews) != null)
                locaUdi = member.GetValue(nodeProperties.reviews).ToString() + "," + locaUdi;
            member.SetValue(nodeProperties.reviews, locaUdi.ToString());

            // Save data to member.
            ApplicationContext.Current.Services.MemberService.Save(member);

            return ValidationReturn;
        }
        catch (Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("linqMembers.vb : UpdateReviews()");
            sb.AppendLine("_memberId: " + _memberId);
            sb.AppendLine("_reviewId: " + _reviewId);

            saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
            ValidationReturn.ExceptionMessage = ex.ToString();

            return ValidationReturn;
        }
    }
    public BusinessReturn UpdateAltEmail(int currentMemberId, string altEmail)
    {
        // Instantiate variables
        BusinessReturn ValidationReturn = new BusinessReturn();
        IMember member;

        try
        {
            // Obtain current member
            member = ApplicationContext.Current.Services.MemberService.GetById(currentMemberId);

            // Set values of member
            member.SetValue(nodeProperties.alternativeEmail, altEmail);

            // Save data to member.
            ApplicationContext.Current.Services.MemberService.Save(member);


            StringBuilder sb = new StringBuilder();
            sb.AppendLine("currentMemberId: " + currentMemberId);
            sb.AppendLine("altEmail: " + altEmail);
            sb.AppendLine("Name: " + member.Name);
            ValidationReturn.ReturnMessage = sb.ToString();


            return ValidationReturn;
        }
        catch (Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("linqMembers.vb : UpdateAltEmail()");
            sb.AppendLine("currentMemberId:" + currentMemberId);
            sb.AppendLine("altEmail:" + altEmail);

            saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
            ValidationReturn.ExceptionMessage = ex.ToString();
        }

        return ValidationReturn;
    }
    #endregion


    #region Deletes
    public BusinessReturn DeleteCreditcard(int _userId)
    {
        // Instantiate variables
        BusinessReturn ValidationReturn = new BusinessReturn();
        IMember member;

        try
        {
            // Obtain current member
            member = ApplicationContext.Current.Services.MemberService.GetById(_userId);

            // Set values of member
            member.SetValue(nodeProperties.creditCardId, string.Empty);
            member.SetValue(nodeProperties.creditCardToken, string.Empty);

            // Save data to member.
            ApplicationContext.Current.Services.MemberService.Save(member);
        }
        catch (Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"\App_Code\linqQueries\linqMembers.vb : DeleteCreditcard()");
            sb.AppendLine("_userId:" + _userId);

            saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
            ValidationReturn.ExceptionMessage = ex.ToString();
        }

        return ValidationReturn;
    }
    #endregion


    #region Log In/Out
    public bool isMemberLoggedIn()
    {
        return memberShipHelper.IsLoggedIn();
    }
    public bool logMemberIn(string _userName, string _password)
    {
        try
        {
            if (memberShipHelper.Login(_userName, _password))
            {
                // Set cookie
                System.Web.Security.FormsAuthentication.SetAuthCookie(_userName, false);

                return true;
            }
            else
                return false;
        }
        catch (Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"\App_Code\linqQueries\linqMembers.vb : logMemberIn()");
            sb.AppendLine("_userName:" + _userName);
            sb.AppendLine("_password:" + _password);
            saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
            return false;
        }
    }
    public bool externallogMemberIn(string _userName)
    {
        try
        {
            // If memberShipHelper.Login(_userName, _password) Then
            System.Web.Security.FormsAuthentication.SetAuthCookie(_userName, false);
            return true;
        }
        // Else
        // Return False
        // End If
        catch (Exception ex)
        {
            saveErrorMessage(getLoggedInMember, ex.ToString(), @"\App_Code\linqQueries\linqMembers.vb : externallogMemberIn()");
            return false;
        }
    }
    public void logMemberOut()
    {
        // Log member out
        System.Web.HttpContext.Current.Session.Clear();
        System.Web.HttpContext.Current.Session.Abandon();
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
        return !IsNothing(member);
    }
    public mediaType_Values getMembersLoginType(string email)
    {
        // Return if exists
        IMember member = ApplicationContext.Current.Services.MemberService.GetByEmail(email);

        if (IsNothing(member))
            return mediaType_Values.none;
        else
            // Determine what kind of account user is.
            if (member.GetValue<bool>(nodeProperties.isFacebookAcct))
            return mediaType_Values.Facebook;
        else if (member.GetValue<bool>(nodeProperties.isTwitterAcct))
            return mediaType_Values.Twitter;
        else if (member.GetValue<bool>(nodeProperties.isLinkedInAcct))
            return mediaType_Values.LinkedIn;
        else
            return mediaType_Values.none;


        return !IsNothing(member);
    }
    public IPublishedContent getCampaignMemberFolder(IPublishedContent _campaignNode)
    {
        try
        {
            // Loop thru child nodes, obtain campaignMember folder.
            foreach (IPublishedContent childNode in _campaignNode.Children)
            {
                if (childNode.DocumentTypeAlias == docTypes.campaignMembers)
                {
                    return childNode;
                    break;
                }
            }
            return null/* TODO Change to default(_) if this is not a reference type */;
        }
        catch (Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"\App_Code\linqQueries\linqMembers.vb : getCampaignMemberFolder()");
            sb.AppendLine("_campaignNode:" + JsonConvert.SerializeObject(_campaignNode));
            saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
        }
        return null/* TODO Change to default(_) if this is not a reference type */;
    }
    public void EnsureStripeCustomerIdExist_byLoginName(string _loginName)
    {
        try
        {
            // Obtain member by email.
            IMember member = ApplicationContext.Current.Services.MemberService.GetByEmail(_loginName);

            // If property exists then exit sub
            if (member.HasProperty(nodeProperties.customerId) && !string.IsNullOrEmpty(member.GetValue<string>(nodeProperties.customerId)))
                return;
            else
                // Value is missing.  create customer in stripe
                CreateStripeCustomer(member.Id, member.Name, _loginName);
        }
        catch (Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"\App_Code\linqQueries\linqMembers.vb : EnsureStripeCustomerIdExist_byLoginName()");
            sb.AppendLine("_loginName:" + _loginName);
            saveErrorMessage(getLoggedInMember, ex.ToString(), sb.ToString());
            saveErrorMessage(getLoggedInMember, ex.ToString(), "");
        }
    }
    #endregion

}
