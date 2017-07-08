using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public partial class Survey
	{
		public Survey LoadAssociations(DevKitDB db)
		{
			var setup = db.GetSetup();

			var mdlUser = db.GetUser(this.fkUser);

			if (mdlUser != null)
				sfkUser = mdlUser.stLogin;

			sdtLog = dtLog?.ToString(setup.stDateFormat);

			if (fkProject != null)
				sfkProject = db.GetProject(fkProject).stName;

			options = LoadOptions(db);

			return this;
		}

		List<SurveyOption> LoadOptions(DevKitDB db)
		{
			return (from e in db.SurveyOption where e.fkSurvey == id select e).
				OrderBy(t => t.nuOrder).
				ToList();
		}
	}
}
