using LinqToDB;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace DataModel
{
	public partial class Project
	{
		public bool Update(DevKitDB db, ref string resp)
		{
			var user = db.currentUser;

			if (CheckDuplicate(this, db))
			{
				resp = "Project name already taken";
				return false;
			}

			switch (updateCommand)
			{
				case "entity":
					{
						new AuditLog {
							fkUser = user.id,
							fkActionLog = EnumAuditAction.ProjectUpdate,
							nuType = EnumAuditType.Project,
							fkTarget = this.id
						}.
						Create(db, TrackChanges(db), "");

						db.Update(this);

						logs = LoadLogs(db);

						break;
					}

				case "newUser":
					{
						var ent = JsonConvert.DeserializeObject<ProjectUser>(anexedEntity.ToString());

						if (ent.id == 0)
						{
							if ((from ne in db.ProjectUser
								 where ne.fkUser == ent.fkUser && ne.fkProject == id
								 select ne).Any())
							{
								resp = "User already added to project!";
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
							Create(db, "New user: " + db.GetUser(ent.fkUser).stLogin + ";Role: " + ent.stRole, "");
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
							Create(db, "Updated role: " + ent.stRole, "");
						}							

						users = LoadUsers(db);
						logs = LoadLogs(db);
						break;
					}

				case "removeUser":
					{
						var ent = JsonConvert.DeserializeObject<ProjectUser>(anexedEntity.ToString());

						db.Delete(ent);

						new AuditLog {
							fkUser = user.id,
							fkActionLog = EnumAuditAction.ProjectUpdateRemoveUser,
							nuType = EnumAuditType.Project,
							fkTarget = this.id
						}.
						Create(db, "User removed: " + db.GetUser(ent.fkUser), "");

						users = LoadUsers(db);
						logs = LoadLogs(db);
						break;
					}

				case "newPhase":
					{
						var ent = JsonConvert.DeserializeObject<ProjectPhase>(anexedEntity.ToString());

						if (ent.id == 0)
						{ 
							if ((from ne in db.ProjectPhase
								 where ne.stName.ToUpper() == ent.stName.ToUpper() && ne.fkProject == id
								 select ne).Any())
							{
								resp = "Phase already added to project!";
								return false;
							}
							
							ent.fkProject = id;

							db.Insert(ent);

							new AuditLog {
								fkUser = user.id,
								fkActionLog = EnumAuditAction.ProjectUpdateAddPhase,
								nuType = EnumAuditType.Project,
								fkTarget = this.id
							}.
							Create(db, "Phase added: " + ent.stName, "");
						}							
						else
						{
							db.Update(ent);

							new AuditLog {
								fkUser = user.id,
								fkActionLog = EnumAuditAction.ProjectUpdateUpdatePhase,
								nuType = EnumAuditType.Project,
								fkTarget = this.id
							}.
							Create(db, "Phase edited: " + ent.stName, "");
						}
							
						phases = LoadPhases(db);
						logs = LoadLogs(db);
						break;
					}

				case "removePhase":
					{
						var ent = JsonConvert.DeserializeObject<ProjectPhase>(anexedEntity.ToString());

						if ((from e in db.Task where e.fkPhase == ent.id select e).Any())
						{
							resp = "This phase is being used in a task";
							return false;
						}

						db.Delete(ent);

						new AuditLog {
							fkUser = user.id,
							fkActionLog = EnumAuditAction.ProjectUpdateRemovePhase,
							nuType = EnumAuditType.Project,
							fkTarget = this.id
						}.
						Create(db, "Phase deleted: " + ent.stName, "");

						phases = LoadPhases(db);
						logs = LoadLogs(db);
						break;
					}

				case "newSprint":
					{
						var ent = JsonConvert.DeserializeObject<ProjectSprint>(anexedEntity.ToString());

						if (ent.id == 0)
						{
							if ((from ne in db.ProjectSprint
								 where ne.fkPhase == id
								 where ne.stName.ToUpper() == ent.stName.ToUpper() && ne.fkProject == id
								 select ne).Any())
							{
								resp = "Sprint already added to project!";
								return false;
							}

							ent.fkProject = id;

							db.Insert(ent);

							new AuditLog {
								fkUser = user.id,
								fkActionLog = EnumAuditAction.ProjectUpdateAddSprint,
								nuType = EnumAuditType.Project,
								fkTarget = this.id
							}.
							Create(db, "New sprint: " + ent.stName, "");

							new AuditLog {
								fkUser = user.id,
								fkActionLog = EnumAuditAction.ProjectUpdateUpdateSprint,
								nuType = EnumAuditType.Sprint,
								fkTarget = ent.id
							}.
							Create(db, "", "");
						}
						else
						{
							db.Update(ent);

							new AuditLog {
								fkUser = user.id,
								fkActionLog = EnumAuditAction.ProjectUpdateUpdateSprint,
								nuType = EnumAuditType.Project,
								fkTarget = this.id
							}.
							Create(db, "", "");
						}							

						sprints = LoadSprints(db);
						logs = LoadLogs(db);
						break;
					}

				case "removeSprint":
					{
						var ent = JsonConvert.DeserializeObject<ProjectSprint>(anexedEntity.ToString());

						if ((from e in db.Task where e.fkSprint == ent.id select e).Any())
						{
							resp = "This sprint is being used in a task";
							return false;
						}

						db.Delete(ent);

						new AuditLog {
							fkUser = user.id,
							fkActionLog = EnumAuditAction.ProjectUpdateRemoveSprint,
							nuType = EnumAuditType.Project,
							fkTarget = this.id
						}.
						Create(db, "Sprint deleted: " + ent.stName , "");

						sprints = LoadSprints(db);
						logs = LoadLogs(db);
						break;
					}
			}

			return true;
		}
	}
}
