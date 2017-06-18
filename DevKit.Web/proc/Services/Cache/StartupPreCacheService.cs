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

            int count = 0, 
                maxRowsToCache = 1000;

            using (var db = new DevKitDB())
            {
                db.currentUser = currentUser;

                foreach (var item in new EnumAccumulatorType().lst) cache.StoreCache(CacheTags.EnumAccumulatorType, item.id, item);
                foreach (var item in new EnumMonth().lst) cache.StoreCache(CacheTags.EnumMonth, item.id, item);
                foreach (var item in new EnumPriority().lst) cache.StoreCache(CacheTags.EnumPriority, item.id, item);
                foreach (var item in new EnumProjectTemplate().lst) cache.StoreCache(CacheTags.EnumProjectTemplate, item.id, item);
                foreach (var item in new EnumVersionState().lst) cache.StoreCache(CacheTags.EnumVersionState, item.id, item);
                
                {
                    var hshReport = cache.SetupCacheReport(CacheTags.EnumAccumulatorTypeReport);
                    var query = (from e in new EnumAccumulatorType().lst select e);
                    var ret = new { count = query.Count(), results = query.ToList() };
                    hshReport[""] = ret;
                }
                
                {
                    var hshReport = cache.SetupCacheReport(CacheTags.EnumMonth);
                    var query = (from e in new EnumMonth().lst select e);
                    var ret = new { count = query.Count(), results = query.ToList() };
                    hshReport[""] = ret;
                }

                {
                    var hshReport = cache.SetupCacheReport(CacheTags.EnumPriority);
                    var query = (from e in new EnumPriority().lst select e);
                    var ret = new { count = query.Count(), results = query.ToList() };
                    hshReport[""] = ret;
                }

                {
                    var hshReport = cache.SetupCacheReport(CacheTags.EnumProjectTemplate);
                    var query = (from e in new EnumProjectTemplate().lst select e);
                    var ret = new { count = query.Count(), results = query.ToList() };
                    hshReport[""] = ret;
                }

                {
                    var hshReport = cache.SetupCacheReport(CacheTags.EnumVersionState);
                    var query = (from e in new EnumVersionState().lst select e);
                    var ret = new { count = query.Count(), results = query.ToList() };
                    hshReport[""] = ret;
                }
                
                // user
                {
                    var hshReport = cache.SetupCacheReport(CacheTags.UserReports);

                    var mdl = new User();
                    var filter = new UserFilter { skip = 0, take = 15 };
                    var results = mdl.ComposedFilters(db, ref count, filter);
                    var ret = new UserReport
                    {
                        count = count,
                        results = results
                    };
                    hshReport[filter.Parameters()] = ret;
                }

                // profile
                {
                    var hshReport = cache.SetupCacheReport(CacheTags.ProfileReports);

                    var mdl = new Profile();
                    var filter = new ProfileFilter { skip = 0, take = 15 };
                    var results = mdl.ComposedFilters(db, ref count, filter);
                    var ret = new ProfileReport
                    {
                        count = count,
                        results = results
                    };
                    hshReport[filter.Parameters()] = ret;
                }

                // client
                {
                    var hshReport = cache.SetupCacheReport(CacheTags.ClientReports);

                    var mdl = new Client();
                    var filter = new ClientFilter { skip = 0, take = 15 };
                    var results = mdl.ComposedFilters(db, ref count, filter, false);
                    var ret = new ClientReport
                    {
                        count = count,
                        results = results
                    };
                    hshReport[filter.Parameters()] = ret;
                }

                // client groups
                {
                    var hshReport = cache.SetupCacheReport(CacheTags.ClientGroupReports);

                    var mdl = new ClientGroup();
                    var filter = new ClientGroupFilter { skip = 0, take = 15 };
                    var results = mdl.ComposedFilters(db, ref count, filter, false);
                    var ret = new ClientGroupReport
                    {
                        count = count,
                        results = results
                    };
                    hshReport[filter.Parameters()] = ret;
                }

                // task type
                {
                    var hshReport = cache.SetupCacheReport(CacheTags.TaskTypeReports);

                    var mdl = new TaskType();
                    var filter = new TaskTypeFilter { skip = 0, take = 15 };

                    var options = new loaderOptionsTaskType
                    {
                        bLoadProject = false,
                        bLoadCategories = false
                    };
                    
                    var results = mdl.ComposedFilters(db, ref count, filter, options);
                    var ret = new TaskTypeReport
                    {
                        count = count,
                        results = results
                    };
                    hshReport[filter.Parameters()] = ret;
                }

                // tasks
                {
                    var hshReport = cache.SetupCacheReport(CacheTags.TaskReports);

                    var mdl = new Task();
                    var filter = new TaskFilter { skip = 0, take = 15, kpa = false, complete = false };

                    var options = new loaderOptionsTask
                    {
                        bLoadTaskCategory = true,
                        bLoadTaskType = true,
                        bLoadProject = true,
                        bLoadPhase = true,
                        bLoadSprint = true,
                        bLoadTaskFlow = true,
                        bLoadVersion = true,
                        bLoadUsers = true,
                    };

                    var results = mdl.ComposedFilters(db, ref count, filter, options);

                    var ret = new TaskReport
                    {
                        count = count,
                        results = results
                    };

                    hshReport[filter.Parameters()] = ret;
                }
                
                // user
                {
                    var q = (from e in db.User select e);
                    if (q.Count() < maxRowsToCache)
                        foreach (var item in q.ToList())
                        {
                            item.LoadAssociations(db);
                            cache.StoreCache(CacheTags.User, item.id, item);
                        }
                }

                // profile
                {
                    var q = (from e in db.Profile select e);
                    if (q.Count() < maxRowsToCache)
                        foreach (var item in q.ToList())
                        {
                            item.LoadAssociations(db);
                            cache.currentCacheTag = CacheTags.Profile + item.id;
                            cache.BackupCache(item);
                        }
                }

                // client
                {
                    var q = (from e in db.Client select e);
                    if (q.Count() < maxRowsToCache)
                        foreach (var item in q.ToList())
                        {
                            item.LoadAssociations(db);

                            cache.currentCacheTag = CacheTags.Client + item.id;
                            cache.BackupCache(item);
                        }                            
                }

                // client groups
                {
                    var q = (from e in db.ClientGroup select e);
                    if (q.Count() < maxRowsToCache)
                        foreach (var item in q.ToList())
                        {
                            item.LoadAssociations(db);

                            cache.currentCacheTag = CacheTags.ClientGroup + item.id;
                            cache.BackupCache(item);
                        }                            
                }

                // task type
                {
                    var q = (from e in db.TaskType select e);

                    var options = new loaderOptionsTaskType
                    {
                        bLoadProject = false,
                        bLoadCategories = false,
                        bLoadCheckPoints = false,
                        bLoadLogs = false
                    };

                    if (q.Count() < maxRowsToCache)
                        foreach (var item in q.ToList())
                        {
                            item.LoadAssociations(db, options);

                            cache.currentCacheTag = CacheTags.TaskType + item.id;
                            cache.BackupCache(item);
                        }
                }

                // tasks
                {
                    var options = new loaderOptionsTask
                    {
                        bLoadTaskCategory = true,
                        bLoadTaskType = true,
                        bLoadProject = true,
                        bLoadPhase = true,
                        bLoadSprint = true,
                        bLoadTaskFlow = true,
                        bLoadVersion = true,
                        bLoadUsers = true,
                        bLoadProgress = true,
                        bLoadMessages = true,
                        bLoadFlows = true,
                        bLoadAccs = true,
                        bLoadDependencies = true,
                        bLoadCheckpoints = true,
                        bLoadQuestions = true,
                        bLoadClients = true,
                        bLoadClientGroups = true,
                        bLoadCustomSteps = true,
                        bLoadLogs = true
                    };

                    var q = (from e in db.Task select e);
                    if (q.Count() < maxRowsToCache)
                        foreach (var item in q.ToList())
                        {
                            item.LoadAssociations(db, options);

                            cache.currentCacheTag = CacheTags.Task + item.id;
                            cache.BackupCache(item);
                        }                            
                }
            }
        }
    }
}