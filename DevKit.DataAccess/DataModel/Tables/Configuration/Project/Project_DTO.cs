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
}
