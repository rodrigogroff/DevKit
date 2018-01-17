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
                        new AuditLog
                        {
                            fkUser = user.id,
                            fkActionLog = EnumAuditAction.PersonUpdate,
                            nuType = EnumAuditType.Person,
                            fkTarget = this.id
                        }.
                        Create(db, "", "");

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

                            new AuditLog
                            {
                                fkUser = user.id,
                                fkActionLog = EnumAuditAction.PersonAddPhone,
                                nuType = EnumAuditType.Person,
                                fkTarget = this.id
                            }.
                            Create(db, ent.stPhone, "");

                        }
						else
                        {
                            var oldPhone = db.PersonPhone.
                                            Where(y => y.id == ent.id).
                                            FirstOrDefault();
                                                        
                            new AuditLog
                            {
                                fkUser = user.id,
                                fkActionLog = EnumAuditAction.PersonEditPhone,
                                nuType = EnumAuditType.Person,
                                fkTarget = this.id
                            }.
                            Create(db, oldPhone.stPhone + " => " + ent.stPhone, "");

                            db.Update(ent);
                        }							

						break;
					}

				case "removePhone":
					{
						var ent = JsonConvert.DeserializeObject<PersonPhone>(anexedEntity.ToString());

                        new AuditLog
                        {
                            fkUser = user.id,
                            fkActionLog = EnumAuditAction.PersonRemovePhone,
                            nuType = EnumAuditType.Person,
                            fkTarget = this.id
                        }.
                        Create(db, ent.stPhone, "");

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

                            new AuditLog
                            {
                                fkUser = user.id,
                                fkActionLog = EnumAuditAction.PersonAddEmail,
                                nuType = EnumAuditType.Person,
                                fkTarget = this.id
                            }.
                            Create(db, ent.stEmail, "");

                        }
                        else
                        {
                            var oldEmail = db.PersonEmail.
                                            Where(y => y.id == ent.id).
                                            FirstOrDefault();

                            if (oldEmail != null)
                            new AuditLog
                            {
                                fkUser = user.id,
                                fkActionLog = EnumAuditAction.PersonAddEmail,
                                nuType = EnumAuditType.Person,
                                fkTarget = this.id
                            }.
                            Create(db, oldEmail.stEmail + " => " + ent.stEmail, "");

                            db.Update(ent);
                        }							

                        break;
					}

				case "removeEmail":
					{
						var ent = JsonConvert.DeserializeObject<PersonEmail>(anexedEntity.ToString());

                        new AuditLog
                        {
                            fkUser = user.id,
                            fkActionLog = EnumAuditAction.PersonRemoveEmail,
                            nuType = EnumAuditType.Person,
                            fkTarget = this.id
                        }.
                        Create(db, ent.stEmail, "");

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
                        {
                            new AuditLog
                            {
                                fkUser = user.id,
                                fkActionLog = EnumAuditAction.PersonAddAddress,
                                nuType = EnumAuditType.Person,
                                fkTarget = this.id
                            }.
                            Create(db, ent.stRua + " "+ ent.stNumero + " " + ent.stReferencia, "");

                            db.Insert(ent);
                        }                            
                        else
                        {
                            var oldEnd = db.PersonAddress.
                                            Where(y => y.id == ent.id).
                                            FirstOrDefault();

                            if (oldEnd != null)
                            new AuditLog
                            {
                                fkUser = user.id,
                                fkActionLog = EnumAuditAction.PersonEditAddress,
                                nuType = EnumAuditType.Person,
                                fkTarget = this.id
                            }.
                            Create(db, oldEnd.stRua + " " + oldEnd.stNumero + " " + oldEnd.stReferencia + " => " +
                                       ent.stRua + " " + ent.stNumero + " " + ent.stReferencia, "");

                            db.Update(ent);
                        }                            

                        break;
                    }

                case "removeEnd":
                    {
                        var ent = JsonConvert.DeserializeObject<PersonAddress>(anexedEntity.ToString());

                        new AuditLog
                        {
                            fkUser = user.id,
                            fkActionLog = EnumAuditAction.PersonRemoveAddress,
                            nuType = EnumAuditType.Person,
                            fkTarget = this.id
                        }.
                        Create(db, ent.stRua + " " + ent.stNumero + " " + ent.stReferencia, "");

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

            if (stVencCartao.StartsWith("("))
                stVencCartao = "";
            
            dtLastUpdate = DateTime.Now;
            fkUserLastUpdate = user.id;

            db.Update(this);

            return true;
		}
	}
}
