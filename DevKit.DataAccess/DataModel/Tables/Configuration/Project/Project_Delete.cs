using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace DataModel
{
	// --------------------------
	// functions
	// --------------------------

	public partial class Project
	{		
		public bool CanDelete(DevKitDB db, ref string resp)
		{
			if ((from e in db.Tasks where e.fkProject == id select e).Any())
			{
				resp = "This project is being used in a task";
				return false;
			}

			return true;
		}

		public void Delete(DevKitDB db)
		{
			foreach (var item in (from e in db.ProjectSprints where e.fkProject == id select e))
			{
				item.Delete(db);
			}

			foreach (var item in (from e in db.ProjectPhases where e.fkProject == id select e))
			{
				db.Delete(item);
			}

			foreach (var item in (from e in db.ProjectUsers where e.fkProject == id select e))
			{
				db.Delete(item);
			}

			db.Delete(this);
		}
	}
}
