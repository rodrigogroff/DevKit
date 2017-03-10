using LinqToDB;
using System.Linq;

namespace DataModel
{
	public class ProjectPhaseFilter
	{
		public int skip, take;
		public int? fkProject;

		public string busca;
	}
	
	// --------------------------
	// functions
	// --------------------------

	public partial class ProjectPhase
	{
		public IQueryable<ProjectPhase> ComposedFilters(DevKitDB db, ProjectPhaseFilter filter)
		{
			var query = from e in db.ProjectPhases select e;

			if (filter.fkProject != null)
				query = from e in query where e.fkProject == filter.fkProject select e;

			if (filter.busca != null)
				query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			return query;
		}

		public ProjectPhase LoadAssociations(DevKitDB db)
		{

			return this;
		}
	}
}
