using DataModel;
using DevKit.Web.Controllers;
using LinqToDB;
using System.Linq;
using System.Web;

namespace DevKit.Web.Services
{
    public class StartupPreCacheService 
    {
        public void Run(HttpApplicationState _app, User currentUser )
        {
            var cache = new MemCacheController()
            {
                myApplication = _app
            };

            int maxRowsToCache = 1000;

            using (var db = new DevKitDB())
            {
                db.currentUser = currentUser;

                // ----------------------------------------------------------------------------------------------------

                foreach (var item in new EnumMonth().lst) cache.StoreCache(CacheTags.EnumMonth, item.id, item);

                // ----------------------------------------------------------------------------------------------------

                {
                    var hshReport = cache.SetupCacheReport(CacheTags.EnumMonthReport);
                    var query = (from e in new EnumMonth().lst select e);
                    var ret = new { count = query.Count(), results = query.ToList() };
                    hshReport[""] = ret;
                }

                // ----------------------------------------------------------------------------------------------------
                
                // user
 //               {
//                    var hshReport = cache.SetupCacheReport(CacheTags.UserReport);
                                        
  //                  var filter = new UserFilter { skip = 0, take = 15 };
    //                hshReport[filter.Parameters()] = new User().ComposedFilters(db, filter);
   //             }

            }
        }
    }
}