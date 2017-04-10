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
			snuPriority = new EnumPriority().Get((long)nuPriority).stName;
			sfkUserStart = db.User(fkUserStart)?.stLogin;

			sfkTaskCategory = db.TaskCategory(fkTaskCategory)?.stName;
			sfkTaskType = db.TaskType(fkTaskType)?.stName;
			sfkProject = db.Project(fkProject)?.stName;
			sfkPhase = db.ProjectPhase(fkPhase)?.stName;
			sfkSprint = db.ProjectSprint(fkSprint)?.stName;
			sfkTaskFlowCurrent = db.TaskFlow(fkTaskFlowCurrent)?.stName;
			sfkVersion = db.ProjectSprintVersion(fkVersion)?.stName;
			sfkUserResponsible = db.User(fkUserResponsible)?.stLogin;

			if (!IsListing)
			{
				usrProgress = LoadProgress(db);
				usrMessages = LoadMessages(db);
				flows = LoadFlows(db);
				accs = LoadAccs(db);
				logs = LoadLogs(db);
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

		List<TaskLog> LoadLogs(DevKitDB db)
		{
			var setup = db.Setup();

			var lstLogs = (from e in db.AuditLogs
						   where e.nuType == EnumAuditType.Task
						   where e.fkTarget == this.id
						   select e).
						   OrderByDescending(y => y.id).
						   ToList();

			var lstUsers = (from e in lstLogs
							join eUser in db.Users on e.fkUser equals eUser.id
							select eUser).
							ToList();

			var ret = new List<TaskLog>();

			foreach (var item in lstLogs)
			{
				ret.Add(new TaskLog
				{
					sdtLog = item.dtLog?.ToString(setup.stDateFormat),
					stUser = lstUsers.Where(y => y.id == item.fkUser).FirstOrDefault().stLogin,
					stDetails = item.stLog
				});
			}

			return ret;
		}
	}
}
