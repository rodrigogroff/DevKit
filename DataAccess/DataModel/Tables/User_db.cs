using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System;

namespace DataModel
{
	public class UserLoadParams
	{
		public bool bTudo = false,
					bPerfil = false,
					bTelefones = false,
					bQtdeTelefones = false,
					bEmails = false,
					bQtdeEmails = false;
	}

	public class UserFilter
	{
		public int skip, take;
		public long? fkPerfil;
		public bool? ativo;
		public string busca;
	}

	public partial class User
	{
		UserLoadParams load = new UserLoadParams { bTudo = true };

		public int	qttyPhones = 0,
					qttyEmails = 0;

		public Profile Profile;
		public List<UserPhone> phones = new List<UserPhone>();
		public List<UserEmail> emails = new List<UserEmail>();

		// functions

		Profile LoadPerfil(DevKitDB db) { return (from e in db.Profiles where e.id == fkProfile select e).FirstOrDefault(); }

		List<UserPhone> LoadPhones(DevKitDB db) { return (from e in db.UserPhones where e.fkUser == id select e).OrderBy ( t=> t.stPhone).ToList(); }
		int CountPhones(DevKitDB db) { return (from e in db.UserPhones where e.fkUser == id select e).Count(); }
		
		List<UserEmail> LoadEmails(DevKitDB db) { return (from e in db.UserEmails where e.fkUser == id select e).OrderByDescending(t=>t.id).ToList(); }
		int CountEmails(DevKitDB db) { return (from e in db.UserEmails where e.fkUser == id select e).Count(); }

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

			if (load.bTudo || load.bPerfil)
				Profile = LoadPerfil(db);

			// telefones
			if (load.bTudo || load.bTelefones) phones = LoadPhones(db);
			if (load.bTudo) qttyPhones = phones.Count(); else if (load.bQtdeTelefones) qttyPhones = CountPhones(db);

			// emails
			if (load.bTudo || load.bEmails) emails = LoadEmails(db);
			if (load.bTudo) qttyEmails = emails.Count(); else if (load.bQtdeEmails) qttyEmails = CountEmails(db);

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

			this.dtCreation = DateTime.Now;

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

			if (!UpdateTelefones(db))
			{
				resp = "Duplicate phone";
				return false;
			}

			if (!UpdateEmails(db))
			{
				resp = "Duplicate email!";
				return false;
			}

			db.Update(this);

			return true;
		}

		public bool CanDelete(DevKitDB db, ref string resp)
		{
			return true;
		}

		public bool UpdateTelefones(DevKitDB db)
		{
			// search global mask
			var mask = "(99) 9999999";

			for (int x = 0; x < phones.Count; ++x)
			{
				var item = phones.ElementAt(x);

				bool foundMask = false;

				foreach (var i in item.stPhone)
					if (!Char.IsLetterOrDigit(i))
					{
						foundMask = true;
						break;
					}
					
				if (!foundMask)
				{
					var result = ""; var index = 0; var maxlen = item.stPhone.Length;

					foreach (var i in mask)
					{
						if (Char.IsLetterOrDigit(i))
						{
							if (index < maxlen)
								result += item.stPhone[index++];
						}
						else
							result += i;
					}

					item.stPhone = result;
				}
			}

			var originais = LoadPhones(db);
			var ids_orig = (from e in originais select e.id).ToList();
			var ids_new = (from e in phones select e.id).ToList();

			foreach (var itemOldId in ids_orig)
				if (!ids_new.Contains(itemOldId))
					db.Delete((from e in originais where e.id == itemOldId select e).FirstOrDefault());

			foreach (var itemNewId in ids_new)
				if (!ids_orig.Contains(itemNewId))
					db.Insert((from e in phones where e.id == itemNewId select e).FirstOrDefault());

			return true;
		}

		public bool UpdateEmails(DevKitDB db)
		{
			var originais = LoadEmails(db);
			var ids_orig = (from e in originais select e.id).ToList();
			var ids_new = (from e in emails select e.id).ToList();

			foreach (var itemOldId in ids_orig)
				if (!ids_new.Contains(itemOldId))
					db.Delete((from e in originais where e.id == itemOldId select e).FirstOrDefault());

			foreach (var itemNewId in ids_new)
				if (!ids_orig.Contains(itemNewId))
					db.Insert((from e in emails where e.id == itemNewId select e).FirstOrDefault());

			return true;
		}
	}
}
