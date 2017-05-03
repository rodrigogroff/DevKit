using LinqToDB;
using Newtonsoft.Json;

namespace DataModel
{
	public partial class ClientGroup
	{
		public bool Update(DevKitDB db, ref string resp)
		{
			var user = db.GetCurrentUser();

			if (CheckDuplicate(this, db))
			{
				resp = "Client group name already taken";
				return false;
			}

			switch (updateCommand)
			{
				case "entity":
					{
						new AuditLog
						{
							fkUser = user.id,
							fkActionLog = EnumAuditAction.ClientGroupEdit,
							nuType = EnumAuditType.ClientGroup,
							fkTarget = this.id
						}.
						Create(db, TrackChanges(db), "");

						db.Update(this);
						break;
					}

				case "newClient":
					{
						var ent = JsonConvert.DeserializeObject<ClientGroupAssociation>(anexedEntity.ToString());
						
						ent.fkClientGroup = this.id;

						db.Insert(ent);						

						clients = LoadClients(db);
						break;
					}

				case "removeClient":
					{
						var ent = JsonConvert.DeserializeObject<ClientGroupAssociation>(anexedEntity.ToString());
						
						db.Delete(ent);

						clients = LoadClients(db);
						break;
					}
			}

			return true;
		}
	}
}
