using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace Services
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    
    public class CustomServices : System.Web.Services.WebService
    {
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string HelloWorld()
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();
            dict.Add(1, "Hello");
            dict.Add(2, "World");

            return JsonConvert.SerializeObject(dict);
        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SendUpdatesByEmail()
        {
            var response = Controllers.MembershipController.SendUpdatesByEmail();


            Dictionary<int, string> dict = new Dictionary<int, string>();
            dict.Add(1, response);

            return JsonConvert.SerializeObject(dict);
        }
    }
}
