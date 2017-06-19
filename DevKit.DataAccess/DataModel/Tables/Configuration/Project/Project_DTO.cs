using System.Collections.Generic;

namespace DataModel
{
	public partial class Project
	{
        public object anexedEntity;

		public string stUser = "",
						sdtCreation = "",
						updateCommand = "";

		public List<ProjectUser> users;
		public List<ProjectPhase> phases;
		public List<ProjectSprint> sprints;
		public List<ProjectLog> logs;
	}

    public class ProjectReport
    {
        public int count = 0;
        public List<Project> results = new List<Project>();
    }

    public class ProjectLog
	{
		public string sdtLog,
						stUser,
						stDetails;
	}
}
