using LinqToDB;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace DataModel
{
	public partial class TaskType
	{
		public bool Update(DevKitDB db, ref string resp)
		{
			var user = db.GetCurrentUser();

			if (CheckDuplicate(this, db))
			{
				resp = "Task type name already taken";
				return false;
			}

			switch (updateCommand)
			{
				case "entity":
					{
						new AuditLog {
							fkUser = user.id,
							fkActionLog = EnumAuditAction.TaskTypeUpdate,
							nuType = EnumAuditType.TaskType,
							fkTarget = this.id
						}.
						Create(db, TrackChanges(db), "");

						db.Update(this);

						logs = LoadLogs(db);
						break;
					}

				case "newCategorie":
					{
						var ent = JsonConvert.DeserializeObject<TaskCategory>(anexedEntity.ToString());

						ent.fkTaskType = id;

						if (ent.id == 0)
						{
							if ((from ne in db.TaskCategories
								 where ne.stName.ToUpper() == ent.stName.ToUpper() 
								 where ne.fkTaskType == id
								 select ne).Any())
							{
								resp = "Category already added to task type!";
								return false;
							}

							ent.id = Convert.ToInt64(db.InsertWithIdentity(ent));

							int order = 1;

							db.Insert(new TaskFlow()
							{
								fkTaskCategory = ent.id,
								fkTaskType = id,
								nuOrder = order++,
								bForceOpen = true,
								stName = "Open"
							});

							db.Insert(new TaskFlow()
							{
								fkTaskCategory = ent.id,
								fkTaskType = id,
								nuOrder = order++,
								bForceOpen = true,
								stName = "Re-Open"
							});

							db.Insert(new TaskFlow()
							{
								fkTaskCategory = ent.id,
								fkTaskType = id,
								nuOrder = order++,
								stName = "Analysis"
							});

							db.Insert(new TaskFlow()
							{
								fkTaskCategory = ent.id,
								fkTaskType = id,
								nuOrder = order++,
								stName = "Development"
							});

							db.Insert(new TaskFlow()
							{
								fkTaskCategory = ent.id,
								fkTaskType = id,
								nuOrder = order++,
								bForceComplete = true,
								stName = "Closed"
							});

							db.Insert(new TaskFlow()
							{
								fkTaskCategory = ent.id,
								fkTaskType = id,
								nuOrder = order++,
								bForceComplete = true,
								stName = "Cancelled"
							});

							new AuditLog {
								fkUser = user.id,
								fkActionLog = EnumAuditAction.TaskTypeUpdate,
								nuType = EnumAuditType.TaskType,
								fkTarget = this.id
							}.
							Create(db, "New category: " + ent.stName, "");
						}
						else
						{
							db.Update(ent);

							new AuditLog {
								fkUser = user.id,
								fkActionLog = EnumAuditAction.TaskTypeUpdateCategory,
								nuType = EnumAuditType.TaskType,
								fkTarget = this.id
							}.
							Create(db, "Edit category: " + ent.stName, "");
						}							

						categories = LoadCategories(db);
						logs = LoadLogs(db);

						break;
					}

				case "removeCategorie":
					{
						var ent = JsonConvert.DeserializeObject<TaskCategory>(anexedEntity.ToString());

						if ((from e in db.Tasks where e.fkTaskCategory == ent.id select e).Any())
						{
							resp = "This category is being used in a task";
							return false;
						}

						db.Delete(ent);

						new AuditLog {
							fkUser = user.id,
							fkActionLog = EnumAuditAction.TaskTypeRemoveCategory,
							nuType = EnumAuditType.TaskType,
							fkTarget = this.id
						}.
						Create(db, "Category deleted: " + ent.stName, "");

						categories = LoadCategories(db);
						logs = LoadLogs(db);
						break;
					}

				case "newFlow":
					{
						var ent = JsonConvert.DeserializeObject<TaskFlow>(anexedEntity.ToString());

						if (ent.id == 0)
						{
							if ((from e in db.TaskFlows
								 where e.fkTaskCategory == ent.fkTaskCategory
								 where e.fkTaskType == ent.fkTaskType
								 where e.stName.ToUpper() == ent.stName.ToUpper() 
								 select e).Any())
							{
								resp = "Flow already added to task type!";
								return false;
							}

							ent.fkTaskType = id;

							db.Insert(ent);

							new AuditLog {
								fkUser = user.id,
								fkActionLog = EnumAuditAction.CategoryAddFlow,
								nuType = EnumAuditType.TaskType,
								fkTarget = this.id
							}.
							Create(db, "New flow: " + ent.stName, "");
						}
						else
						{
							db.Update(ent);

							new AuditLog {
								fkUser = user.id,
								fkActionLog = EnumAuditAction.CategoryUpdateFlow,
								nuType = EnumAuditType.TaskType,
								fkTarget = this.id
							}.
							Create(db, "Edit flow: " + ent.stName, "");
						}

						logs = LoadLogs(db);
						break;
					}

				case "removeFlow":
					{
						var ent = JsonConvert.DeserializeObject<TaskFlow>(anexedEntity.ToString());

						if ((from e in db.Tasks where e.fkTaskFlowCurrent == ent.id select e).Any())
						{
							resp = "This flow is being used in a task";
							return false;
						}

						db.Delete(ent);

						new AuditLog {
							fkUser = user.id,
							fkActionLog = EnumAuditAction.CategoryRemoveFlow,
							nuType = EnumAuditType.TaskType,
							fkTarget = this.id
						}.
						Create(db, "Flow delete: " + ent.stName, "");

						logs = LoadLogs(db);
						break;
					}

				case "newAcc":
					{
						var ent = JsonConvert.DeserializeObject<TaskTypeAccumulator>(anexedEntity.ToString());

						if (ent.id == 0)
						{
							if ((from e in db.TaskTypeAccumulators
								 where e.fkTaskFlow == ent.fkTaskFlow
								 where e.fkTaskCategory == ent.fkTaskCategory
								 where e.fkTaskType == ent.fkTaskType
								 where e.stName.ToUpper() == ent.stName.ToUpper() 
								 select e).Any())
							{
								resp = "Accumulator already added to task type!";
								return false;
							}

							ent.fkTaskType = id;

							db.Insert(ent);

							new AuditLog {
								fkUser = user.id,
								fkActionLog = EnumAuditAction.CategoryAddAccumulator,
								nuType = EnumAuditType.TaskType,
								fkTarget = this.id
							}.
							Create(db, "New accumulator: " + ent.stName, "");
						}
						else
						{
							db.Update(ent);

							new AuditLog {
								fkUser = user.id,
								fkActionLog = EnumAuditAction.CategoryUpdateAccumulator,
								nuType = EnumAuditType.TaskType,
								fkTarget = this.id
							}.
							Create(db, "Edit accumulator: " + ent.stName, "");
						}

						logs = LoadLogs(db);
						break;
					}

				case "removeAcc":
					{
						var ent = JsonConvert.DeserializeObject<TaskTypeAccumulator>(anexedEntity.ToString());

						db.Delete(ent);

						new AuditLog {
							fkUser = user.id,
							fkActionLog = EnumAuditAction.CategoryRemoveAccumulator,
							nuType = EnumAuditType.TaskType,
							fkTarget = this.id
						}.
						Create(db, "Accumulator deleted: " + ent.stName, "");

						logs = LoadLogs(db);
						break;
					}
			}

			return true;
		}
	}
}
