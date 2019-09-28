using System.Collections.Generic;

namespace Entities.Api.Associado
{
    public class AssociadoExtratoAtual
    {
        public string mesAtual { get; set; }
        public string totalExtrato { get; set; }
        public string saldoDisponivel { get; set; }

        public List<AssociadoExtratoVenda> vendas = new List<AssociadoExtratoVenda>();
    }

    public class AssociadoExtratoVenda
    {
        public string dtVenda { get; set; }
        public string nsu { get; set; }
        public string valor { get; set; }
        public string parcela { get; set; }
        public string estab { get; set; }
    }

    public class AssociadoExtratoFuturo
    {
        public List<AssociadoExtratoFuturoSintetico> parcelamento = new List<AssociadoExtratoFuturoSintetico>();
    }

    public class AssociadoExtratoFuturoSintetico
    {
        public string mesAno { get; set; }
        public string valor { get; set; }
        public string pctComprometido { get; set; }
        public string vrDisponivel { get; set; }
    }

    public class AssociadoExtratoFechada
    {
        public List<AssociadoExtratoFechadoSintetico> faturas = new List<AssociadoExtratoFechadoSintetico>();
    }
    
    public class AssociadoExtratoFechadoSintetico
    {
        public string mesAno { get; set; }
        public string valorTotal { get; set; }

        public List<AssociadoExtratoVenda> vendas = new List<AssociadoExtratoVenda>();
    }
}
