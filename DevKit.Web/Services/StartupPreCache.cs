using DataModel;
using DevKit.Web.Controllers;
using LinqToDB;
using System.Linq;

namespace DevKit.Web.Schedulers
{
    public class StartupPreCache 
    {
        public void Run()
        {
            var cache = new MemCacheController();
            var defaultCacheTop = 1000;

            using (var db = new DevKitDB())
            {
                // enums

                foreach (var item in new EnumAccumulatorType().lst) cache.StoreCache(CacheObject.EnumAccumulatorType, item.id, item);
                foreach (var item in new EnumMonth().lst) cache.StoreCache(CacheObject.EnumMonth, item.id, item);
                foreach (var item in new EnumPriority().lst) cache.StoreCache(CacheObject.EnumPriority, item.id, item);
                foreach (var item in new EnumProjectTemplate().lst) cache.StoreCache(CacheObject.EnumProjectTemplate, item.id, item);
                foreach (var item in new EnumVersionState().lst) cache.StoreCache(CacheObject.EnumVersionState, item.id, item);

                new AccumulatorTypeController { IsPreCached = true }.Get();
                new MonthController { IsPreCached = true }.Get();
                new PriorityController { IsPreCached = true }.Get();
                new ProjectTemplateController { IsPreCached = true }.Get();
                new VersionStateController { IsPreCached = true }.Get();

                // tables

                {
                    var q = (from e in db.User select e);                    
                    if (q.Count() < defaultCacheTop)
                        foreach (var item in q.ToList())
                            cache.StoreCache(CacheObject.User, item.id, item);
                }

                {
                    var q = (from e in db.Client select e);
                    if (q.Count() < defaultCacheTop)
                        foreach (var item in q.ToList())
                            cache.StoreCache(CacheObject.Client, item.id, item);
                }

                {
                    var q = (from e in db.ClientGroup select e);
                    if (q.Count() < defaultCacheTop)
                        foreach (var item in q.ToList())
                            cache.StoreCache(CacheObject.ClientGroup, item.id, item);
                }

                {
                    var q = (from e in db.Task select e);
                    if (q.Count() < defaultCacheTop)
                        foreach (var item in q.ToList())
                            cache.StoreCache(CacheObject.ClientGroup, item.id, item);
                }
            }
        }
    }
}