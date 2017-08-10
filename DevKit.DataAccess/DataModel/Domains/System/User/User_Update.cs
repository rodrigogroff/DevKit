using LinqToDB;
using Newtonsoft.Json;
using System.Linq;

namespace DataModel
{
	public partial class User
	{
		public bool Update(DevKitDB db, ref string resp, ref bool bProfileChanged)
		{
			var user = db.currentUser;

			if (CheckDuplicate(this, db))
			{
				resp = "Login já utilizado";
				return false;
			}

			switch (updateCommand)
			{
				case "changePassword":
					{
						var ent = JsonConvert.DeserializeObject<UserPasswordChange>(anexedEntity.ToString());

						if (ent.stCurrentPassword.ToUpper() != this.stPassword.ToUpper() )
						{
							resp = "Password atual não confere!";
							return false;
						}

						this.stPassword = ent.stNewPassword;

						db.Update(this);

						new AuditLog
						{
							fkUser = user.id,
							fkActionLog = EnumAuditAction.UserPasswordReset,
							nuType = EnumAuditType.User,
							fkTarget = this.id
						}.
						Create(db, "Password atualizada", "");

						break;
					}

				case "resetPassword":
					{
						this.stPassword = GetRandomString(8);
						
						db.Update(this);

						resetPassword = this.stPassword;

						new AuditLog
						{
							fkUser = user.id,
							fkActionLog = EnumAuditAction.UserPasswordReset,
							nuType = EnumAuditType.User,
							fkTarget = this.id
						}.
						Create(db, "Nova senha gerada [" + this.stPassword + "]", "");

						break;
					}

				case "entity":
					{
						new AuditLog {
							fkUser = user.id,
							fkActionLog = EnumAuditAction.SystemUserUpdate,
							nuType = EnumAuditType.User,
							fkTarget = this.id
						}.
						Create(db, TrackChanges(db, ref bProfileChanged), "");

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
							if ((from ne in db.UserPhone where ne.stPhone == ent.stPhone select ne).Any())
							{
								resp = "Telefone duplicado!";
								return false;
							}

							db.Insert(ent);

							new AuditLog {
								fkUser = user.id,
								fkActionLog = EnumAuditAction.UserAddPhone,
								nuType = EnumAuditType.User,
								fkTarget = this.id
							}.
							Create(db, "Novo telefone: " + ent.stPhone, "");
						}
						else
						{
							new AuditLog {
								fkUser = user.id,
								fkActionLog = EnumAuditAction.UserEditPhone,
								nuType = EnumAuditType.User,
								fkTarget = this.id
							}.
							Create(db, "Telefone editado: " + ent.stPhone, "");

							db.Update(ent);
						}							
						break;
					}

				case "removePhone":
					{
						var ent = JsonConvert.DeserializeObject<UserPhone>(anexedEntity.ToString());

						db.Delete(ent);

						new AuditLog {
							fkUser = user.id,
							fkActionLog = EnumAuditAction.UserRemovePhone,
							nuType = EnumAuditType.User,
							fkTarget = this.id
						}.
						Create(db, "Telefone removido: " + ent.stPhone, "");

						break;
					}

				case "newEmail":
					{
						var ent = JsonConvert.DeserializeObject<UserEmail>(anexedEntity.ToString());

						ent.fkUser = id;

						if (ent.id == 0)
						{
							if ((from ne in db.UserEmail where ne.stEmail == ent.stEmail select ne).Any())
							{
								resp = "Email duplicado!";
								return false;
							}

							db.Insert(ent);

							new AuditLog {
								fkUser = user.id,
								fkActionLog = EnumAuditAction.UserAddEmail,
								nuType = EnumAuditType.User,
								fkTarget = this.id
							}.
							Create(db, "Email criado: " + ent.stEmail, "");
						}
						else
						{
							new AuditLog {
								fkUser = user.id,
								fkActionLog = EnumAuditAction.UserEditEmail,
								nuType = EnumAuditType.User,
								fkTarget = this.id
							}.
							Create(db, "Email editado: " + ent.stEmail, "");

							db.Update(ent);
						}
						break;
					}

				case "removeEmail":
					{
						var ent = JsonConvert.DeserializeObject<UserEmail>(anexedEntity.ToString());

						db.Delete(ent); 

						new AuditLog {
							fkUser = user.id,
							fkActionLog = EnumAuditAction.UserRemoveEmail,
							nuType = EnumAuditType.User,
							fkTarget = this.id
						}.
						Create(db, "Email removido: " + ent.stEmail, "");
						break;
					}
			}

			return true;
		}
	}
}
