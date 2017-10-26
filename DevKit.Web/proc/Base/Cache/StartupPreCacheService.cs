using DataModel;
using DevKit.Web.Controllers;
using LinqToDB;
using System.Linq;
using System.Web;

namespace DevKit.Web.Services
{
    public class StartupPreCacheService 
    {
        public void Run(HttpApplicationState _app, T_Loja currentUser )
        {
            var cache = new MemCacheController()
            {
                myApplication = _app
            };

            int maxRowsToCache = 1000;

            using (var db = new AutorizadorCNDB())
            {
                db.currentLojista = currentUser;

                // ----------------------------------------------------------------------------------------------------

                foreach (var item in new EnumMonth().lst) cache.StoreCache(CacheTags.EnumMonth, item.id, item);
                foreach (var item in new EnumTipoVenda().lst) cache.StoreCache(CacheTags.EnumMonth, item.id, item);

                // ----------------------------------------------------------------------------------------------------

                {
                    var hshReport = cache.SetupCacheReport(CacheTags.EnumMonthReport);
                    var query = (from e in new EnumMonth().lst select e);
                    var ret = new { count = query.Count(), results = query.ToList() };
                    hshReport[""] = ret;
                }

                {
                    var hshReport = cache.SetupCacheReport(CacheTags.EnumTipoVendaReport);
                    var query = (from e in new EnumTipoVenda().lst select e);
                    var ret = new { count = query.Count(), results = query.ToList() };
                    hshReport[""] = ret;
                }
            }
        }
    }
}