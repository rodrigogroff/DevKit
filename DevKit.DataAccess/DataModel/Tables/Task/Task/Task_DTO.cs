using System.Collections.Generic;

namespace DataModel
{
	public partial class Task
	{
		public object anexedEntity;

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
		
		public string stUserMessage = "",
						fkNewFlow_Message = "",
						updateCommand = "";

		public long? fkNewFlow = null;

		public List<TaskProgress> usrProgress;
		public List<TaskMessage> usrMessages;
		public List<TaskFlowChange> flows;
		public List<TaskTypeAccumulator> accs;
		public List<TaskLog> logs;
	}

	public class TaskLog
	{
		public string sdtLog,
						stUser,
						stDetails;
	}
}
