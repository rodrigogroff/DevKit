using System.Collections.Generic;

namespace DataModel
{
    public partial class MedicoAddress
    {
        public string sfkEstado = "",
                      sfkCidade = "";
    }

    public partial class MedicoEmpresa
    {
        public string sfkEmpresa = "";
    }

    public partial class MedicoEmpresaTuss
    {
        public string stProcedimento, 
                      svrCoPart,
                      svrProcedimento;
    }

    public partial class Medico
    {
        public object anexedEntity;

        public string   sfkEspecialidade,
                        updateCommand = "";

        public List<MedicoEmpresaTuss> procedimentos;
        public List<MedicoEmpresa> empresas;
        public List<MedicoPhone> phones;
        public List<MedicoEmail> emails;
        public List<MedicoAddress> enderecos;        
    }

    public class MedicoReport
    {
        public int count = 0;
        public List<Medico> results = new List<Medico>();
    }
}
