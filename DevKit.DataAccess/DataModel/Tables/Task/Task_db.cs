using LinqToDB;
using System;
using System.Linq;

namespace DataModel
{
	public class TaskFilter
	{
		public int skip, take;

		public long?	nuPriority,
						fkProject;

		public string busca;
	}

	// --------------------------
	// properties
	// --------------------------

	public partial class Task
	{
		public string sdtStart = "";

		public string snuPriority = "";
		public string sfkUserStart = "";
		public string sfkProject = "";
		public string sfkPhase = "";
		public string sfkSprint = "";
		public string sfkVersion = "";

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

			if (filter.nuPriority != null)
				query = from e in query
						where e.nuPriority == filter.nuPriority
						select e;

			if (filter.fkProject != null)
				query = from e in query
						where e.fkProject == filter.fkProject
						select e;

			return query;
		}

		public Task LoadAssociations(DevKitDB db)
		{
			var setup = db.Setup();

			sdtStart = dtStart?.ToString(setup.stDateFormat);

			if (nuPriority != null)
				snuPriority = new EnumPriority().lst.Where(t => t.id == nuPriority).FirstOrDefault().stName;

			sfkUserStart = db.User(fkUserStart).stLogin;
			sfkProject = db.Project(fkProject).stName;
			sfkPhase = db.ProjectPhase(fkPhase).stName;
			sfkSprint = db.ProjectSprint(fkSprint).stName;
			sfkVersion = db.ProjectSprintVersion(fkVersion).stName;
			
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
