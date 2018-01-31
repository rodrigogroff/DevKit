using System.Collections.Generic;

namespace DataModel
{
    public partial class MedicoAddress
    {
        public string sfkEstado = "",
                      sfkCidade = "";
    }

    public partial class Medico
    {
        public object anexedEntity;

        public string   sfkEspecialidade,
                        updateCommand = "";

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
