using LinqToDB;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace DataModel
{
	public partial class Task
	{
		public bool Update(DevKitDB db, User userLogged, ref string resp)
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
						}

						if (stUserMessage != "")
						{
							db.Insert(new TaskMessage()
							{
								stMessage = stUserMessage,
								dtLog = DateTime.Now,
								fkTask = id,
								fkUser = userLogged.id,
								fkCurrentFlow = fkTaskFlowCurrent
							});

							stUserMessage = "";
						}

						if (fkNewFlow != null && oldTask.fkTaskFlowCurrent != fkNewFlow)
						{
							db.Insert(new TaskFlowChange()
							{
								dtLog = DateTime.Now,
								fkTask = id,
								fkUser = userLogged.id,
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
						}

						db.Update(this);
						LoadAssociations(db);

						break;
					}

				case "newAcc":
					{
						var ent = JsonConvert.DeserializeObject<TaskAccumulatorValue>(anexedEntity.ToString());

						
						ent.fkTask = id;
						ent.fkUser = userLogged.id;
						ent.dtLog = DateTime.Now;
						
						db.Insert(ent);
						accs = LoadAccs(db);						
						break;
					}
			}

			return true;
		}
	}
}
