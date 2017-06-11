using System.Web;

namespace DevKit.Web.Controllers
{
    public class CacheClean : MemCacheController
    {
        public void Clean (string target, long? id)
        {
            var app = HttpContext.Current.Application;

            switch (target)
            {
                case CacheObject.User:

                    if (id != null)
                        StoreCache(CacheObject.User, id, null);
                    break;

                case CacheObject.Client:

                    if (id != null)
                        StoreCache(CacheObject.Client, id, null);
                    CleanCacheReport(CacheObject.ClientReports);
                    break;

                case CacheObject.ClientGroup:

                    if (id != null)
                        StoreCache(CacheObject.ClientGroup, id, null);
                    CleanCacheReport(CacheObject.ClientGroupReports);
                    break;

                case CacheObject.Task:

                    StoreCache(CacheObject.Task, id, null);
                    CleanCacheReport(CacheObject.TaskReports);
                    break;
            }
        }
    }
}
