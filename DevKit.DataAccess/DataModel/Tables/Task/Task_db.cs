using LinqToDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public class TaskFilter
	{
		public int skip, take;
		public string busca;

		public bool? complete;

		public long? nuPriority,
						fkProject,
						fkPhase,
						fkSprint,
						fkUserStart,
						fkUserResponsible,
						fkTaskType,
						fkTaskFlowCurrent,
						fkTaskCategory;

		public List<long?> lstProjects = null;
	}

	// --------------------------
	// properties
	// --------------------------

	public partial class Task
	{
		public string	sdtStart = "",
						snuPriority = "",
						sfkUserStart = "",
						sfkUserResponsible = "",
						sfkTaskType = "",
						sfkTaskCategory = "",
						sfkTaskFlowCurrent = "",
						sfkProject = "",
						sfkPhase = "",
						sfkSprint = "",
						sfkVersion = "";

		public List<TaskProgress> usrProgress;
		public List<TaskMessage> usrMessages;
		public List<TaskFlowChange> flows;
		public List<TaskTypeAccumulator> accs;

		// up commands

		public string stUserMessage = "";

		public long? fkNewFlow = null;
		public string fkNewFlow_Message = "";

		public string updateCommand = "";
		public object anexedEntity;
	}

	// --------------------------
	// functions
	// --------------------------

	public partial class Task
	{
		public IQueryable<Task> ComposedFilters(DevKitDB db, TaskFilter filter)
		{
			var query = from e in db.Tasks select e;

			if (filter.busca != null)
				query = from e in query
						where	e.stDescription.ToUpper().Contains(filter.busca) ||
								e.stLocalization.ToUpper().Contains(filter.busca) ||
								e.stTitle.ToUpper().Contains(filter.busca) 
						select e;

			if (filter.complete != null)
				query = from e in query where e.bComplete == filter.complete select e;

			if (filter.nuPriority != null)
				query = from e in query where e.nuPriority == filter.nuPriority select e;

			if (filter.fkProject != null)
				query = from e in query where e.fkProject == filter.fkProject select e;

			if (filter.lstProjects != null)
			{
				query = from e in query where filter.lstProjects.Contains(e.fkProject) select e;
			}

			if (filter.fkPhase != null)
				query = from e in query where e.fkPhase == filter.fkPhase select e;

			if (filter.fkTaskType != null)
				query = from e in query where e.fkTaskType == filter.fkTaskType select e;

			if (filter.fkTaskCategory != null)
				query = from e in query where e.fkTaskCategory == filter.fkTaskCategory select e;

			if (filter.fkUserStart != null)
				query = from e in query where e.fkUserStart == filter.fkUserStart select e;

			if (filter.fkTaskFlowCurrent != null)
				query = from e in query where e.fkTaskFlowCurrent == filter.fkTaskFlowCurrent select e;
			
			if (filter.fkUserResponsible != null)
				query = from e in query where e.fkUserResponsible == filter.fkUserResponsible select e;

			if (filter.complete != null)
				query = from e in query where e.bComplete == filter.complete select e;

			return query;
		}

		public Task LoadAssociations(DevKitDB db, bool IsListing = false)
		{
			var setup = db.Setup();

			sdtStart = dtStart?.ToString(setup.stDateFormat);
			snuPriority = new EnumPriority().lst.Where(t => t.id == nuPriority).FirstOrDefault().stName;
			sfkUserStart = db.User(fkUserStart).stLogin;

			sfkTaskCategory = db.TaskCategory(fkTaskCategory).stName;
			sfkTaskType = db.TaskType(fkTaskType).stName;
			sfkProject = db.Project(fkProject).stName;
			sfkPhase = db.ProjectPhase(fkPhase).stName;
			sfkSprint = db.ProjectSprint(fkSprint).stName;
			sfkTaskFlowCurrent = db.TaskFlow(fkTaskFlowCurrent).stName;

			if (fkVersion != null)
				sfkVersion = db.ProjectSprintVersion(fkVersion).stName;

			if (fkUserResponsible != null)
				sfkUserResponsible = db.User(fkUserResponsible).stLogin;

			if (!IsListing)
			{
				usrProgress = LoadProgress(db);
				usrMessages = LoadMessages(db);
				flows = LoadFlows(db);
				accs = LoadAccs(db);
			}

			return this;
		}

		public List<TaskProgress> LoadProgress(DevKitDB db)
		{
			var ret = (from e in db.TaskProgresses where e.fkTask == id select e).OrderByDescending(t => t.dtLog).ToList();

			var setup = db.Setup();

			for (int i = 0; i < ret.Count(); i++)
			{
				var item = ret.ElementAt(i);

				item.sdtLog = item.dtLog?.ToString(setup.stDateFormat);
				item.sfkUserAssigned = db.User(item.fkUserAssigned).stLogin;
			}

			return ret;
		}

		public List<TaskMessage> LoadMessages(DevKitDB db)
		{
			var ret = (from e in db.TaskMessages where e.fkTask == id select e).OrderByDescending(t => t.dtLog).ToList();

			var setup = db.Setup();

			for (int i = 0; i < ret.Count(); i++)
			{
				var item = ret.ElementAt(i);

				item.sdtLog = item.dtLog?.ToString(setup.stDateFormat);
				item.sfkUser = db.User(item.fkUser).stLogin;

				if (item.fkCurrentFlow != null)
					item.sfkFlow = db.TaskFlow(item.fkCurrentFlow).stName;
			}

			return ret;
		}

		public List<TaskFlowChange> LoadFlows(DevKitDB db)
		{
			var ret = (from e in db.TaskFlowChanges where e.fkTask == id select e).
				OrderByDescending(t => t.dtLog).
				ToList();

			var setup = db.Setup();

			for (int i = 0; i < ret.Count(); i++)
			{
				var item = ret.ElementAt(i);

				item.sdtLog = item.dtLog?.ToString(setup.stDateFormat);
				item.sfkUser = db.User(item.fkUser).stLogin;

				if (item.fkOldFlowState != null)
					item.sfkOldFlowState = db.TaskFlow(item.fkOldFlowState).stName;

				if (item.fkNewFlowState != null)
					item.sfkNewFlowState = db.TaskFlow(item.fkNewFlowState).stName;
			}

			return ret;
		}

		public List<TaskTypeAccumulator> LoadAccs(DevKitDB db)
		{
			var ret = (from e in db.TaskTypeAccumulators where e.fkTaskCategory == this.fkTaskCategory select e).
				ToList();

			var setup = db.Setup();
			var stypes = new EnumAccumulatorType().lst;

			for (int i = 0; i < ret.Count(); i++)
			{
				var item = ret.ElementAt(i);

				item.sfkTaskAccType = stypes.Where(y => y.id == item.fkTaskAccType).FirstOrDefault().stName;

				item.snuTotal = GetValueForType(db, item.sfkTaskAccType, id, item.id);

				// logs

				var logs = (from e in db.TaskAccumulatorValues
							where e.fkTask == id
							where e.fkTaskAcc == item.id
							select e).
							OrderByDescending ( y=> y.id ).
							ToList();

				foreach (var l in logs)
				{
					item.logs.Add(new LogAccumulatorValue()
					{
						sfkUser = db.User(l.fkUser).stLogin,
						sdtLog = l.dtLog?.ToString(setup.stDateFormat),
						sValue = GetValueForType(db, item.sfkTaskAccType, id, item.id, l.id)
					});
				}
			}

			return ret;
		}

		string GetValueForType(DevKitDB db, string _type, long task_id, long task_acc, long acc_id = 0 )
		{
			var ret = "";

			switch (_type)
			{
				case "Money":

					if (acc_id == 0)
						ret = ( from e in db.TaskAccumulatorValues
								where acc_id == 0 || e.id == acc_id 
								where e.fkTask == task_id
								where e.fkTaskAcc == task_acc
								select e).Select(y => y.nuValue).ToString();
					else
						ret = ( from e in db.TaskAccumulatorValues
								where e.fkTask == task_id
								where e.fkTaskAcc == task_acc
								select e).Sum(y => y.nuValue).ToString();

					break;

				case "Hours":

					long? hh = 0, mm = 0;

					if (acc_id == 0)
					{
						hh = (from e in db.TaskAccumulatorValues
							  where e.fkTask == task_id && e.fkTaskAcc == task_acc
							  select e).Sum(y => y.nuHourValue) * 60;

						if (hh == null) hh = 0;
					}						
					else
					{
						hh = (from e in db.TaskAccumulatorValues
							  where e.id == acc_id
							  where e.fkTask == task_id && e.fkTaskAcc == task_acc
							  select e).FirstOrDefault().nuHourValue;

						if (hh != null)
							hh = hh * 60;
						else
							hh = 0;
					}

					if (acc_id == 0)
					{
						mm = (from e in db.TaskAccumulatorValues
							  where e.fkTask == task_id && e.fkTaskAcc == task_acc
							  select e).Sum(y => y.nuMinValue);

						if (mm == null)
							mm = 0;
					}
					else
					{
						mm = (from e in db.TaskAccumulatorValues
							  where e.id == acc_id
							  where e.fkTask == task_id && e.fkTaskAcc == task_acc
							  select e).FirstOrDefault().nuMinValue;

						if (mm == null)
							mm = 0;
					}

					var tot = hh + mm;

					if (tot != 0)
					{
						var totHours = (hh + mm) / 60;
						var totMins = tot - totHours * 60;

						ret = totHours.ToString() + ":" + totMins.ToString().PadLeft(2, '0');
					}
					else
						ret = "00:00";

					break;
			}

			return ret;
		}

		bool CheckDuplicate(Task item, DevKitDB db)
		{
			var query = from e in db.Tasks select e;

			if (item.stTitle != null)
			{
				var _st = item.stTitle.ToUpper();
				query = from e in query where e.stTitle.ToUpper().Contains(_st) select e;
			}

			if (item.fkProject != null)
				query = from e in query where e.fkProject == item.fkProject select e;

			if (item.fkSprint != null)
				query = from e in query where e.fkSprint == item.fkSprint select e;

			if (item.id > 0)
				query = from e in query where e.id != item.id select e;

			return query.Any();
		}

		public bool Create(DevKitDB db, User usr, ref string resp)
		{
			bComplete = false;
			dtStart = DateTime.Now;
			fkUserStart = usr.id;
			fkTaskFlowCurrent = (from e in db.TaskFlows
								 where e.fkTaskType == this.fkTaskType
								 where e.fkTaskCategory == this.fkTaskCategory
								 select e).
								 OrderBy(t => t.nuOrder).
								 FirstOrDefault().
								 id;
			
			id = Convert.ToInt64(db.InsertWithIdentity(this));

			return true;
		}

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

		public bool CanDelete(DevKitDB db, ref string resp)
		{
			return false;
		}

		public void Delete(DevKitDB db)
		{
			db.Delete(this);
		}
	}
}
