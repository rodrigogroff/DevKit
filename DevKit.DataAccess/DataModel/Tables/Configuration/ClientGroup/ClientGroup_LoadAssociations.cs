using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class ClientGroup
	{
		public ClientGroup LoadAssociations(DevKitDB db)
		{
			var setup = db.Setup();

			var mdlUser = db.User(this.fkUser);

			sfkUser = mdlUser?.stLogin;
			sdtStart = dtStart?.ToString(setup.stDateFormat);

			clients = LoadClients(db);

			return this;
		}

		List<Client> LoadClients(DevKitDB db)
		{
			return (from e in db.ClientGroupAssociations
					 join eCli in db.Clients on e.fkClient equals eCli.id
					 where e.fkClientGroup == this.id
					 select eCli).
					 ToList();
		}
	}
}
