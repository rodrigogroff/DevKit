using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System;

namespace DataModel
{
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
		public int qttyPermissions = 0;

		public List<User> Users { get; set; }
	}

	// --------------------------
	// functions
	// --------------------------

	public partial class Profile
	{
		public IQueryable<Profile> ComposedFilters(DevKitDB db, ProfileFilter filter)
		{
			var query = from e in db.Profiles select e;

			if (filter.busca != null)
				query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			return query;
		}

		public Profile Load(DevKitDB db)
		{
			if ( stPermissions != null && stPermissions.Length > 0)
				qttyPermissions = stPermissions.Split('|').Length / 2;
			
			Users = LoadUsers(db);
						
			return this;
		}

		List<User> LoadUsers(DevKitDB db)
		{
			return (from e in db.Users where e.fkProfile == id select e).
				ToList();
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
			
			if ((from e in db.Users where e.fkProfile == this.id select e).Any())
			{
				resp = "Profile still associated with other users";
				return false;
			}

			return true;
		}
	}
}
