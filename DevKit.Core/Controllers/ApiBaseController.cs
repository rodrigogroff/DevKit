using DataModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DevKit.Core.Controllers
{
    public class ApiBaseController : Controller
    {
        [NonAction]
        public LoginInfo GetLoginInfo()
        {
            var strRequest = Request.GetQueryStringValue("login");

            if (strRequest != null)
                return JsonConvert.DeserializeObject<LoginInfo>(strRequest);

            return new LoginInfo { idUser = 0 };
        }
    }
}
