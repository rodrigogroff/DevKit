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
        public bool AuthorizeAndStartDatabase(LoginInfo _login)
        {
            myApplication = HttpContext.Current.Application;

            if (_login == null)
                return false;

            login = _login;
            
            return SetupDb();
        }

        [NonAction]
        public bool SetupDb()
        {
            var myUserTag = CachedObject.User + login.idUser;

            db = new DevKitDB()
            {
                currentUser = RestoreCache(myUserTag) as User
            };

            if (!db.ValidateUser(login))
                return false;

            BackupCache(myUserTag, db.currentUser);

            return true;
        }

        [NonAction]
        public LoginInfo GetLoginFromRequest()
        {
            var strRequest = Request.GetQueryStringValue("login");

            if (strRequest != null)
                return JsonConvert.DeserializeObject<LoginInfo>(strRequest);

            return null;
        }
    }
}
