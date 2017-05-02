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
				dependencies = LoadDependencies(db);
				checkpoints = LoadCheckpoints(db);
				questions = LoadQuestions(db);
			}

			return this;
		}

		public List<TaskQuestion> LoadQuestions(DevKitDB db)
		{
			var ret = (from e in db.TaskQuestions
					   where e.fkTask == id
					   select e).
					   OrderByDescending(t => t.id).
					   ToList();

			var setup = db.Setup();

			foreach (var item in ret)
			{
				item.sfkUserOpen = db.User(item.fkUserOpen).stLogin;
				item.sfkUserDirected = db.User(item.fkUserDirected).stLogin;
				item.sdtOpen = item.dtOpen?.ToString(setup.stDateFormat);
				item.sdtClosed = item.dtClosed?.ToString(setup.stDateFormat);
			}

			return ret;
		}

		public List<TaskProgress> LoadProgress(DevKitDB db)
		{
			var ret = (from e in db.TaskProgresses
					   where e.fkTask == id
					   select e).
					   OrderByDescending(t => t.dtLog).
					   ToList();

			var setup = db.Setup();

			foreach (var item in ret)
			{ 
				item.sdtLog = item.dtLog?.ToString(setup.stDateFormat);
				item.sfkUserAssigned = db.User(item.fkUserAssigned).stLogin;
			}

			return ret;
		}

		public List<TaskDependency> LoadDependencies(DevKitDB db)
		{
			var ret = (from e in db.TaskDependencies
					   where e.fkMainTask == id
					   select e).
					   OrderByDescending(t => t.dtLog).
					   ToList();

			var setup = db.Setup();

			foreach (var item in ret)
			{
				item.sdtLog = item.dtLog?.ToString(setup.stDateFormat);
				item.sfkUser = db.User(item.fkUser).stLogin;

				var subTask = db.Task(item.fkSubTask);

				item.sfkTaskFlowCurrent = db.TaskFlow(subTask.fkTaskFlowCurrent).stName;
				item.stProtocol = subTask.stProtocol;
				item.stTitle = subTask.stTitle;
				item.stLocalization = subTask.stLocalization;
			}

			return ret;
		}
		
		public List<TaskMessage> LoadMessages(DevKitDB db)
		{
			var ret = (from e in db.TaskMessages
					   where e.fkTask == id
					   select e).
					   OrderByDescending(t => t.dtLog).
					   ToList();

			var setup = db.Setup();

			foreach (var item in ret)
			{
				item.sdtLog = item.dtLog?.ToString(setup.stDateFormat);
				item.sfkUser = db.User(item.fkUser).stLogin;

				if (item.fkCurrentFlow != null)
					item.sfkFlow = db.TaskFlow(item.fkCurrentFlow).stName;
			}

			return ret;
		}

		public List<TaskFlowChange> LoadFlows(DevKitDB db)
		{
			var ret =  (from e in db.TaskFlowChanges
					    where e.fkTask == id
					    select e).
						OrderByDescending(t => t.dtLog).
						ToList();

			var setup = db.Setup();

			foreach (var item in ret)
			{
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
			var setup = db.Setup();
			var stypes = new EnumAccumulatorType().lst;

			var ret = (from e in db.TaskTypeAccumulators
					   where e.fkTaskCategory == this.fkTaskCategory
					   select e).
					   ToList();

			foreach (var item in ret)
			{
				item.sfkTaskAccType = stypes.
					Where(y => y.id == item.fkTaskAccType).
						FirstOrDefault().
							stName;

				item.snuTotal = GetValueForType(db, item.sfkTaskAccType, id, item.id);

				var logs = (from e in db.TaskAccumulatorValues
							where e.fkTask == id
							where e.fkTaskAcc == item.id
							select e).
							OrderByDescending ( y=> y.id ).
							ToList();

				item.logs = new List<LogAccumulatorValue>();

				foreach (var l in logs)
				{
					item.logs.Add(new LogAccumulatorValue()
					{
						id = l.id,
						sfkUser = db.User(l.fkUser).stLogin,
						sdtLog = l.dtLog?.ToString(setup.stDateFormat),
						sValue = GetValueForType(db, item.sfkTaskAccType, id, item.id, l.id)
					});
				}
			}

			return ret;
		}

		List<TaskCheckPoint> LoadCheckpoints(DevKitDB db)
		{
			var setup = db.Setup();

			var lst = (from e in db.TaskCheckPoints
					   where e.fkCategory == this.fkTaskCategory
					   select e).
					   ToList();

			foreach (var item in lst)
			{
				var mark = (from e in db.TaskCheckPointMarks
							where e.fkCheckPoint == item.id
							where e.fkTask == this.id
							select e).
							FirstOrDefault();

				if (mark != null)
				{
					item.bSelected = true;
					item.sdtLog = mark.dtLog?.ToString(setup.stDateFormat);
					item.sfkUser = db.User(mark.fkUser).stLogin;
				}
				else
					item.bSelected = false;
			}

			return lst;
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

					stUser = lstUsers.
								Where(y => y.id == item.fkUser).
									FirstOrDefault().
										stLogin,

					stDetails = item.stLog
				});
			}

			return ret;
		}
	}
}
