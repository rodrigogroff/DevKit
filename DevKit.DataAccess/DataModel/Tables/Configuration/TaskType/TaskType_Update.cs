using LinqToDB;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace DataModel
{
	public partial class TaskType
	{
		public bool Update(DevKitDB db, User user, ref string resp)
		{
			if (CheckDuplicate(this, db))
			{
				resp = "Task type name already taken";
				return false;
			}

			switch (updateCommand)
			{
				case "entity":
					{
						db.Update(this);

						new AuditLog { fkUser = user.id, fkActionLog = EnumAuditAction.TaskTypeUpdate }.Create(db, "", "");

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

							new AuditLog { fkUser = user.id, fkActionLog = EnumAuditAction.TaskTypeUpdate }.Create(db, "", "");
						}
						else
						{
							db.Update(ent);

							new AuditLog { fkUser = user.id, fkActionLog = EnumAuditAction.TaskTypeUpdateCategory }.Create(db, "", "");
						}							

						categories = LoadCategories(db);

						break;
					}

				case "removeCategorie":
					{
						var categDel = JsonConvert.DeserializeObject<TaskCategory>(anexedEntity.ToString());

						if ((from e in db.Tasks where e.fkTaskCategory == categDel.id select e).Any())
						{
							resp = "This category is being used in a task";
							return false;
						}

						db.Delete(categDel);

						new AuditLog { fkUser = user.id, fkActionLog = EnumAuditAction.TaskTypeRemoveCategory }.Create(db, "", "");

						categories = LoadCategories(db);
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

							new AuditLog { fkUser = user.id, fkActionLog = EnumAuditAction.CategoryAddFlow }.Create(db, "", "");
						}
						else
						{
							db.Update(ent);

							new AuditLog { fkUser = user.id, fkActionLog = EnumAuditAction.CategoryUpdateFlow }.Create(db, "", "");
						}							
						
						break;
					}

				case "removeFlow":
					{
						var flowDel = JsonConvert.DeserializeObject<TaskFlow>(anexedEntity.ToString());

						if ((from e in db.Tasks where e.fkTaskFlowCurrent == flowDel.id select e).Any())
						{
							resp = "This flow is being used in a task";
							return false;
						}

						db.Delete(flowDel);

						new AuditLog { fkUser = user.id, fkActionLog = EnumAuditAction.CategoryRemoveFlow }.Create(db, "", "");

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

							new AuditLog { fkUser = user.id, fkActionLog = EnumAuditAction.CategoryAddAccumulator }.Create(db, "", "");
						}
						else
						{
							db.Update(ent);

							new AuditLog { fkUser = user.id, fkActionLog = EnumAuditAction.CategoryUpdateAccumulator }.Create(db, "", "");
						}							

						break;
					}

				case "removeAcc":
					{
						var accDel = JsonConvert.DeserializeObject<TaskTypeAccumulator>(anexedEntity.ToString());

						db.Delete(accDel);

						new AuditLog { fkUser = user.id, fkActionLog = EnumAuditAction.CategoryRemoveAccumulator }.Create(db, "", "");

						break;
					}
			}

			return true;
		}
	}
}
