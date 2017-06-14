using DataModel;
using Newtonsoft.Json;
using System.Web;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	[Authorize]
	public class ApiControllerBase : MemCacheController
	{
        public DevKitDB db;
        public LoginInfo login;

        public int count = 0;
        public string serviceResponse = "";
        
        [NonAction]
        public bool AuthorizeAndStartDatabase()
        {
            myApplication = HttpContext.Current.Application;
            login = GetLoginFromRequest();

            if (login == null)
                return false;

            return SetupDb();
        }

        [NonAction]
        public LoginInfo GetLoginFromRequest()
        {
            var strRequest = Request.GetQueryStringValue("login");

            if (strRequest != null)
                return JsonConvert.DeserializeObject<LoginInfo>(strRequest);

            return null;
        }

        [NonAction]
        public bool AuthorizeAndStartDatabase(LoginInfo _login)
        {
            login = _login;
            myApplication = HttpContext.Current.Application;

            if (_login == null)
                return false;

            return SetupDb();
        }

        [NonAction]
        public bool SetupDb()
        {
            db = new DevKitDB();

            if (!db.ValidateUser(login))
                return false;

            BackupCache(db.currentUser);

            return true;
        }
    }
}
