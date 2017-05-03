using LinqToDB;
using System;
using System.Linq;

namespace DataModel
{
	public partial class Task
	{
		public bool Update_Entity(DevKitDB db, User user, ref string resp)
		{				
			var oldTask = (from ne in db.Tasks where ne.id == id select ne).FirstOrDefault();

			if (oldTask.fkUserResponsible != fkUserResponsible) 
			{
				#region - code (change assignee) - 

				db.Insert(new TaskProgress()
				{
					dtLog = DateTime.Now,
					fkTask = id,
					fkUserAssigned = fkUserResponsible
				});

				new AuditLog
				{
					fkUser = user.id,
					fkActionLog = EnumAuditAction.TaskUpdateProgress,
					nuType = EnumAuditType.Task,
					fkTarget = this.id
				}.
				Create(db, "New assigned: " + db.User(fkUserResponsible).stLogin, "");

				#endregion
			}

			if (stUserMessage != "")
			{
				#region - code (save message) - 

				db.Insert(new TaskMessage()
				{
					stMessage = stUserMessage,
					dtLog = DateTime.Now,
					fkTask = id,
					fkUser = user.id,
					fkCurrentFlow = fkTaskFlowCurrent
				});

				stUserMessage = "";

				new AuditLog
				{
					fkUser = user.id,
					fkActionLog = EnumAuditAction.TaskUpdateProgress,
					nuType = EnumAuditType.Task,
					fkTarget = this.id
				}.
				Create(db, "New message", "");

				#endregion
			}

			if (checkpoints != null && checkpoints.Count() > 0)
			{
				#region - code (mark / unmark checkpoints) -

				foreach (var itemCheck in checkpoints)
				{
					var reg = (from e in db.TaskCheckPointMarks
								where e.fkCheckPoint == itemCheck.id
								where e.fkTask == this.id
								select e).
								FirstOrDefault();
								
					if (itemCheck.bSelected == true)
					{
						if (reg == null)
							db.Insert(new TaskCheckPointMark
							{
								fkCheckPoint = itemCheck.id,
								fkTask = this.id,
								fkUser = user.id,
								dtLog = DateTime.Now
							});
					}
					else
						db.Delete(reg);
				}

				#endregion
			}

			if (fkNewFlow != null && oldTask.fkTaskFlowCurrent != fkNewFlow)
			{
				var flowDestinyState = (from e in db.TaskFlows
										where e.id == fkNewFlow
										select e).
										FirstOrDefault();

				#region - code (changing state) - 

				if (flowDestinyState.bForceComplete == true)
				{
					#region - code (check dependencies) -

					var tmp_dependencies = (from e in db.TaskDependencies
										   	where e.fkMainTask == id
											select e).
											ToList();

					if (tmp_dependencies.Count() > 0)
					{
						foreach (var item in tmp_dependencies)
						{
							var subTask = db.Task(item.fkSubTask);

							if (subTask.bComplete == false)
							{
								resp = "Sub-Task " + subTask.stProtocol + " is still open!";
								return false;
							}
						}
					}

					#endregion

					#region - code (check points) -

					var lstChecks = (from e in db.TaskCheckPoints
										where e.fkCategory == this.fkTaskCategory
										where e.bMandatory == true
										select e).
										ToList();

					foreach (var item in lstChecks)
					{									
						if ( !(from e in db.TaskCheckPointMarks
								where e.fkCheckPoint == item.id
								where e.fkTask == this.id
								select e).
								Any())
						{
							resp = "Check Point mandatory: " + item.stName;
							return false;
						}
					}

					#endregion
				}

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

				if (flowDestinyState.bForceComplete == true)
					this.bComplete = true;

				if (flowDestinyState.bForceOpen == true)
					this.bComplete = false;

				new AuditLog
				{
					fkUser = user.id,
					fkActionLog = EnumAuditAction.TaskUpdateProgress,
					nuType = EnumAuditType.Task,
					fkTarget = this.id
				}.
				Create(db, "State changed -> " + db.TaskFlow(fkTaskFlowCurrent).stName, "");

				#endregion
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

			return true;
		}
	}
}
