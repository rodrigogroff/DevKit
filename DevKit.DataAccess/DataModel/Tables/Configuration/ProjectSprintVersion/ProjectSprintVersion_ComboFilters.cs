using LinqToDB;
using System.Linq;

namespace DataModel
{
	public partial class ProjectSprintVersion
	{
        public ComboReport ComboFilters(DevKitDB db, string searchItem, long? _fkSprint)
        {
            var query = from e in db.ProjectSprintVersion select e;

            if (searchItem != "")
                query = from e in query
                        where e.stName.ToUpper().Contains(searchItem)
                        select e;

            if (_fkSprint != null)
                query = from e in query where e.fkSprint == _fkSprint select e;

            query = from e in query orderby e.stName select e;

            return new ComboReport
            {
                count = query.Count(),
                results = (from e in query select new BaseComboResponse { id = e.id, stName = e.stName }).ToList()
            };
        }
    }
}
