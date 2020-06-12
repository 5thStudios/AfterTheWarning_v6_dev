using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.Mvc;
using static Models.Common;
using static umbraco.uQuery;

namespace Helpers
{
    public static class HtmlExtensions
    {
        public static Boolean isUserPledgedForPrayer(this HtmlHelper helper, String loginId, int prayerId)
        {
            //Update member requests
            IMember member = ApplicationContext.Current.Services.MemberService.GetByEmail(loginId);  // MemberService.GetByEmail(loginId);
            
            if (member == null)
            {
                //Save error message to umbraco
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"Helpers/HtmlExtensions.cs : isUserPledgedForPrayer()");
                sb.AppendLine("loginId: " + loginId);
                sb.AppendLine("prayerId: " + prayerId);
                Common.saveErrorMessage(sb.ToString(), sb.ToString());

                return false;
            }
            else
            {
                //Instantiate list
                List<Models._prayerRequest> lstPrayerRequests = new List<Models._prayerRequest>();

                //Populate list with any existing data
                if (member.HasProperty(NodeProperties.prayersOfferedFor) && member.GetValue(NodeProperties.prayersOfferedFor) != null)
                {
                    lstPrayerRequests = JsonConvert.DeserializeObject<List<Models._prayerRequest>>(member.GetValue(NodeProperties.prayersOfferedFor).ToString());
                }

                //Return if any records exist for prayer within member's pledges.
                return lstPrayerRequests.Exists(e => e.prayer == prayerId.ToString());

            }
        }
    }
}