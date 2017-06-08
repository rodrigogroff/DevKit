using System.Collections.Generic;

namespace DataModel
{
	public partial class Project
	{
        public LoginInfo login;

        public object anexedEntity;

		public string stUser = "",
						sdtCreation = "",
						updateCommand = "";

		public List<ProjectUser> users;
		public List<ProjectPhase> phases;
		public List<ProjectSprint> sprints;
		public List<ProjectLog> logs;
	}

	public class ProjectLog
	{
		public string sdtLog,
						stUser,
						stDetails;
	}
}
