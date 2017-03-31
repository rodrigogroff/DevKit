using System.Collections.Generic;

namespace DataModel
{
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

		public string stUserMessage = "";

		public long? fkNewFlow = null;
		public string fkNewFlow_Message = "";

		public string updateCommand = "";
		public object anexedEntity;
	}
}
