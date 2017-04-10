using LinqToDB;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace DataModel
{
	public partial class Task
	{
		public bool Update(DevKitDB db, User user, ref string resp)
		{
			switch (updateCommand)
			{
				case "entity":
					{
						var oldTask = (from ne in db.Tasks where ne.id == id select ne).
							FirstOrDefault();

						if (oldTask.fkUserResponsible != fkUserResponsible)
						{
							db.Insert(new TaskProgress()
							{
								dtLog = DateTime.Now,
								fkTask = id,
								fkUserAssigned = fkUserResponsible
							});

							new AuditLog {
								fkUser = user.id,
								fkActionLog = EnumAuditAction.TaskUpdateProgress,
								nuType = EnumAuditType.Task,
								fkTarget = this.id
							}.
							Create(db, "New assigned: " + db.User(fkUserResponsible).stLogin, "");
						}

						if (stUserMessage != "")
						{
							db.Insert(new TaskMessage()
							{
								stMessage = stUserMessage,
								dtLog = DateTime.Now,
								fkTask = id,
								fkUser = user.id,
								fkCurrentFlow = fkTaskFlowCurrent
							});

							stUserMessage = "";

							new AuditLog {
								fkUser = user.id,
								fkActionLog = EnumAuditAction.TaskUpdateProgress,
								nuType = EnumAuditType.Task,
								fkTarget = this.id
							}.
							Create(db, "New message", "");
						}

						if (fkNewFlow != null && oldTask.fkTaskFlowCurrent != fkNewFlow)
						{
							db.Insert(new TaskFlowChange()
							{
								dtLog = DateTime.Now,
								fkTask = id,
								fkUser = user.id,
								stMessage = fkNewFlow_Message,
								fkNewFlowState = fkNewFlow,
								fkOldFlowState = fkTaskFlowCurrent
							});

							fkTaskFlowCurrent = fkNewFlow;
							fkNewFlow = null;
							fkNewFlow_Message = "";

							var flowState = (from e in db.TaskFlows
											 where e.id == fkTaskFlowCurrent
											 select e).
											 FirstOrDefault();

							if (flowState.bForceComplete == true)
								this.bComplete = true;

							if (flowState.bForceOpen == true)
								this.bComplete = false;

							new AuditLog {
								fkUser = user.id,
								fkActionLog = EnumAuditAction.TaskUpdateProgress,
								nuType = EnumAuditType.Task,
								fkTarget = this.id
							}.
							Create(db, "State changed -> " + db.TaskFlow(fkNewFlow).stName, "");
						}

						new AuditLog
						{
							fkUser = user.id,
							fkActionLog = EnumAuditAction.TaskUpdate,
							nuType = EnumAuditType.Task,
							fkTarget = this.id
						}.
						Create(db, TrackChanges(db), "");

						db.Update(this);

						LoadAssociations(db);

						break;
					}

				case "newAcc":
					{
						var ent = JsonConvert.DeserializeObject<TaskAccumulatorValue>(anexedEntity.ToString());
												
						ent.fkTask = id;
						ent.fkUser = user.id;
						ent.dtLog = DateTime.Now;
						
						db.Insert(ent);

						new AuditLog {
							fkUser = user.id,
							fkActionLog = EnumAuditAction.TaskUpdateAccSaved,
							nuType = EnumAuditType.Task,
							fkTarget = this.id
						}.
						Create(db, "New time added: " + ent.nuHourValue.ToString() + ":" + ent.nuMinValue.ToString(), "");

						accs = LoadAccs(db);
						logs = LoadLogs(db);
						break;
					}
			}

			return true;
		}
	}
}
