using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace DataModel
{
	public class ProjectSprintFilter
	{
		public int skip, take;

		public long? fkProject, fkPhase;

		public string busca;
	}

	// --------------------------
	// properties
	// --------------------------

	public partial class ProjectSprint
	{
		public string sdtStart = "";
		public string sdtEnd = "";
		public string sfkProject = "";
		public string sfkPhase = "";

		public string updateCommand = "";
		public object anexedEntity;

		public List<ProjectSprintVersion> versions;
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

			if (filter.fkProject != null)
				query = from e in query where e.fkProject == filter.fkProject select e;

			if (filter.fkPhase != null)
				query = from e in query where e.fkPhase == filter.fkPhase select e;

			return query;
		}

		public ProjectSprint LoadAssociations(DevKitDB db)
		{
			var setup = db.Setup();

			sdtStart = dtStart?.ToString(setup.stDateFormat);
			sdtEnd = dtEnd?.ToString(setup.stDateFormat);

			if (fkProject != null)
				sfkProject = db.Project(fkProject).stName;

			if (fkPhase != null)
				sfkPhase = db.ProjectPhase(fkPhase).stName;

			versions = LoadVersions(db);

			return this;
		}

		List<ProjectSprintVersion> LoadVersions(DevKitDB db)
		{
			var lst = (from e in db.ProjectSprintVersions where e.fkSprint == id select e).
				OrderByDescending(t => t.id).
				ToList();

			return lst;
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

				case "newVersion":
					{
						var ent = JsonConvert.DeserializeObject<ProjectSprintVersion>(anexedEntity.ToString());

						if ((from ne in db.ProjectSprintVersions
							 where ne.stName == ent.stName && ne.fkSprint == id
							 select ne).Any())
						{
							resp = "Version already added to project!";
							return false;
						}

						ent.fkSprint = id;
						
						db.Insert(ent);
						versions = LoadVersions(db);
						break;
					}

				case "removeVersion":
					{
						var versionDel = JsonConvert.DeserializeObject<ProjectSprintVersion>(anexedEntity.ToString());

						if ((from e in db.Tasks where e.fkVersion == id select e).Any())
						{
							resp = "This version is being used in a task";
							return false;
						}

						db.Delete(versionDel);
						versions = LoadVersions(db);
						break;
					}
			}

			return true;
		}

		public bool CanDelete(DevKitDB db, ref string resp)
		{
			if ((from e in db.Tasks where e.fkSprint == id select e).Any())
			{
				resp = "This sprint is being used in a task";
				return false;
			}

			return true;
		}

		public void Delete(DevKitDB db)
		{
			foreach (var item in (from e in db.ProjectSprintVersions
								  where e.fkSprint == id
								  select e))
				db.Delete(item);

			db.Delete(this);
		}
	}
}
