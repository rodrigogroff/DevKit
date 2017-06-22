
namespace DevKit.Web.Controllers
{
    public class CacheClean : MemCacheController
    {
        public void Clean (string target, long? id)
        {
            switch (target)
            {
                case CacheTags.User:            if (id != null) StoreCache(CacheTags.User, id, null);
                                                CleanCacheReport(CacheTags.UserReport);
                                                break;

                case CacheTags.Profile:         if (id != null) StoreCache(CacheTags.Profile, id, null);
                                                CleanCacheReport(CacheTags.ProfileReport);
                                                break;

                case CacheTags.Client:          if (id != null) StoreCache(CacheTags.Client, id, null);
                                                CleanCacheReport(CacheTags.ClientReport);
                                                break;

                case CacheTags.ClientGroup:     if (id != null) StoreCache(CacheTags.ClientGroup, id, null);
                                                CleanCacheReport(CacheTags.ClientGroupReport);
                                                break;

                case CacheTags.Task:            if (id != null) StoreCache(CacheTags.Task, id, null);
                                                CleanCacheReport(CacheTags.TaskReport);
                                                break;

                case CacheTags.CompanyNews:     if (id != null) StoreCache(CacheTags.CompanyNews, id, null);
                                                CleanCacheReport(CacheTags.CompanyNewsReport);
                                                break;

                case CacheTags.TaskType:        if (id != null) StoreCache(CacheTags.TaskType, id, null);
                                                CleanCacheReport(CacheTags.TaskTypeReport);
                                                break;

                case CacheTags.Project:         if (id != null) StoreCache(CacheTags.Project, id, null);
                                                CleanCacheReport(CacheTags.ProjectReport);
                                                break;

                case CacheTags.ProjectPhase:    if (id != null) StoreCache(CacheTags.ProjectPhase, id, null);
                                                CleanCacheReport(CacheTags.ProjectPhaseReport);
                                                break;

                case CacheTags.ProjectSprint:   if (id != null) StoreCache(CacheTags.ProjectSprint, id, null);
                                                CleanCacheReport(CacheTags.ProjectSprintReport);
                                                break;

                case CacheTags.Survey:          if (id != null) StoreCache(CacheTags.Survey, id, null);
                                                CleanCacheReport(CacheTags.SurveyReport);
                                                break;

                case CacheTags.TaskTypeAccumulator:     if (id != null) StoreCache(CacheTags.TaskTypeAccumulator, id, null);
                                                        CleanCacheReport(CacheTags.TaskTypeAccumulatorReport);
                                                        break;
            }
        }
    }
}
