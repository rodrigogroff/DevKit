using LinqToDB;
using System.Linq;

namespace DataModel
{	
	public partial class TaskCategory
	{
        public ComboReport ComboFilters(DevKitDB db, TaskCategoryFilter filter)
        {
            var query = from e in db.TaskCategory select e;

            if (filter.busca != "")
                query = from e in query
                        where e.stName.ToUpper().Contains(filter.busca)
                        select e;

            if (filter.fkTaskType != null)
                query = from e in query
                        where e.fkTaskType == filter.fkTaskType
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
