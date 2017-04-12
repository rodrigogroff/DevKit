using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public partial class Survey
	{
		public Survey LoadAssociations(DevKitDB db)
		{
			var setup = db.Setup();

			var mdlUser = db.User(this.fkUser);

			if (mdlUser != null)
				sfkUser = mdlUser.stLogin;

			sdtLog = dtLog?.ToString(setup.stDateFormat);

			if (fkProject != null)
				sfkProject = db.Project(fkProject).stName;

			options = LoadOptions(db);

			return this;
		}

		List<SurveyOption> LoadOptions(DevKitDB db)
		{
			return (from e in db.SurveyOptions where e.fkSurvey == id select e).
				OrderBy(t => t.nuOrder).
				ToList();
		}
	}
}
