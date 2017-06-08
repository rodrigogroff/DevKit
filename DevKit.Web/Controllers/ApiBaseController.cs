using DataModel;
using Newtonsoft.Json;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	[Authorize]
	public class ApiControllerBase : ApiController
	{
        [NonAction]
		public LoginInfo GetLoginInfo()
        {
            var strRequest = Request.GetQueryStringValue("login");

            if (strRequest != null)
                return JsonConvert.DeserializeObject<LoginInfo>(strRequest);

            return null;
        }

        public DevKitDB db;
        public LoginInfo login;

        [NonAction]
        public bool GetAuthorizationAndDatabase()
        {
            var login = GetLoginInfo();

            if (login == null)
                return false;

            db = new DevKitDB();
            
            if (!db.ValidateUser(login.idUser))
                return false;

            return true;
        }
    }
}