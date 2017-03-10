using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using DataModel;

namespace DataModel
{
	public class ProjectSprintFilter
	{
		public int skip, take;
		public string busca;
	}

	// --------------------------
	// properties
	// --------------------------

	public partial class ProjectSprint
	{
		public string sdtStart = "";
		public string sdtEnd = "";
		
		public string updateCommand = "";
		public object anexedEntity;
	}

	// --------------------------
	// functions
	// --------------------------

	public partial class ProjectSprint
	{
		public IQueryable<ProjectSprint> ComposedFilters(DevKitDB db, ProjectSprintFilter filter)
		{
			var query = from e in db.ProjectSprints select e;

			if (filter.busca != null)
				query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			return query;
		}

		public ProjectSprint Load(DevKitDB db)
		{
			var setup = db.Setups.Find(1);

			sdtStart = dtStart?.ToString(setup.stDateFormat);
			sdtEnd = dtEnd?.ToString(setup.stDateFormat);

			return this;
		}

		bool CheckDuplicate(ProjectSprint item, DevKitDB db)
		{
			var query = from e in db.ProjectSprints select e;

			if (item.stName != null)
			{
				var _st = item.stName.ToUpper();
				query = from e in query where e.stName.ToUpper().Contains(_st) select e;
			}

			if (item.fkPhase > 0)
				query = from e in query where e.fkPhase == item.fkPhase select e;

			if (item.id > 0)
				query = from e in query where e.id != item.id select e;

			return query.Any();
		}

		public bool Create(DevKitDB db, User usr, ref string resp)
		{
			if (CheckDuplicate(this, db))
			{
				resp = "Project name already taken";
				return false;
			}

			id = Convert.ToInt64(db.InsertWithIdentity(this));

			return true;
		}

		public bool Update(DevKitDB db, ref string resp)
		{
			if (CheckDuplicate(this, db))
			{
				resp = "Project name already taken";
				return false;
			}

			switch (updateCommand)
			{
				case "entity":
					{
						db.Update(this);
						break;
					}
			}

			return true;
		}

		public bool CanDelete(DevKitDB db, ref string resp)
		{
			return true;
		}
	}
}
