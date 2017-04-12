using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public partial class CompanyNews
	{
		public CompanyNews LoadAssociations(DevKitDB db)
		{
			var setup = db.Setup();

			var mdlUser = db.User(this.fkUser);

			sfkUser = mdlUser.stLogin;
			sdtLog = dtLog?.ToString(setup.stDateFormat);

			//logs = LoadLogs(db);

			return this;
		}
	}
}
