/*
using LinqToDB;

namespace DataModel
{
	public partial class Client
	{
		public bool Update(DevKitDB db, ref string resp)
		{
			var user = db.currentUser;

			if (CheckDuplicate(this, db))
			{
				resp = "Nome do cliente já em uso";
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
*/
