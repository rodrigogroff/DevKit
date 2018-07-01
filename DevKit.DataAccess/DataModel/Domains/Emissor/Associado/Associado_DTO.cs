using System.Collections.Generic;

namespace DataModel
{
    public partial class AssociadoEndereco
    {
        public string sfkEstado = "",
                      sfkCidade = "";
    }

    public partial class AssociadoDependente
    {
        public string snuTit = "",
                      sdtNasc = "",
                      sfkTipoCoberturaDependente = "";
    }

    public partial class Associado
    {
        public object anexedEntity;

		public string sdtLastContact = "",
                      sdtLastUpdate = "",
                      sfkUserLastContact = "",
                      sfkUserLastUpdate = "",
                      sfkUserAdd = "",
                      sfkSecao = "",
                      sdtStart = "",
                      snuAge = "",
                      stgStatus = "",
                      stgExpedicao = "",
                      svrMaxEmp = "",
                      updateCommand = "";

		public List<AssociadoTelefone> phones;
		public List<AssociadoEmail> emails;
        public List<AssociadoEndereco> enderecos;
        public List<AssociadoDependente> dependentes;

        public bool bConfigSenha = false;
    }

    public class AssociadoReport
    {
        public int count = 0;
        public List<Associado> results = new List<Associado>();
    }
}
