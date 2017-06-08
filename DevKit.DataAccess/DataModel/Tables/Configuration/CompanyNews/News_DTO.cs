using System.Collections.Generic;

namespace DataModel
{
	public partial class CompanyNews
	{
        public LoginInfo login;

        public object anexedEntity;

		public string sfkUser = "",
						sfkProject = "",
						sdtLog = "",
						updateCommand = "";
	}
}
