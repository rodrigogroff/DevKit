using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

namespace DataModel
{
	public class ProjectLoadParams
	{
		public bool bAll = false;
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

		public string updateCommand = "";
		public object anexedEntity;
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

			var mdlUser = (from e in db.Users where e.id == this.fkUser select e).FirstOrDefault();

			if (mdlUser!= null)
				stUser = mdlUser.stLogin;

			return this;
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

			this.dtCreation = DateTime.Now;
			this.fkUser = usr.id;
			
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
