using System.Collections.Generic;

namespace DataModel
{
	public partial class ProjectSprint
	{
		public string sfkProject = "";
		public string sfkPhase = "";

		public List<ProjectSprintVersion> versions;

		public string updateCommand = "";
		public object anexedEntity;		
	}
}
