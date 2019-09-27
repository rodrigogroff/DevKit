using System.Collections.Generic;

namespace Entities.Api.Associado
{
    public class AssociadoExtratoVenda
    {
        public string dtVenda { get; set; }
        public string nsu { get; set; }
        public string valor { get; set; }
        public string parcela { get; set; }
        public string estab { get; set; }
    }

    public class AssociadoExtratoAtual
    {
        public string mesAtual { get; set; }
        public string totalExtrato { get; set; }
        public string saldoDisponivel { get; set; }

        public List<AssociadoExtratoVenda> vendas = new List<AssociadoExtratoVenda>();
    }
}
