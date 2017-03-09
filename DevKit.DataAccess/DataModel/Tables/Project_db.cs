using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using DataModel;

namespace DataModel
{
	public class ProjectLoadParams
	{
		public bool bAll = false,
					bUsers = false;
	}

	public class ProjectFilter
	{
		public int skip, take;
		public string busca;
	}

	// --------------------------
	// properties
	// --------------------------

	public partial class Project
	{
		public string stUser = "";
		public string sdtCreation = "";

		public List<ProjectUser> users = new List<ProjectUser>();
		public List<ProjectPhase> phases = new List<ProjectPhase>();
		public List<ProjectSprint> sprints = new List<ProjectSprint>();

		public string updateCommand = "";
		public object anexedEntity;
	}

	public partial class ProjectUser
	{
		public string stUser = "";
		public string sdtJoin = "";
	}

	public partial class ProjectPhase
	{
		public string sdtStart = "";
		public string sdtEnd = "";
	}

	// --------------------------
	// functions
	// --------------------------

	public partial class Project
	{
		ProjectLoadParams load = new ProjectLoadParams { bAll = true };
		
		public IQueryable<Project> ComposedFilters(DevKitDB db, ProjectFilter filter)
		{
			var query = from e in db.Projects select e;

			if (filter.busca != null)
				query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			return query;
		}

		public Project Load(DevKitDB db, ProjectLoadParams _load = null)
		{
			if (_load != null)
				load = _load;

			var setup = db.Setups.Find(1);
			var mdlUser = db.Users.Find((long)this.fkUser);

			stUser = mdlUser?.stLogin;
			sdtCreation = dtCreation?.ToString(setup.stDateFormat);

			users = LoadUsers(db);
			phases = LoadPhases(db);
			sprints = LoadSprints(db);

			return this;
		}

		List<ProjectSprint> LoadSprints(DevKitDB db)
		{
			var lst = (from e in db.ProjectSprints where e.fkProject == id select e).
				OrderBy(t => t.id).
				ToList();

			return lst;
		}

		List<ProjectUser> LoadUsers(DevKitDB db)
		{
			var setup = db.Setups.Find(1);

			var lst = (from e in db.ProjectUsers where e.fkProject == id select e).
				OrderBy(t => t.id).
				ToList();

			foreach (var item in lst)
			{
				item.stUser = db.Users.Find((long)item.fkUser).stLogin;
				item.sdtJoin = item.dtJoin?.ToString(setup.stDateFormat);
			}

			return lst;
		}

		List<ProjectPhase> LoadPhases(DevKitDB db)
		{
			var setup = db.Setups.Find(1);

			var lst = (from e in db.ProjectPhases where e.fkProject == id select e).
				OrderBy(t => t.id).
				ToList();

			foreach (var item in lst)
			{
				item.sdtStart = item.dtStart?.ToString(setup.stDateFormat);
				item.sdtEnd = item.dtEnd?.ToString(setup.stDateFormat);
			}

			return lst;
		}

		bool CheckDuplicate(Project item, DevKitDB db)
		{
			var query = from e in db.Projects select e;

			if (item.stName != null)
			{
				var _st = item.stName.ToUpper();
				query = from e in query where e.stName.ToUpper().Contains(_st) select e;
			}

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

			dtCreation = DateTime.Now;
			fkUser = usr.id;
			
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

				case "newUser":
					{
						var ent = JsonConvert.DeserializeObject<ProjectUser>(anexedEntity.ToString());

						if ((from ne in db.ProjectUsers
							 where ne.fkUser == ent.fkUser && ne.fkProject == id
							 select ne).Any())
						{
							resp = "User already added to project!";
							return false;
						}

						ent.fkProject = id;
						ent.dtJoin = DateTime.Now;

						db.Insert(ent);
						users = LoadUsers(db);
						break;
					}

				case "removeUser":
					{
						db.Delete(JsonConvert.DeserializeObject<ProjectUser>(anexedEntity.ToString()));
						users = LoadUsers(db);
						break;
					}

				case "newPhase":
					{
						var ent = JsonConvert.DeserializeObject<ProjectPhase>(anexedEntity.ToString());

						if ((from ne in db.ProjectPhases
							 where ne.stName.ToUpper() == ent.stName.ToUpper() && ne.fkProject == id
							 select ne).Any())
						{
							resp = "Phase already added to project!";
							return false;
						}

						ent.fkProject = id;
						
						db.Insert(ent);
						phases = LoadPhases(db);
						break;
					}
										
				case "removePhase":
					{
						db.Delete(JsonConvert.DeserializeObject<ProjectPhase>(anexedEntity.ToString()));
						phases = LoadPhases(db);
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
