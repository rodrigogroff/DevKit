
namespace DevKit.Web.Controllers
{
    public class CacheClean : MemCacheController
    {
        public void Clean (string target, long? id)
        {
            switch (target)
            {
                case CacheObject.User:          if (id != null) StoreCache(CacheObject.User, id, null);
                                                CleanCacheReport(CacheObject.UserReports);
                                                break;

                case CacheObject.Profile:       if (id != null) StoreCache(CacheObject.Profile, id, null);
                                                CleanCacheReport(CacheObject.ProfileReports);
                                                break;

                case CacheObject.Client:        if (id != null) StoreCache(CacheObject.Client, id, null);
                                                CleanCacheReport(CacheObject.ClientReports);
                                                break;

                case CacheObject.ClientGroup:   if (id != null) StoreCache(CacheObject.ClientGroup, id, null);
                                                CleanCacheReport(CacheObject.ClientGroupReports);
                                                break;

                case CacheObject.Task:          if (id != null) StoreCache(CacheObject.Task, id, null);
                                                CleanCacheReport(CacheObject.TaskReports);
                                                break;

                case CacheObject.CompanyNews:   if (id != null) StoreCache(CacheObject.CompanyNews, id, null);
                                                CleanCacheReport(CacheObject.CompanyNewsReports);
                                                break;
            }
        }
    }
}
