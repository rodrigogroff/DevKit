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

		public List<SurveyOption> options;
	}

	public class SurveySelOption
	{
		public long id = 0;
	}
}
