using LinqToDB;
using System.Linq;
using Newtonsoft.Json;

namespace DataModel
{
	public partial class ProjectSprint
	{
		bool CheckDuplicate(ProjectSprint item, DevKitDB db)
		{
			var query = from e in db.ProjectSprints select e;

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
		
		public bool Update(DevKitDB db, User user, ref string resp)
		{
			if (CheckDuplicate(this, db))
			{
				resp = "Project name already taken";
				return false;
			}

			switch (updateCommand)
			{
				case "entity":
					{
						db.Update(this);

						new AuditLog {
							fkUser = user.id,
							fkActionLog = EnumAuditAction.ProjectUpdateUpdateSprint,
							nuType = EnumAuditType.Sprint,
							fkTarget = this.id
						}.
						Create(db, TrackChanges(db), "");

						break;
					}

				case "newVersion":
					{
						var ent = JsonConvert.DeserializeObject<ProjectSprintVersion>(anexedEntity.ToString());

						if (ent.id == 0)
						{
							if ((from ne in db.ProjectSprintVersions
								 where ne.stName == ent.stName && ne.fkSprint == id
								 select ne).
								 Any())
							{
								resp = "Version already added to project!";
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
							Create(db, "", "");
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
							Create(db, "", "");
						}
						
						versions = LoadVersions(db);
						break;
					}

				case "removeVersion":
					{
						var versionDel = JsonConvert.DeserializeObject<ProjectSprintVersion>(anexedEntity.ToString());

						if ((from e in db.Tasks where e.fkVersion == id select e).Any())
						{
							resp = "This version is being used in a task";
							return false;
						}

						db.Delete(versionDel);

						new AuditLog {
							fkUser = user.id,
							fkActionLog = EnumAuditAction.SprintRemoveVersion,
							nuType = EnumAuditType.Sprint,
							fkTarget = this.id
						}.
						Create(db, "", "");

						versions = LoadVersions(db);
						break;
					}
			}

			return true;
		}
	}
}
