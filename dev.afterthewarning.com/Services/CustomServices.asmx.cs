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
        public string SendUpdatesByEmail()
        {
            //Send updates via email and return results.
            Dictionary<string, int> dict = Controllers.MembershipController.SendUpdatesByEmail();
            return JsonConvert.SerializeObject(dict);
        }
    }
}




//[WebMethod]
//[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
//public string HelloWorld()
//{
//    Dictionary<int, string> dict = new Dictionary<int, string>();
//    dict.Add(1, "Hello");
//    dict.Add(2, "World");

//    return JsonConvert.SerializeObject(dict);
//}