using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public class TaskFilter
	{
		public int skip, take;
		public string busca;

		public bool? complete;

		public long? nuPriority,
						fkProject,
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
			var query = from e in db.Tasks select e;

			if (filter.busca != null)
				query = from e in query
						where	e.stDescription.ToUpper().Contains(filter.busca) ||
								e.stLocalization.ToUpper().Contains(filter.busca) ||
								e.stTitle.ToUpper().Contains(filter.busca) 
						select e;

			if (filter.complete != null)
				query = from e in query where e.bComplete == filter.complete select e;

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
			
			if (filter.fkUserResponsible != null)
				query = from e in query where e.fkUserResponsible == filter.fkUserResponsible select e;

			if (filter.complete != null)
				query = from e in query where e.bComplete == filter.complete select e;

			count = query.Count();

			query = query.OrderBy(y => y.nuPriority).ThenBy(y => y.fkSprint);

			var results = (query.Skip(() => filter.skip).Take(() => filter.take)).ToList();

			results.ForEach(y => { y = y.LoadAssociations(db); });

			return results;
		}
	}
}
