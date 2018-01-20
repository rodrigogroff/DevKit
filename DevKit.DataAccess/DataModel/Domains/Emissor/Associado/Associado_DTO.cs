using System.Collections.Generic;

namespace DataModel
{
    public partial class AssociadoEndereco
    {
        public string sfkEstado = "",
                      sfkCidade = "";
    }

    public partial class Associado
    {
        public object anexedEntity;

		public string sdtLastContact = "",
                      sdtLastUpdate = "",
                      sfkUserLastContact = "",
                      sfkUserLastUpdate = "",
                      sfkUserAdd = "",
                      sdtStart = "",
                      snuAge = "",
                      stgStatus = "",
                      stgExpedicao = "",
                      updateCommand = "";

		public List<AssociadoTelefone> phones;
		public List<AssociadoEmail> emails;
        public List<AssociadoEndereco> enderecos;
    }

    public class AssociadoReport
    {
        public int count = 0;
        public List<Associado> results = new List<Associado>();
    }
}
