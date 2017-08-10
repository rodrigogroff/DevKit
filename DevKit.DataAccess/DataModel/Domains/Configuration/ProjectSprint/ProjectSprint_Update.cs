using LinqToDB;
using System.Linq;
using Newtonsoft.Json;

namespace DataModel
{
	public partial class ProjectSprint
	{
		bool CheckDuplicate(ProjectSprint item, DevKitDB db)
		{
			var query = from e in db.ProjectSprint select e;

			if (item.stName != null)
			{
				var _st = item.stName.ToUpper();
				query = from e in query where e.stName.ToUpper().Contains(_st) select e;
			}

			if (item.fkPhase > 0)
				query = from e in query where e.fkPhase == item.fkPhase select e;

			if (item.id > 0)
				query = from e in query where e.id != item.id select e;

			return query.Any();
		}
		
		public bool Update(DevKitDB db, ref string resp)
		{
			var user = db.currentUser;

			if (CheckDuplicate(this, db))
			{
				resp = "Nome de ´sprint já utilizado";
				return false;
			}

			switch (updateCommand)
			{
				case "entity":
					{
						new AuditLog {
							fkUser = user.id,
							fkActionLog = EnumAuditAction.ProjectUpdateUpdateSprint,
							nuType = EnumAuditType.Sprint,
							fkTarget = this.id
						}.
						Create(db, TrackChanges(db), "");

						db.Update(this);
						
						break;
					}

				case "newVersion":
					{
						var ent = JsonConvert.DeserializeObject<ProjectSprintVersion>(anexedEntity.ToString());

						if (ent.id == 0)
						{
							if ((from ne in db.ProjectSprintVersion
								 where ne.stName == ent.stName && ne.fkSprint == id
								 select ne).
								 Any())
							{
								resp = "Versão já adicionada ao projeto";
								return false;
							}

							ent.fkSprint = id;
							
							db.Insert(ent);

							new AuditLog {
								fkUser = user.id,
								fkActionLog = EnumAuditAction.SprintAddVersion,
								nuType = EnumAuditType.Sprint,
								fkTarget = this.id
							}.
							Create(db, "Nova versão: " + ent.stName, "");
						}
						else
						{
							db.Update(ent);

							new AuditLog {
								fkUser = user.id,
								fkActionLog = EnumAuditAction.SprintUpdateVersion,
								nuType = EnumAuditType.Sprint,
								fkTarget = this.id
							}.
							Create(db, "Versão atualizada: " + ent.stName, "");
						}
						
						break;
					}

				case "removeVersion":
					{
						var versionDel = JsonConvert.DeserializeObject<ProjectSprintVersion>(anexedEntity.ToString());

						if ((from e in db.Task where e.fkVersion == id select e).Any())
						{
							resp = "Esta versão é usada em uma tarefa";
							return false;
						}

						db.Delete(versionDel);

						new AuditLog {
							fkUser = user.id,
							fkActionLog = EnumAuditAction.SprintRemoveVersion,
							nuType = EnumAuditType.Sprint,
							fkTarget = this.id
						}.
						Create(db, "Versão removida " + versionDel.stName, "");

						break;
					}
			}

			return true;
		}
	}
}
