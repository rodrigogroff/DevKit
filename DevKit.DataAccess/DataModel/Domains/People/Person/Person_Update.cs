using LinqToDB;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace DataModel
{
	public partial class Person
	{
		public bool Update(DevKitDB db, ref string resp)
		{
			var user = db.currentUser;

			if (CheckDuplicate(this, db))
			{
				resp = "Name already taken";
				return false;
			}
            
			switch (updateCommand)
			{
				case "entity":
					{
						
						break;
					}
									
				case "newPhone":
					{
						var ent = JsonConvert.DeserializeObject<PersonPhone>(anexedEntity.ToString());

						ent.stPhone = GetMaskedValue(db, ent.stPhone);
						ent.fkUser = id;

						if (ent.id == 0)
						{
							if ((from ne in db.PersonPhone where ne.stPhone == ent.stPhone select ne).Any())
							{
								resp = "Phone duplicated!";
								return false;
							}

							db.Insert(ent);
						}
						else
						{
							db.Update(ent);
						}							
						break;
					}

				case "removePhone":
					{
						var ent = JsonConvert.DeserializeObject<PersonPhone>(anexedEntity.ToString());

						db.Delete(ent);
						break;
					}

				case "newEmail":
					{
						var ent = JsonConvert.DeserializeObject<PersonEmail>(anexedEntity.ToString());

						ent.fkUser = id;

						if (ent.id == 0)
						{
							if ((from ne in db.PersonEmail where ne.stEmail == ent.stEmail select ne).Any())
							{
								resp = "Email duplicated!";
								return false;
							}

							db.Insert(ent);
						}
						else
						{
							db.Update(ent);
						}
						break;
					}

				case "removeEmail":
					{
						var ent = JsonConvert.DeserializeObject<PersonEmail>(anexedEntity.ToString());

						db.Delete(ent); 
						break;
					}
			}

            this.dtLastUpdate = DateTime.Now;
            this.fkUserLastUpdate = user.id;

            db.Update(this);

            return true;
		}
	}
}
