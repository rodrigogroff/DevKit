using System.Collections.Generic;

namespace DataModel
{
    public partial class EmpresaEndereco
    {
        public string sfkEstado = "",
                      sfkCidade = "";
    }

    public partial class Empresa
    {
        public object anexedEntity;

        public string sqtdCartoes = "",
                      updateCommand = "";

        public List<EmpresaSecao> secoes;
        public List<EmpresaTelefone> telefones;
        public List<EmpresaEmail> emails;
        public List<EmpresaEndereco> enderecos;
    }

    public class EmpresaReport
    {
        public int count = 0;
        public List<Empresa> results = new List<Empresa>();
    }

}
