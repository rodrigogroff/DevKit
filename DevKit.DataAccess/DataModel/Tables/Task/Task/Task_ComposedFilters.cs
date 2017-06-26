using LinqToDB;
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
    public class TaskFilter : BaseFilter
    {
        public bool? complete, kpa, expired;

        public long? nuPriority,
                        fkProject,
                        fkClient,
                        fkClientGroup,
                        fkPhase,
                        fkSprint,
                        fkUserStart,
                        fkUserResponsible,
                        fkTaskType,
                        fkTaskFlowCurrent,
                        fkTaskCategory;

        public List<long?> lstProjects = null;

        public string Parameters()
        {
            return Export();
        }

        string Export()
        {
            var ret = new StringBuilder();

            // base
            ret.Append(skip + ",");
            ret.Append(take + ",");
            ret.Append(busca + ",");

            // customs
            if (complete != null)
                ret.Append(complete);
            ret.Append(",");

            if (kpa != null)
                ret.Append(kpa);
            ret.Append(",");

            if (expired != null)
                ret.Append(expired);
            ret.Append(",");

            if (nuPriority != null)
                ret.Append(nuPriority);
            ret.Append(",");

            if (fkProject != null)
                ret.Append(fkProject);
            ret.Append(",");

            if (fkClient != null)
                ret.Append(fkClient);
            ret.Append(",");

            if (fkClientGroup != null)
                ret.Append(fkClientGroup);
            ret.Append(",");

            if (fkPhase != null)
                ret.Append(fkPhase);
            ret.Append(",");

            if (fkSprint != null)
                ret.Append(fkSprint);
            ret.Append(",");

            if (fkUserStart != null)
                ret.Append(fkUserStart);
            ret.Append(",");

            if (fkUserResponsible != null)
                ret.Append(fkUserResponsible);
            ret.Append(",");

            if (fkTaskType != null)
                ret.Append(fkTaskType);
            ret.Append(",");

            if (fkTaskFlowCurrent != null)
                ret.Append(fkTaskFlowCurrent);
            ret.Append(",");

            if (fkTaskCategory != null)
                ret.Append(fkTaskCategory);
            
            return ret.ToString();
        }
    }

    public partial class Task
	{
		public TaskReport ComposedFilters(DevKitDB db, TaskFilter filter, loaderOptionsTask options )
		{
			var query = from e in db.Task select e;

            if (!string.IsNullOrEmpty(filter.busca))
            {
                query = from e in query
                        where e.stDescription.ToUpper().Contains(filter.busca) ||
                                e.stLocalization.ToUpper().Contains(filter.busca) ||
                                e.stTitle.ToUpper().Contains(filter.busca) ||
                                e.stProtocol.ToUpper().Contains(filter.busca)
                        select e;
            }                           
            
            if (filter.expired != null)
            {
                if (filter.expired == true)
                    query = from e in query where e.dtExpired != null && DateTime.Now > e.dtExpired select e;
                else
                    query = from e in query where e.dtExpired != null && DateTime.Now < e.dtExpired select e;
            }

            if (filter.nuPriority != null)
                query = from e in query where e.nuPriority == filter.nuPriority select e;

            if (filter.fkProject != null)
                query = from e in query where e.fkProject == filter.fkProject select e;

            if (filter.lstProjects != null)
                query = from e in query where filter.lstProjects.Contains(e.fkProject) select e;

            if (filter.fkPhase != null)
                query = from e in query where e.fkPhase == filter.fkPhase select e;

            if (filter.fkTaskType != null)
                query = from e in query where e.fkTaskType == filter.fkTaskType select e;

            if (filter.fkTaskCategory != null)
                query = from e in query where e.fkTaskCategory == filter.fkTaskCategory select e;

            if (filter.fkUserStart != null)
                query = from e in query where e.fkUserStart == filter.fkUserStart select e;

            if (filter.fkUserResponsible != null)
                query = from e in query where e.fkUserResponsible == filter.fkUserResponsible select e;

            if (filter.fkTaskFlowCurrent != null)
                query = from e in query where e.fkTaskFlowCurrent == filter.fkTaskFlowCurrent select e;

            if (filter.complete != null)
                query = from e in query where e.bComplete == filter.complete select e;

            if (filter.kpa != null)
            {
                var queryAux = from e in db.TaskType
                                where e.bKPA == filter.kpa
                                where filter.fkTaskType == null || e.id == filter.fkTaskType
                                where filter.fkProject == null || e.fkProject == filter.fkProject
                                select e.id;

                query = from e in query where queryAux.Contains((long)e.fkTaskType) select e;
            }

            if (filter.fkClient != null)
            {
                var queryAux = from e in db.TaskClient
                                where filter.fkClient == e.fkClient
                                select e.fkTask;

                query = from e in query where queryAux.Contains(e.id) select e;
            }

            if (filter.fkClientGroup != null)
            {
                var queryAux = from e in db.TaskClientGroup
                                where filter.fkClientGroup == e.fkClientGroup
                                select e.fkTask;

                query = from e in query
                        where queryAux.Contains(e.id)
                        select e;
            }            

			var count = query.Count();

			query = query.OrderBy(y => y.nuPriority).
                          ThenBy(y => y.fkSprint);

            var results = query.Skip(() => filter.skip).
                                Take(() => filter.take).
                                ToList();
            
            results = Loader(db, results, options );

            var resultsListing = new List<TaskListing>();

            results.ForEach( item =>  {
                    resultsListing.Add ( new TaskListing
                    {
                        sdtStart = item.sdtStart,
                        sfkPhase = item.sfkPhase,
                        sfkProject = item.sfkProject,
                        sfkSprint = item.sfkSprint,
                        sfkTaskCategory = item.sfkTaskCategory,
                        sfkTaskFlowCurrent = item.sfkTaskFlowCurrent,
                        sfkTaskType = item.sfkTaskType,
                        sfkUserResponsible = item.sfkTaskType,
                        sfkUserStart = item.sfkUserStart,
                        sfkVersion = item.sfkVersion,
                        snuPriority = item.snuPriority,
                        stLocalization = item.stLocalization,
                        stProtocol = item.stProtocol,
                        stTitle = item.stTitle
                    });
                });

            return new TaskReport
            {
                count = count,
                results = resultsListing
            };
		}
	}
}
