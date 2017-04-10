using LinqToDB;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace DataModel
{
	public partial class Project
	{
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
						new AuditLog {
							fkUser = user.id,
							fkActionLog = EnumAuditAction.ProjectUpdate,
							nuType = EnumAuditType.Project,
							fkTarget = this.id
						}.
						Create(db, TrackChanges(db), "");

						db.Update(this);
						break;
					}

				case "newUser":
					{
						var ent = JsonConvert.DeserializeObject<ProjectUser>(anexedEntity.ToString());

						if (ent.id == 0)
						{
							if ((from ne in db.ProjectUsers
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
							Create(db, "", "");
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
							Create(db, "", "");
						}							

						users = LoadUsers(db);
						break;
					}

				case "removeUser":
					{
						db.Delete(JsonConvert.DeserializeObject<ProjectUser>(anexedEntity.ToString()));

						new AuditLog {
							fkUser = user.id,
							fkActionLog = EnumAuditAction.ProjectUpdateRemoveUser,
							nuType = EnumAuditType.Project,
							fkTarget = this.id
						}.
						Create(db, "", "");

						users = LoadUsers(db);
						break;
					}

				case "newPhase":
					{
						var ent = JsonConvert.DeserializeObject<ProjectPhase>(anexedEntity.ToString());

						if (ent.id == 0)
						{ 
							if ((from ne in db.ProjectPhases
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
							Create(db, "", "");
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
							Create(db, "", "");
						}
							
						phases = LoadPhases(db);
						break;
					}

				case "removePhase":
					{
						var mdlDel = JsonConvert.DeserializeObject<ProjectPhase>(anexedEntity.ToString());

						if ((from e in db.Tasks where e.fkPhase == mdlDel.id select e).Any())
						{
							resp = "This phase is being used in a task";
							return false;
						}

						db.Delete(mdlDel);

						new AuditLog {
							fkUser = user.id,
							fkActionLog = EnumAuditAction.ProjectUpdateRemovePhase,
							nuType = EnumAuditType.Project,
							fkTarget = this.id
						}.
						Create(db, "", "");

						phases = LoadPhases(db);
						break;
					}

				case "newSprint":
					{
						var ent = JsonConvert.DeserializeObject<ProjectSprint>(anexedEntity.ToString());

						if (ent.id == 0)
						{
							if ((from ne in db.ProjectSprints
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
							Create(db, "", "");

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
						break;
					}

				case "removeSprint":
					{
						var mdlDel = JsonConvert.DeserializeObject<ProjectSprint>(anexedEntity.ToString());

						if ((from e in db.Tasks where e.fkSprint == mdlDel.id select e).Any())
						{
							resp = "This sprint is being used in a task";
							return false;
						}

						db.Delete(mdlDel);

						new AuditLog {
							fkUser = user.id,
							fkActionLog = EnumAuditAction.ProjectUpdateRemoveSprint,
							nuType = EnumAuditType.Project,
							fkTarget = this.id
						}.
						Create(db, "", "");

						sprints = LoadSprints(db);
						break;
					}
			}

			return true;
		}
	}
}
