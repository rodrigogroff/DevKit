using System.Collections.Generic;

namespace DataModel
{
    public partial class CredenciadoEndereco
    {
        public string sfkEstado = "",
                      sfkCidade = "";
    }

    public partial class CredenciadoEmpresa
    {
        public string sfkEmpresa = "";
    }

    public partial class CredenciadoEmpresaTuss
    {
        public string stProcedimento, 
                      svrCoPart,
                      svrProcedimento;
    }

    public partial class Credenciado
    {
        public object anexedEntity;

        public string   sfkEspecialidade,
                        snuTipo,
                        updateCommand = "";

        public List<CredenciadoEmpresaTuss> procedimentos;
        public List<CredenciadoEmpresa> empresas;
        public List<CredenciadoTelefone> phones;
        public List<CredenciadoEmail> emails;
        public List<CredenciadoEndereco> enderecos;        
    }

    public class CredenciadoReport
    {
        public int count = 0;
        public List<Credenciado> results = new List<Credenciado>();
    }
}
