using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

namespace DataModel
{
	public class UserFilter
	{
		public int skip, take;
		public string busca;

		public bool? ativo;
		public long? fkPerfil;
	}

	// --------------------------
	// properties
	// --------------------------

	public partial class User
	{ 
		public string sdtLastLogin = "";
		public string sdtCreation = "";

		public Profile profile;

		public List<UserPhone> phones;
		public List<UserEmail> emails;
		
		public string updateCommand = "";
		public object anexedEntity;
	}

	// --------------------------
	// functions
	// --------------------------

	public partial class User
	{
		public IQueryable<User> ComposedFilters(DevKitDB db, UserFilter filter)
		{
			var query = from e in db.Users select e;

			if (filter.ativo != null)
				query = from e in query where e.bActive == filter.ativo select e;

			if (filter.busca != null)
				query = from e in query where e.stLogin.ToUpper().Contains(filter.busca) select e;

			if (filter.fkPerfil != null)
				query = from e in query where e.fkProfile == filter.fkPerfil select e;

			return query;
		}

		public User LoadAssociations(DevKitDB db)
		{
			var setup = db.Setup();

			sdtLastLogin = dtLastLogin?.ToString(setup.stDateFormat);
			sdtCreation = dtCreation?.ToString(setup.stDateFormat);

			profile = db.Profile(fkProfile);

			phones = LoadPhones(db);
			emails = LoadEmails(db);

			return this;
		}

		Profile LoadProfile(DevKitDB db)
		{
			return (from e in db.Profiles where e.id == fkProfile select e).
				FirstOrDefault();
		}

		List<UserPhone> LoadPhones(DevKitDB db)
		{
			return (from e in db.UserPhones where e.fkUser == id select e).
				OrderBy(t => t.stPhone).
				ToList();
		}

		List<UserEmail> LoadEmails(DevKitDB db)
		{
			return (from e in db.UserEmails where e.fkUser == id select e).
				OrderByDescending(t => t.id).
				ToList();
		}

		bool CheckDuplicate(User item, DevKitDB db)
		{
			var query = from e in db.Users select e;

			if (item.bActive != null)
				query = from e in query where e.bActive == item.bActive select e;

			if (item.stLogin != null)
			{
				var _stLogin = item.stLogin.ToUpper();
				query = from e in query where e.stLogin.ToUpper().Contains(_stLogin) select e;
			}
				
			if (item.id > 0)
				query = from e in query where e.id != item.id select e;

			return query.Any();
		}

		public bool Create(DevKitDB db, ref string resp)
		{
			if (CheckDuplicate(this, db))
			{
				resp = "Login already taken";
				return false;
			}

			dtCreation = DateTime.Now;

			id = Convert.ToInt64(db.InsertWithIdentity(this));

			return true;
		}

		public bool Update(DevKitDB db, ref string resp)
		{
			if (CheckDuplicate(this, db))
			{
				resp = "Login already taken";
				return false;
			}

			switch (updateCommand)
			{
				case "entity":
					{
						db.Update(this);
						break;
					}
									
				case "newPhone":
					{
						var ent = JsonConvert.DeserializeObject<UserPhone>(anexedEntity.ToString());
						ent.stPhone = GetMaskedValue(db, ent.stPhone);

						if ((from ne in db.UserPhones where ne.stPhone == ent.stPhone select ne).Any())
						{
							resp = "Phone duplicated!";
							return false;
						}

						ent.fkUser = id;

						db.Insert(ent);

						phones = LoadPhones(db);
						break;
					}

				case "removePhone":
					{
						db.Delete(JsonConvert.DeserializeObject<UserPhone>(anexedEntity.ToString()));

						phones = LoadPhones(db);
						break;
					}

				case "newEmail":
					{
						var ent = JsonConvert.DeserializeObject<UserEmail>(anexedEntity.ToString());

						if ((from ne in db.UserEmails where ne.stEmail == ent.stEmail select ne).Any())
						{
							resp = "Email duplicated!";
							return false;
						}

						ent.fkUser = id;

						db.Insert(ent);

						emails = LoadEmails(db);
						break;
					}

				case "removeEmail":
					{
						db.Delete(JsonConvert.DeserializeObject<UserEmail>(anexedEntity.ToString()));

						emails = LoadEmails(db);
						break;
					}
			}

			return true;
		}

		public bool CanDelete(DevKitDB db, ref string resp)
		{
			if (stLogin.ToUpper() == "DBA")
			{
				resp = "DBA user cannot be removed";
				return false;
			}

			if ( (from e in db.ProjectUsers where e.fkUser == id select e).Count() > 0)
			{
				resp = "this user is allocated in a project and cannot be removed";
				return false;
			}

			if ((from e in db.Tasks where e.fkUserStart == id select e).Count() > 0)
			{
				resp = "this user is allocated in a task and cannot be removed";
				return false;
			}

			return true;
		}

		public void Delete(DevKitDB db)
		{
			foreach (var item in (from e in db.UserPhones where e.fkUser == id select e))
				db.Delete(item);

			foreach (var item in (from e in db.UserEmails where e.fkUser == id select e))
				db.Delete(item);

			db.Delete(this);			
		}

		public string GetMaskedValue(DevKitDB db, string stPhone)
		{
			var pref = (from e in db.Setups select e).FirstOrDefault();
			var mask = "(99) 9999999"; // default

			if (pref != null)
				mask = pref.stPhoneMask;

			bool foundMask = false;

			foreach (var i in stPhone)
				if (!Char.IsLetterOrDigit(i))
				{
					foundMask = true;
					break;
				}

			if (!foundMask)
			{
				var result = ""; var index = 0; var maxlen = stPhone.Length;

				foreach (var i in mask)
				{
					if (Char.IsLetterOrDigit(i))
					{
						if (index < maxlen)
							result += stPhone[index++];
					}
					else
						result += i;
				}

				return result;
			}
			else
				return stPhone;			
		}
	}
}
