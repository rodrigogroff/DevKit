using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class ClientGroup
	{
		public ClientGroup LoadAssociations(DevKitDB db)
		{
			var setup = db.GetSetup();

			var mdlUser = db.GetUser(this.fkUser);

			sfkUser = mdlUser?.stLogin;
			sdtStart = dtStart?.ToString(setup.stDateFormat);

			clients = LoadClients(db);

			return this;
		}

        public ClientGroup ClearAssociations()
        {
            clients = null;

            return this;
        }

        List<Client> LoadClients(DevKitDB db)
		{
			return (from e in db.ClientGroupAssociation
					 join eCli in db.Client on e.fkClient equals eCli.id
					 where e.fkClientGroup == this.id
					 select eCli).
					 ToList();
		}
	}
}
