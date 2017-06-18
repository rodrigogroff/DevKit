
namespace DevKit.Web.Controllers
{
    public class CacheClean : MemCacheController
    {
        public void Clean (string target, long? id)
        {
            switch (target)
            {
                case CacheTags.User:            if (id != null) StoreCache(CacheTags.User, id, null);
                                                CleanCacheReport(CacheTags.UserReports);
                                                break;

                case CacheTags.Profile:         if (id != null) StoreCache(CacheTags.Profile, id, null);
                                                CleanCacheReport(CacheTags.ProfileReports);
                                                break;

                case CacheTags.Client:          if (id != null) StoreCache(CacheTags.Client, id, null);
                                                CleanCacheReport(CacheTags.ClientReports);
                                                break;

                case CacheTags.ClientGroup:     if (id != null) StoreCache(CacheTags.ClientGroup, id, null);
                                                CleanCacheReport(CacheTags.ClientGroupReports);
                                                break;

                case CacheTags.Task:            if (id != null) StoreCache(CacheTags.Task, id, null);
                                                CleanCacheReport(CacheTags.TaskReports);
                                                break;

                case CacheTags.CompanyNews:     if (id != null) StoreCache(CacheTags.CompanyNews, id, null);
                                                CleanCacheReport(CacheTags.CompanyNewsReports);
                                                break;

                case CacheTags.TaskType:        if (id != null) StoreCache(CacheTags.TaskType, id, null);
                                                CleanCacheReport(CacheTags.TaskTypeReports);
                                                break;
            }
        }
    }
}
