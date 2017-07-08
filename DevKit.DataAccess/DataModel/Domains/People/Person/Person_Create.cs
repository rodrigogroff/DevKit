using LinqToDB;
using System;
using System.Linq;

namespace DataModel
{
	public partial class Person
	{
		bool CheckDuplicate(Person item, DevKitDB db)
		{
			var query = from e in db.Person select e;

			if (item.stName != null)
				query = from e in query where e.stName.ToUpper().Contains(item.stName) select e;
				
			if (item.id > 0)
				query = from e in query where e.id != item.id select e;

			return query.Any();
		}

		public bool Create(DevKitDB db, ref string resp)
		{
			var user = db.currentUser;

			if (CheckDuplicate(this, db))
			{
				resp = "Name already taken";
				return false;
			}

			if (stName == "")
			{
				resp = "Invalid Name";
				return false;
			}

            this.dtStart = DateTime.Now;
            this.fkUserAdd = user.id;

            id = Convert.ToInt64(db.InsertWithIdentity(this));
            		
			return true;
		}
	}
}
