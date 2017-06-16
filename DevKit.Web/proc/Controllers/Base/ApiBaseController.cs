using DataModel;
using DevKit.Web.Services;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	[Authorize]
	public class ApiControllerBase : MemCacheController
	{
        public DevKitDB db;
        
        public int count = 0;
        public string apiResponse = "";
        
        [NonAction]
        public bool StartDatabaseAndAuthorize()
        {
            db = new DevKitDB();

            var resp = db.ValidateUser();

            if (resp == true && myApplication["start"] == null)
            {
                myApplication["start"] = true;

                StartCache();

                System.Threading.Tasks.Task.Run(() => { new StartupPreCacheService().Run(myApplication, db.currentUser); });
                System.Threading.Tasks.Task.Run(() => { new CacheControlService().Run(myApplication); });
            }

            return resp;
        }
    }
}
