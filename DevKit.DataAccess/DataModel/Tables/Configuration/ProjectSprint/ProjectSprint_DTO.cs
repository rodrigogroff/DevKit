using System.Collections.Generic;

namespace DataModel
{
	public partial class ProjectSprint
	{
        public object anexedEntity;

		public string sfkProject = "",
						sfkPhase = "",
						updateCommand = "";

		public List<ProjectSprintVersion> versions;
		public List<SprintLog> logs;
	}

	public class SprintLog
	{
		public string sdtLog,
						stUser,
						stDetails;
	}
}
