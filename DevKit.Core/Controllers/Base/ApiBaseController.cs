using DataModel;
using LinqToDB.Data;
using Npgsql;
using LinqToDB.DataProvider.PostgreSQL;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DevKit.Core.Controllers
{
    public class ApiBaseController : Controller
    {
        public DevKitDB db;
        public LoginInfo login;

        public int count = 0;

        public string serviceResponse = "";

        [NonAction]
        public bool AuthorizeAndStartDatabase()
        {
            //myApplication = HttpContext.Current.Application;
       //     login = GetLoginFromRequest();

       //     if (login == null)
        //        return false;

            return SetupDb();
        }

        [NonAction]
        public bool AuthorizeAndStartDatabase(LoginInfo _login)
        {
            //myApplication = HttpContext.Current.Application;

         //   if (_login == null)
           //     return false;

            //login = _login;

            return SetupDb();
        }

        [NonAction]
        public bool SetupDb()
        {
            //var myUserTag = CachedObject.User + login.idUser;
      //      DataConnection.DefaultSettings = new MySettings();
//            DataConnection.DefaultConfiguration = "MySettings";
  //          DataConnection.AddConfiguration("MySettings", "Server=localhost;Port=5432;Database=DevKit;User Id=postgres;Password=rodrigo;",  )

            

            db = new DevKitDB()
            {                
              //  currentUser = RestoreCache(myUserTag) as User
            };

            var setup = db.Setup.Find(1);

            if (!db.ValidateUser(login))
                return false;

        //    BackupCache(myUserTag, db.currentUser);

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
