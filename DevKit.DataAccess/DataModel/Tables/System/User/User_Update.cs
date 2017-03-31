using LinqToDB;
using Newtonsoft.Json;
using System.Linq;

namespace DataModel
{
	public partial class User
	{
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
						ent.fkUser = id;

						if (ent.id == 0)
						{
							if ((from ne in db.UserPhones where ne.stPhone == ent.stPhone select ne).Any())
							{
								resp = "Phone duplicated!";
								return false;
							}

							db.Insert(ent);
						}
						else
							db.Update(ent);

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

						ent.fkUser = id;

						if (ent.id == 0)
						{
							if ((from ne in db.UserEmails where ne.stEmail == ent.stEmail select ne).Any())
							{
								resp = "Email duplicated!";
								return false;
							}

							db.Insert(ent);
						}
						else
							db.Update(ent);

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
	}
}
