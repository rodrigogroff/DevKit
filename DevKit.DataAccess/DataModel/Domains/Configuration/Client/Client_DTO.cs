using System.Collections.Generic;

namespace DataModel
{
	public partial class Client
	{
        public object anexedEntity;

		public string sfkUser = "",
                        sfkEstado = "",
                        sfkCidade = "",
						sdtStart = "",
						updateCommand = "";
	}

    public class ClientReport
    {
        public int count = 0;
        public List<Client> results = new List<Client>();
    }   
}
