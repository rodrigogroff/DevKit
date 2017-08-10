using LinqToDB;
using Newtonsoft.Json;
using System.Linq;

namespace DataModel
{
	public partial class ClientGroup
	{
		public bool Update(DevKitDB db, ref string resp)
		{
			var user = db.currentUser;

			if (CheckDuplicate(this, db))
			{
				resp = "Nome do grupo já usado";
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

                        if (!(from e in db.ClientGroupAssociation
                             where e.fkClientGroup == this.id
                             where e.fkClient == ent.id
                             select e).Any())
                        {
                            ent.fkClientGroup = this.id;
                            db.Insert(ent);                            
                        }
												
						break;
					}

				case "removeClient":
					{
						var ent = JsonConvert.DeserializeObject<ClientGroupAssociation>(anexedEntity.ToString());
						
						db.Delete(ent);

						break;
					}
			}

			return true;
		}
	}
}
