using LinqToDB;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace DataModel
{
	public partial class LoteGrafica
    {
		public bool Update(DevKitDB db, ref string resp)
		{
			/*
			switch (updateCommand)
			{
				case "newUser":
					{
						var ent = JsonConvert.DeserializeObject<ProjectUser>(anexedEntity.ToString());

						if (ent.id == 0)
						{
							if ((from ne in db.ProjectUser
								 where ne.fkUser == ent.fkUser && ne.fkProject == id
								 select ne).Any())
							{
								resp = "User já acrescentado no projeto";
								return false;
							}
						
							ent.fkProject = id;
							ent.dtJoin = DateTime.Now;
                            
                            db.Insert(ent);

							new AuditLog {
								fkUser = user.id,
								fkActionLog = EnumAuditAction.ProjectUpdateAddUser,
								nuType = EnumAuditType.Project,
								fkTarget = this.id
							}.
							Create(db, "Novo usuario: " + db.GetUser(ent.fkUser).stLogin + ";Papel: " + ent.stRole, "");
						}							
						else
						{
							db.Update(ent);

							new AuditLog {
								fkUser = user.id,
								fkActionLog = EnumAuditAction.ProjectUpdateUpdateUser,
								nuType = EnumAuditType.Project,
								fkTarget = this.id
							}.
							Create(db, "Papel atualizado: " + ent.stRole, "");
						}							
						break;
					}

			}
            */

			return true;
		}
	}
}
