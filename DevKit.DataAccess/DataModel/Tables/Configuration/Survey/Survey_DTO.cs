using System.Collections.Generic;

namespace DataModel
{
	public partial class Survey
	{
        public bool bUserSelected;
		
		public object anexedEntity;

		public string sfkUser = "",
						sfkProject = "",
						sdtLog = "",
						sNuPct = "",				
						updateCommand = "";

		public List<SurveyOption> options;
	}

    public class SurveyReport
    {
        public int count = 0;
        public List<Survey> results = new List<Survey>();
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
