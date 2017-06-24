using LinqToDB;
using System.Linq;

namespace DataModel
{	
	public partial class TaskCategory
	{
        public ComboReport ComboFilters(DevKitDB db, string searchItem, long? _fkTaskType)
        {
            var query = from e in db.TaskCategory select e;

            if (searchItem != "")
                query = from e in query
                        where e.stName.ToUpper().Contains(searchItem)
                        select e;

            if (_fkTaskType != null)
            {
                query = from e in query
                        where e.fkTaskType == _fkTaskType
                        select e;
            }

            return new ComboReport
            {
                count = query.Count(),
                results = (from e in query select new BaseComboResponse { id = e.id, stName = e.stName }).ToList()
            };
        }
	}
}
