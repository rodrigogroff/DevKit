using LinqToDB;
using System;
using System.Linq;

namespace DataModel
{
	public partial class Survey
	{
		bool CheckDuplicate(Survey item, DevKitDB db)
		{
			var query = from e in db.CompanyNews select e;

			if (item.stTitle != null)
			{
				var _st = item.stTitle.ToUpper();
				query = from e in query where e.stTitle.ToUpper().Contains(_st) select e;
			}

			if (item.id > 0)
				query = from e in query where e.id != item.id select e;

			return query.Any();
		}
		
		public bool Create(DevKitDB db, ref string resp)
		{
			var user = db.currentUser;

			if (CheckDuplicate(this, db))
			{
				resp = "Título de pesquisa já utilizado previamente";
				return false;
			}

			fkUser = user.id;
			dtLog = DateTime.Now;

			id = Convert.ToInt64(db.InsertWithIdentity(this));

			return true;
		}
	}
}
