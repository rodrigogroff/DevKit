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
				resp = "Nome de projeto já usado";
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
						Create(db, "Usuário removido: " + db.GetUser(ent.fkUser), "");

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
								resp = "Fase já acrescentada no projeto!";
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
							Create(db, "Fase nova: " + ent.stName, "");
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
							Create(db, "Fase atualizada: " + ent.stName, "");
						}
						break;
					}

				case "removePhase":
					{
						var ent = JsonConvert.DeserializeObject<ProjectPhase>(anexedEntity.ToString());

						if ((from e in db.Task where e.fkPhase == ent.id select e).Any())
						{
							resp = "Esta fase está sendo usada em uma tarefa";
							return false;
						}

						db.Delete(ent);

						new AuditLog {
							fkUser = user.id,
							fkActionLog = EnumAuditAction.ProjectUpdateRemovePhase,
							nuType = EnumAuditType.Project,
							fkTarget = this.id
						}.
						Create(db, "Fase removida: " + ent.stName, "");
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
								resp = "Sprint já adicionado no projeto!";
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
							Create(db, "Novo sprint: " + ent.stName, "");

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
						break;
					}

				case "removeSprint":
					{
						var ent = JsonConvert.DeserializeObject<ProjectSprint>(anexedEntity.ToString());

						if ((from e in db.Task where e.fkSprint == ent.id select e).Any())
						{
							resp = "Este sprint já está sendo usando em uma tarefa";
							return false;
						}

						db.Delete(ent);

						new AuditLog {
							fkUser = user.id,
							fkActionLog = EnumAuditAction.ProjectUpdateRemoveSprint,
							nuType = EnumAuditType.Project,
							fkTarget = this.id
						}.
						Create(db, "Sprint removido: " + ent.stName , "");

						break;
					}
			}

			return true;
		}
	}
}
