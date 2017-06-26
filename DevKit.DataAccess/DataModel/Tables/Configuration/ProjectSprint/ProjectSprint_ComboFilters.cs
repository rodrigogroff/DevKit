using LinqToDB;
using System.Linq;

namespace DataModel
{
	public partial class ProjectSprint
	{
        public ComboReport ComboFilters(DevKitDB db, string searchItem, long? _fkProject, long? _fkPhase)
        {
            var query = from e in db.ProjectSprint select e;

            if (searchItem != "")
                query = from e in query
                        where e.stName.ToUpper().Contains(searchItem)
                        select e;

            if (_fkProject != null)
            {
                query = from e in query
                        where e.fkProject == _fkProject
                        select e;
            }

            if (_fkPhase != null)
            {
                query = from e in query
                        where e.fkPhase == _fkPhase
                        select e;
            }

            query = from e in query orderby e.stName select e;

            return new ComboReport
            {
                count = query.Count(),
                results = (from e in query select new BaseComboResponse { id = e.id, stName = e.stName }).ToList()
            };
        }
    }
}
