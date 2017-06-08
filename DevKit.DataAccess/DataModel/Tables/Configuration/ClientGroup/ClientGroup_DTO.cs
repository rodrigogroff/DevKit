using System.Collections.Generic;

namespace DataModel
{
	public partial class ClientGroup
	{
        public LoginInfo login;

        public object anexedEntity;

		public string sfkUser = "",
						sdtStart = "",
						updateCommand = "";

		public List<Client> clients;
	}
}
