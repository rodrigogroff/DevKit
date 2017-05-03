
namespace DataModel
{
	public partial class ClientGroup
	{
		public ClientGroup LoadAssociations(DevKitDB db)
		{
			var setup = db.Setup();

			var mdlUser = db.User(this.fkUser);

			stUser = mdlUser?.stLogin;
			sdtStart = dtStart?.ToString(setup.stDateFormat);

			return this;
		}
	}
}
