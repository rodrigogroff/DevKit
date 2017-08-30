using DataModel;
using DevKit.Web.Services;
using System.Web.Http;
using System.Linq;
using System.Threading;
using System.Web;

namespace DevKit.Web.Controllers
{
	[Authorize]
	public class ApiControllerBase : MemCacheController
	{
        public AutorizadorCNDB db;
        
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
            db = new AutorizadorCNDB();

            var userCurrentName = userLoggedName;

            if (userCurrentName != "DBA")
            {
                var tagName = CacheTags.T_Loja + userCurrentName;

                db.currentUser = RestoreCacheNoHit(tagName) as T_Loja;

                if (db.currentUser == null)
                {
                    db.currentUser = (from ne in db.T_Loja
                                      where ne.st_loja.ToUpper() == userCurrentName
                                      select ne).
                                      FirstOrDefault();

                    if (db.currentUser == null)
                        return false;

                    BackupCacheNoHit(tagName, db.currentUser);
                }
            }
            else
            {
                if (myApplication == null)
                    myApplication = HttpContext.Current.Application;
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
