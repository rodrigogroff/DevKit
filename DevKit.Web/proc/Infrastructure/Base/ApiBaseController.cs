using DataModel;
using DevKit.Web.Services;
using System.Web.Http;
using System.Linq;
using System.Threading;

namespace DevKit.Web.Controllers
{
	[Authorize]
	public class ApiControllerBase : MemCacheController
	{
        public DevKitDB db;
        
        public string apiError = "";

        public string userLoggedName
        {
            get
            {
                return Thread.CurrentPrincipal.Identity.Name.ToUpper();
            }
        }
        
        [NonAction]
        public bool StartDatabaseAndAuthorize()
        {
            db = new DevKitDB();

            var userCurrentName = userLoggedName;
            var tagName = CacheTags.Lojista + userCurrentName;

            db.currentUser = RestoreCacheNoHit(tagName) as User;
            
            if (db.currentUser == null)
            {
                db.currentUser = (from ne in db.User
                                  where ne.stLogin.ToUpper() == userCurrentName
                                  select ne).
                                  FirstOrDefault();

                if (db.currentUser == null)
                    return false;

                BackupCacheNoHit(tagName, db.currentUser);
            }

            if (myApplication["start"] == null)
            {
                myApplication["start"] = true;

                StartCache();

                if (IsPrecacheEnabled)
                    System.Threading.Tasks.Task.Run(() => { new StartupPreCacheService().Run(myApplication, db.currentUser); });

                if (!IsSingleMachine)
                    System.Threading.Tasks.Task.Run(() => { new CacheControlService().Run(myApplication); });
            }

            return true;
        }
    }
}
