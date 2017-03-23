using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public class TaskAccumulatorFilter
	{
		public int skip, take;
		public string busca;

		public long? fkTaskCategory;		
	}

	// --------------------------
	// properties
	// --------------------------

	public partial class TaskTypeAccumulator
	{
		public string sfkFlow = "";

		public List<LogAccumulatorValue> logs = new List<LogAccumulatorValue>();
	}

	// extra class

	public class LogAccumulatorValue
	{
		public string sfkUser = "",
			          sdtLog = "",
					  sValue = "";
	}
	
	// --------------------------
	// functions
	// --------------------------

	public partial class TaskTypeAccumulator
	{
		public IQueryable<TaskTypeAccumulator> ComposedFilters(DevKitDB db, TaskAccumulatorFilter filter)
		{
			var query = from e in db.TaskTypeAccumulators select e;

			if (filter.fkTaskCategory != null)
				query = from e in query where e.fkTaskCategory == filter.fkTaskCategory select e;

			if (filter.busca != null)
				query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			return query;
		}

		public TaskTypeAccumulator LoadAssociations(DevKitDB db)
		{
			sfkFlow = db.TaskFlow(fkTaskFlow).stName;

			return this;
		}		
	}
}
