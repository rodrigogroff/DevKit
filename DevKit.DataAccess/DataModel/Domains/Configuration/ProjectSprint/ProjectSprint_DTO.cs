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

    public class ProjectSprintReport
    {
        public int count = 0;
        public List<ProjectSprint> results = new List<ProjectSprint>();
    }

    public class SprintLog
	{
		public string sdtLog,
						stUser,
						stDetails;
	}
}
