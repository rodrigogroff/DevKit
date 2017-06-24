
namespace DevKit.Web.Controllers
{
    public class CacheClean : MemCacheController
    {
        public void Clean(string target, long? id)
        {
            switch (target)
            {
                case CacheTags.User:
                    if (id != null)
                    {
                        StoreCache(CacheTags.User, id, null);
                        StoreCache(CacheTags.UserCombo, id, null); // ok
                    }
                    CleanCacheReport(CacheTags.UserReport);
                    CleanCacheReport(CacheTags.UserComboReport);
                    break;

                case CacheTags.Profile:
                    if (id != null)
                    {
                        StoreCache(CacheTags.Profile, id, null);
                        StoreCache(CacheTags.ProfileCombo, id, null); // ok
                    }
                    CleanCacheReport(CacheTags.ProfileReport);
                    CleanCacheReport(CacheTags.ProfileComboReport);
                    break;

                case CacheTags.Client:
                    if (id != null)
                    {
                        StoreCache(CacheTags.Client, id, null);
                        StoreCache(CacheTags.ClientCombo, id, null); // ok
                    }
                    CleanCacheReport(CacheTags.ClientReport);
                    CleanCacheReport(CacheTags.ClientComboReport);
                    break;

                case CacheTags.ClientGroup:
                    if (id != null)
                    {
                        StoreCache(CacheTags.ClientGroup, id, null);
                        StoreCache(CacheTags.ClientGroupCombo, id, null); // ok
                    }
                    CleanCacheReport(CacheTags.ClientGroupReport);
                    CleanCacheReport(CacheTags.ClientGroupComboReport);
                    break;

                case CacheTags.Task:
                    if (id != null) StoreCache(CacheTags.Task, id, null);
                    CleanCacheReport(CacheTags.TaskReport);
                    break;

                case CacheTags.CompanyNews:
                    if (id != null) StoreCache(CacheTags.CompanyNews, id, null);
                    CleanCacheReport(CacheTags.CompanyNewsReport);
                    break;

                case CacheTags.TaskType:
                    if (id != null)
                    {
                        StoreCache(CacheTags.TaskType, id, null);
                        StoreCache(CacheTags.TaskTypeCombo, id, null);
                    }
                    CleanCacheReport(CacheTags.TaskTypeReport);
                    CleanCacheReport(CacheTags.TaskTypeComboReport);
                    break;

                case CacheTags.Project:
                    if (id != null)
                    {
                        StoreCache(CacheTags.Project, id, null);
                        StoreCache(CacheTags.ProjectCombo, id, null); // ok
                    }
                    CleanCacheReport(CacheTags.ProjectReport);
                    CleanCacheReport(CacheTags.ProjectComboReport);
                    break;

                case CacheTags.ProjectPhase:
                    if (id != null)
                    {
                        StoreCache(CacheTags.ProjectPhase, id, null);
                        StoreCache(CacheTags.ProjectPhaseCombo, id, null);
                    }
                    CleanCacheReport(CacheTags.ProjectPhaseReport);
                    CleanCacheReport(CacheTags.ProjectPhaseComboReport);
                    break;

                case CacheTags.ProjectSprint:
                    if (id != null)
                    {
                        StoreCache(CacheTags.ProjectSprint, id, null);
                        StoreCache(CacheTags.ProjectSprintCombo, id, null);
                    }
                    CleanCacheReport(CacheTags.ProjectSprintReport);
                    CleanCacheReport(CacheTags.ProjectSprintComboReport);
                    break;

                case CacheTags.Survey:
                    if (id != null) StoreCache(CacheTags.Survey, id, null);
                    CleanCacheReport(CacheTags.SurveyReport);
                    break;

                case CacheTags.TaskTypeAccumulator:
                    if (id != null) StoreCache(CacheTags.TaskTypeAccumulator, id, null);
                    CleanCacheReport(CacheTags.TaskTypeAccumulatorReport);
                    break;
            }
        }
    }
}
