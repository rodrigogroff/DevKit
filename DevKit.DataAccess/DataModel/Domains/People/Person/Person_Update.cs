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
								resp = "Telefone já utilizado!";
								return false;
							}

							db.Insert(ent);
						}
						else
							db.Update(ent);

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
								resp = "Email já utilizado!";
								return false;
							}

							db.Insert(ent);
						}
						else
							db.Update(ent);

                        break;
					}

				case "removeEmail":
					{
						var ent = JsonConvert.DeserializeObject<PersonEmail>(anexedEntity.ToString());

						db.Delete(ent); 

						break;
					}

                case "newEnd":
                    {
                        var ent = JsonConvert.DeserializeObject<PersonAddress>(anexedEntity.ToString());

                        ent.fkUser = id;

                        if (ent.bPrincipal == null)
                            ent.bPrincipal = false;

                        if (!db.PersonAddress.Where(y=>y.fkPerson == id).Any())
                            ent.bPrincipal = true;

                        if (ent.id == 0)
                            db.Insert(ent);
                        else
                            db.Update(ent);

                        break;
                    }

                case "removeEnd":
                    {
                        var ent = JsonConvert.DeserializeObject<PersonAddress>(anexedEntity.ToString());

                        db.Delete(ent);

                        if (db.PersonAddress.Where(y => y.fkPerson == id).Count() == 1)
                        {
                            var pa = db.PersonAddress.Where(y => y.fkPerson == id).FirstOrDefault();

                            pa.bPrincipal = true;

                            db.Update(pa);
                        }

                        break;
                    }
            }

            dtLastUpdate = DateTime.Now;
            fkUserLastUpdate = user.id;

            db.Update(this);

            return true;
		}
	}
}
