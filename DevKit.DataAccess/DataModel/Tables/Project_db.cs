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

		public string updateCommand = "";
		public object anexedEntity;
	}

	public partial class ProjectUser
	{
		public string stUser = "";
		public string sdtJoin = "";
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

			users = Loadusers(db);

			return this;
		}

		List<ProjectUser> Loadusers(DevKitDB db)
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

						if ((from ne in db.ProjectUsers where ne.fkUser == ent.fkUser select ne).Any())
						{
							resp = "User duplicated!";
							return false;
						}

						ent.dtJoin = DateTime.Now;

						db.Insert(ent);
						users = Loadusers(db);
						break;
					}

				case "removeUser":
					{
						db.Delete(JsonConvert.DeserializeObject<ProjectUser>(anexedEntity.ToString()));
						users = Loadusers(db);
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
