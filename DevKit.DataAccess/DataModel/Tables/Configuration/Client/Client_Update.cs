using LinqToDB;

namespace DataModel
{
	public partial class Client
	{
		public bool Update(DevKitDB db, long fkCurrentUser, ref string resp)
		{
			var user = db.GetCurrentUser(fkCurrentUser);

			if (CheckDuplicate(this, db))
			{
				resp = "Client name already taken";
				return false;
			}

			switch (updateCommand)
			{
				case "entity":
					{
						new AuditLog {
							fkUser = user.id,
							fkActionLog = EnumAuditAction.ClientEdit,
							nuType = EnumAuditType.Client,
							fkTarget = this.id
						}.
						Create(db, TrackChanges(db), "");

						db.Update(this);
						break;
					}			
			}

			return true;
		}
	}
}
