
namespace DataModel
{
	public partial class CompanyNews
	{
		public CompanyNews LoadAssociations(DevKitDB db)
		{
			var setup = db.GetSetup();

			var mdlUser = db.GetUser(this.fkUser);

			if (mdlUser != null)
				sfkUser = mdlUser.stLogin;

			sdtLog = dtLog?.ToString(setup.stDateFormat);

			if (fkProject != null)
				sfkProject = db.GetProject(fkProject).stName;

			stMessage = stMessage.Replace("\n", "<br>");
			
			return this;
		}
	}
}
