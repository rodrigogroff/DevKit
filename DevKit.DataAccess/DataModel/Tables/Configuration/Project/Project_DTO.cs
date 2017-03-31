using System.Collections.Generic;

namespace DataModel
{
	public partial class Project
	{
		public string stUser = "";
		public string sdtCreation = "";

		public List<ProjectUser> users;
		public List<ProjectPhase> phases;
		public List<ProjectSprint> sprints;

		public string updateCommand = "";
		public object anexedEntity;
	}

	public partial class ProjectUser
	{
		public string stUser = "";
		public string sdtJoin = "";
	}

	public partial class ProjectPhase
	{
		public string sdtStart = "";
		public string sdtEnd = "";
	}
}
