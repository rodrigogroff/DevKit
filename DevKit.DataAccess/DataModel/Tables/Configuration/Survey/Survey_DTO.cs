using System.Collections.Generic;

namespace DataModel
{
	public partial class Survey
	{
		public object anexedEntity;

		public string sfkUser = "",
						sfkProject = "",
						sdtLog = "",
						updateCommand = "";

		//public List<ProjectLog> logs;
	}

	/*
	public class ProjectLog
	{
		public string sdtLog,
						stUser,
						stDetails;
	}*/
}
