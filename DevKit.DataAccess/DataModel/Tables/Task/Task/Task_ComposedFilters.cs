using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System;

namespace DataModel
{
	public class TaskFilter
	{
		public int skip, take;
		public string busca;

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
	}

	public partial class Task
	{
		public List<Task> ComposedFilters(DevKitDB db, ref int count, TaskFilter filter)
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

                query = from e in query where queryAux.Contains((long)e.id) select e;
            }

            if (filter.fkClientGroup != null)
            {
                var queryAux = from e in db.TaskClientGroup
                                where filter.fkClientGroup == e.fkClientGroup
                                select e.fkTask;

                query = from e in query where queryAux.Contains((long)e.id) select e;
            }            

			count = query.Count();

			query = query.OrderBy(y => y.nuPriority).ThenBy(y => y.fkSprint);
            
			return Report(db, (query.Skip(() => filter.skip).Take(() => filter.take)).ToList(), true);
		}
	}
}
