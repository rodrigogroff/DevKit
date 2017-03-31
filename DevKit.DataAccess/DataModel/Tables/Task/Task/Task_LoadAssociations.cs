using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class Task
	{
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

		public string GetValueForType(DevKitDB db, string _type, long task_id, long task_acc, long accVal_id = 0, List<long> lstRange = null )
		{
			var ret = "";

			switch (_type)
			{
				case "Money":

					if (accVal_id == 0)
						ret = ( from e in db.TaskAccumulatorValues
								where accVal_id == 0 || e.id == accVal_id 
								where task_id ==0 || e.fkTask == task_id
								where lstRange == null || lstRange.Contains ( (long)e.fkTask)
								where e.fkTaskAcc == task_acc
								select e).Select(y => y.nuValue).ToString();
					else
						ret = ( from e in db.TaskAccumulatorValues
								where task_id == 0 || e.fkTask == task_id
								where e.fkTaskAcc == task_acc
								select e).Sum(y => y.nuValue).ToString();

					break;

				case "Hours":

					long? hh = 0, mm = 0;

					if (accVal_id == 0)
					{
						hh = (from e in db.TaskAccumulatorValues
							  where task_id == 0 || e.fkTask == task_id
							  where lstRange == null || lstRange.Contains((long)e.fkTask)
							  where e.fkTaskAcc == task_acc
							  select e).Sum(y => y.nuHourValue) * 60;

						if (hh == null) hh = 0;
					}						
					else
					{
						hh = (from e in db.TaskAccumulatorValues
							  where e.id == accVal_id
							  where task_id == 0 || e.fkTask == task_id
							  where e.fkTaskAcc == task_acc
							  select e).FirstOrDefault().nuHourValue;

						if (hh != null)
							hh = hh * 60;
						else
							hh = 0;
					}

					if (accVal_id == 0)
					{
						mm = (from e in db.TaskAccumulatorValues
							  where task_id == 0 || e.fkTask == task_id
							  where lstRange == null || lstRange.Contains((long)e.fkTask)
							  where e.fkTaskAcc == task_acc
							  select e).Sum(y => y.nuMinValue);

						if (mm == null)
							mm = 0;
					}
					else
					{
						mm = (from e in db.TaskAccumulatorValues
							  where e.id == accVal_id
							  where task_id == 0 || e.fkTask == task_id
							  where e.fkTaskAcc == task_acc
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
	}
}
