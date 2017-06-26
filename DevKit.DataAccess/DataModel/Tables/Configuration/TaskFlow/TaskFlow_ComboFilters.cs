using LinqToDB;
using System.Linq;

namespace DataModel
{
	public partial class TaskFlow
	{
        public ComboReport ComboFilters(DevKitDB db, string searchItem, long? _fkTaskType, long? _fkTaskCategory)
        {
            var query = from e in db.TaskFlow select e;

            if (searchItem != "")
                query = from e in query
                        where e.stName.ToUpper().Contains(searchItem)
                        select e;

            if (_fkTaskType != null)
                query = from e in query where e.fkTaskType == _fkTaskType select e;

            if (_fkTaskCategory != null)
                query = from e in query where e.fkTaskCategory == _fkTaskCategory select e;

            query = from e in query orderby e.stName select e;

            return new ComboReport
            {
                count = query.Count(),
                results = (from e in query select new BaseComboResponse { id = e.id, stName = e.stName }).ToList()
            };
        }
	}
}
