using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace DataModel
{
	public class TaskFilter
	{
		public int skip, take;
		public string busca;
	}

	// --------------------------
	// properties
	// --------------------------

	public partial class Task
	{
		public string sdtStart = "";
		public string sfkUserStart = "";
	
		public string updateCommand = "";
		public object anexedEntity;
	}

	// --------------------------
	// functions
	// --------------------------

	public partial class Task
	{
		public IQueryable<Task> ComposedFilters(DevKitDB db, TaskFilter filter)
		{
			var query = from e in db.Tasks select e;

			if (filter.busca != null)
				query = from e in query
						where	e.stDescription.ToUpper().Contains(filter.busca) ||
								e.stLocalization.ToUpper().Contains(filter.busca) ||
								e.stTitle.ToUpper().Contains(filter.busca) 
						select e;

			return query;
		}

		public Task LoadAssociations(DevKitDB db)
		{
			var setup = db.Setups.Find(1);

			sdtStart = dtStart?.ToString(setup.stDateFormat);
			sfkUserStart = (from e in db.Users where e.id == fkUserStart select e).FirstOrDefault().stLogin;

			return this;
		}
		
		bool CheckDuplicate(Task item, DevKitDB db)
		{
			var query = from e in db.Tasks select e;

			if (item.stTitle != null)
			{
				var _st = item.stTitle.ToUpper();
				query = from e in query where e.stTitle.ToUpper().Contains(_st) select e;
			}

			if (item.fkProject != null)
				query = from e in query where e.fkProject == item.fkProject select e;

			if (item.fkSprint != null)
				query = from e in query where e.fkSprint == item.fkSprint select e;

			if (item.id > 0)
				query = from e in query where e.id != item.id select e;

			return query.Any();
		}

		public bool Create(DevKitDB db, User usr, ref string resp)
		{
			if (CheckDuplicate(this, db))
			{
				resp = "Task title cannot be duplicated";
				return false;
			}

			dtStart = DateTime.Now;
			fkUserStart = usr.id;
			
			id = Convert.ToInt64(db.InsertWithIdentity(this));

			return true;
		}

		public bool Update(DevKitDB db, ref string resp)
		{
			if (CheckDuplicate(this, db))
			{
				resp = "Task title cannot be duplicated";
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

		public void Delete(DevKitDB db)
		{
			db.Delete(this);
		}
	}
}
