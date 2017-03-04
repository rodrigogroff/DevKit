using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

namespace DataModel
{
	public class UserLoadParams
	{
		public bool bAll = false,
					bProfile = false,
					bPhones = false,
					bQttyPhones = false,
					bEmails = false,
					bQttyEmails = false;
	}

	public class UserFilter
	{
		public int skip, take;
		public long? fkPerfil;
		public bool? ativo;
		public string busca;
	}

	// --------------------------
	// properties
	// --------------------------

	public partial class User
	{
		public int qttyPhones = 0,
					qttyEmails = 0;

		public Profile Profile;
		public List<UserPhone> phones = new List<UserPhone>();
		public List<UserEmail> emails = new List<UserEmail>();

		public string updateCommand = "";
		public object anexedEntity;
	}

	// --------------------------
	// functions
	// --------------------------

	public partial class User
	{
		UserLoadParams load = new UserLoadParams { bAll = true };

		Profile LoadPerfil(DevKitDB db)
		{
			return (from e in db.Profiles where e.id == fkProfile select e).
				FirstOrDefault();
		}

		List<UserPhone> LoadPhones(DevKitDB db)
		{
			return (from e in db.UserPhones where e.fkUser == id select e).
				OrderBy ( t=> t.stPhone).
				ToList();
		}

		int CountPhones(DevKitDB db)
		{
			return (from e in db.UserPhones where e.fkUser == id select e).Count();
		}
		
		List<UserEmail> LoadEmails(DevKitDB db)
		{
			return (from e in db.UserEmails where e.fkUser == id select e).
				OrderByDescending(t=>t.id).
				ToList();
		}

		int CountEmails(DevKitDB db)
		{
			return (from e in db.UserEmails where e.fkUser == id select e).Count();
		}

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

		public User Load(DevKitDB db, UserLoadParams _load = null)
		{
			if (_load != null)
				load = _load;

			if (load.bAll || load.bProfile)
				Profile = LoadPerfil(db);

			// phones
			if (load.bAll || load.bPhones) phones = LoadPhones(db);
			if (load.bAll) qttyPhones = phones.Count(); else if (load.bQttyPhones) qttyPhones = CountPhones(db);

			// emails
			if (load.bAll || load.bEmails) emails = LoadEmails(db);
			if (load.bAll) qttyEmails = emails.Count(); else if (load.bQttyEmails) qttyEmails = CountEmails(db);

			return this;
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
						ent.stPhone = GetMaskedValue(ent.stPhone);
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

			return true;
		}

		public string GetMaskedValue(string stPhone, string mask = "(99) 9999999")
		{
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
