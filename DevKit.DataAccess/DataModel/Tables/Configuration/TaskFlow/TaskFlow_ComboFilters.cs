using LinqToDB;
using System.Linq;

namespace DataModel
{
	public partial class TaskFlow
	{
        public ComboReport ComboFilters(DevKitDB db, TaskFlowFilter filter)
        {
            var query = from e in db.TaskFlow select e;

            if (filter.busca != "")
                query = from e in query
                        where e.stName.ToUpper().Contains(filter.busca)
                        select e;

            if (filter.fkTaskType != null)
                query = from e in query
                        where e.fkTaskType == filter.fkTaskType
                        select e;

            if (filter.fkTaskCategory != null)
                query = from e in query
                        where e.fkTaskCategory == filter.fkTaskCategory
                        select e;

            query = from e in query orderby e.stName select e;

            return new ComboReport
            {
                count = query.Count(),
                results = (from e in query select new BaseComboResponse { id = e.id, stName = e.stName }).ToList()
            };
        }
	}
}
