using System.Collections.Generic;

namespace DataModel
{
	public partial class ProjectSprintVersion
	{
		public string sfkVersionState;
	}

    public class ProjectSprintVersionReport
    {
        public int count = 0;
        public List<ProjectSprintVersion> results = new List<ProjectSprintVersion>();
    }
}
