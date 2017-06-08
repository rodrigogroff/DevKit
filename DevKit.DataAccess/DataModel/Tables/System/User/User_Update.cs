using LinqToDB;
using Newtonsoft.Json;
using System.Linq;

namespace DataModel
{
	public partial class User
	{
		public bool Update(DevKitDB db, long fkCurrentUser, ref string resp)
		{
			var user = db.GetCurrentUser(fkCurrentUser);

			if (CheckDuplicate(this, db))
			{
				resp = "Login already taken";
				return false;
			}

			switch (updateCommand)
			{
				case "changePassword":
					{
						var ent = JsonConvert.DeserializeObject<UserPasswordChange>(anexedEntity.ToString());

						if (ent.stCurrentPassword.ToUpper() != this.stPassword.ToUpper() )
						{
							resp = "Current password does not match!";
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
						Create(db, "Password changed by user", "");

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
						Create(db, "A new password was generated [" + this.stPassword + "]", "");

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
						Create(db, TrackChanges(db), "");

						db.Update(this);

						logs = LoadLogs(db);
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
								resp = "Phone duplicated!";
								return false;
							}

							db.Insert(ent);

							new AuditLog {
								fkUser = user.id,
								fkActionLog = EnumAuditAction.UserAddPhone,
								nuType = EnumAuditType.User,
								fkTarget = this.id
							}.
							Create(db, "New phone: " + ent.stPhone, "");
						}
						else
						{
							new AuditLog {
								fkUser = user.id,
								fkActionLog = EnumAuditAction.UserEditPhone,
								nuType = EnumAuditType.User,
								fkTarget = this.id
							}.
							Create(db, "Edit phone: " + ent.stPhone, "");

							db.Update(ent);
						}							

						phones = LoadPhones(db);
						logs = LoadLogs(db);
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
						Create(db, "Phone removed: " + ent.stPhone, "");

						phones = LoadPhones(db);
						logs = LoadLogs(db);
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
								resp = "Email duplicated!";
								return false;
							}

							db.Insert(ent);

							new AuditLog {
								fkUser = user.id,
								fkActionLog = EnumAuditAction.UserAddEmail,
								nuType = EnumAuditType.User,
								fkTarget = this.id
							}.
							Create(db, "Email created: " + ent.stEmail, "");
						}
						else
						{
							new AuditLog {
								fkUser = user.id,
								fkActionLog = EnumAuditAction.UserEditEmail,
								nuType = EnumAuditType.User,
								fkTarget = this.id
							}.
							Create(db, "Email edited: " + ent.stEmail, "");

							db.Update(ent);
						}
							
						emails = LoadEmails(db);
						logs = LoadLogs(db);
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
						Create(db, "Email removed: " + ent.stEmail, "");

						emails = LoadEmails(db);
						logs = LoadLogs(db);
						break;
					}
			}

			return true;
		}
	}
}
