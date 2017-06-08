using System.Collections.Generic;

namespace DataModel
{
	public partial class Survey
	{
        public LoginInfo login;

        public bool bUserSelected;
		
		public object anexedEntity;

		public string sfkUser = "",
						sfkProject = "",
						sdtLog = "",
						sNuPct = "",				
						updateCommand = "";

		public List<SurveyOption> options;
	}

	public partial class SurveyOption
	{
		public bool bChecked;
		public string sNuOptionPct;
	}

	public class SurveySelOption
	{
		public long id = 0;
	}
}
