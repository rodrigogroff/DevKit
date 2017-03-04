using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System;

namespace DataModel
{
	public class ProfileLoad_Params
	{
		public bool bAll = false,
					bUsers = false,
					bQttyUsers = false;
	}

	public class ProfileFilter
	{
		public int skip, take;		
		public bool? ativo;
		public string busca;
	}

	// --------------------------
	// properties
	// --------------------------

	public partial class Profile
	{
		public int qttyUsers = 0,
					qttyPermissions = 0;

		public List<User> Users { get; set; }
	}

	// --------------------------
	// functions
	// --------------------------

	public partial class Profile
	{
		ProfileLoad_Params load = new ProfileLoad_Params { bAll = true };

		List<User> LoadUsers(DevKitDB db)
		{
			return (from e in db.Users where e.fkProfile == id select e).
				ToList();
		}

		int CountUsers(DevKitDB db)
		{
			return (from e in db.Users where e.fkProfile == id select e).Count();
		}

		public IQueryable<Profile> ComposedFilters(DevKitDB db, ProfileFilter filter)
		{
			var query = from e in db.Profiles select e;

			if (filter.busca != null)
				query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			return query;
		}

		public Profile Load(DevKitDB db, ProfileLoad_Params _load = null)
		{
			if (_load != null)
				load = _load;

			if ( stPermissions != null && stPermissions.Length > 0)
				qttyPermissions = stPermissions.Split('|').Length / 2;
			
			if (load.bAll || load.bUsers) Users = LoadUsers(db);
			if (load.bAll) qttyUsers = Users.Count(); else if (load.bQttyUsers) qttyUsers = CountUsers(db);
			
			return this;
		}

		bool CheckDuplicate(Profile item, DevKitDB db)
		{
			var query = from e in db.Profiles select e;

			if (item.stName != null)
			{
				var _stName = item.stName.ToUpper();
				query = from e in query where e.stName.ToUpper().Contains(_stName) select e;
			}

			if (item.id > 0)
				query = from e in query where e.id != item.id select e;

			return query.Any();
		}

		public bool Create(DevKitDB db, ref string resp)
		{
			if (CheckDuplicate(this, db))
			{
				resp = "The name '" + stName + "' is already taken";
				return false;
			}

			id = Convert.ToInt64(db.InsertWithIdentity(this));

			return true;
		}

		public bool Update(DevKitDB db, ref string resp)
		{
			if (CheckDuplicate(this, db))
			{
				resp = "The name '" + stName + "' is already taken";
				return false;
			}

			db.Update(this);

			return true;
		}

		public bool CanDelete(DevKitDB db, ref string resp)
		{
			if (stName.ToUpper() == "ADMINISTRATOR")
			{
				resp = "Administrator profile cannot be deleted";
				return false;
			}

			resp = "Profile still associated with other users";

			var query = from e in db.Users where e.fkProfile == this.id select e;

			return query.Any();
		}
	}
}
