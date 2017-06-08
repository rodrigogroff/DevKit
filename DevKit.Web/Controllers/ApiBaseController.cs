using DataModel;
using Newtonsoft.Json;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	[Authorize]
	public class ApiControllerBase : ApiController
	{
        public DevKitDB db;
        public LoginInfo login;

        public string serviceResponse = "";

        [NonAction]
		public LoginInfo GetLoginFromRequest()
        {
            var strRequest = Request.GetQueryStringValue("login");
            if (strRequest != null)
                return JsonConvert.DeserializeObject<LoginInfo>(strRequest);
            return null;
        }
        
        [NonAction]
        public bool AuthorizeAndStartDatabase()
        {
            var login = GetLoginFromRequest();
            if (login == null)
                return false;

            db = new DevKitDB();
            
            if (!db.ValidateUser(login.idUser))
                return false;
            return true;
        }

        [NonAction]
        public bool AuthorizeAndStartDatabase(LoginInfo _login)
        {
            if (_login == null)
                return false;

            db = new DevKitDB();

            if (!db.ValidateUser(_login.idUser))
                return false;

            login = _login;

            return true;
        }
    }
}