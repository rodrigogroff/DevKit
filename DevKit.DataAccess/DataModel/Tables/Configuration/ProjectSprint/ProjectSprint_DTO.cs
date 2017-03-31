using System.Collections.Generic;

namespace DataModel
{
	public partial class ProjectSprint
	{
		public string sdtStart = "";
		public string sdtEnd = "";
		public string sfkProject = "";
		public string sfkPhase = "";

		public List<ProjectSprintVersion> versions;

		public string updateCommand = "";
		public object anexedEntity;		
	}
}
