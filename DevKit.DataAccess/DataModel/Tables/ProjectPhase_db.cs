using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using DataModel;

namespace DataModel
{
	public class ProjectPhaseLoadParams
	{
		public bool bAll = false;
	}

	public class ProjectPhaseFilter
	{
		public int skip, take;
		public int? fkProject;

		public string busca;
	}

	// --------------------------
	// properties
	// --------------------------

	public partial class ProjectPhase
	{
		
	}

	// --------------------------
	// functions
	// --------------------------

	public partial class ProjectPhase
	{
		ProjectPhaseLoadParams load = new ProjectPhaseLoadParams { bAll = true };
		
		public IQueryable<ProjectPhase> ComposedFilters(DevKitDB db, ProjectPhaseFilter filter)
		{
			var query = from e in db.ProjectPhases select e;

			if (filter.fkProject != null)
				query = from e in query where e.fkProject == filter.fkProject select e;

			if (filter.busca != null)
				query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			return query;
		}

		public ProjectPhase Load(DevKitDB db, ProjectPhaseLoadParams _load = null)
		{
			if (_load != null)
				load = _load;

			return this;
		}
	}
}
