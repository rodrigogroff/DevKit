using System.Collections.Generic;

namespace DataModel
{
	public class TaskFilter
	{
		public int skip, take;
		public string busca;

		public bool? complete, kpa, expired;

		public long? nuPriority,
						fkProject,
						fkClient,
						fkClientGroup,
						fkPhase,
						fkSprint,
						fkUserStart,
						fkUserResponsible,
						fkTaskType,
						fkTaskFlowCurrent,
						fkTaskCategory;

		public List<long?> lstProjects = null;
	}
}
