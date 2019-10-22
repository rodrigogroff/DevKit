using System.Collections.Generic;

namespace Entities.Api.Associado
{
    public class LojaRede
    {
        public string nomeLoja { get; set; }

        public string end { get; set; }
    }

    public class AssociadoRede
    {
        public List<LojaRede> resultados = new List<LojaRede>();
    }
}
