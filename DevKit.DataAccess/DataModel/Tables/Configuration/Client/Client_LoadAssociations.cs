
namespace DataModel
{
	public partial class Client
	{
		public Client LoadAssociations(DevKitDB db)
		{
			var setup = db.Setup();

			var mdlUser = db.User(this.fkUser);

			stUser = mdlUser?.stLogin;
			sdtStart = dtStart?.ToString(setup.stDateFormat);

			return this;
		}
	}
}
