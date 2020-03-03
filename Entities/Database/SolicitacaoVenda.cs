using System;

namespace Entities.Database
{
    public partial class SolicitacaoVenda
    {
        public long id { get; set; }
        public long? fkCartao { get; set; }
        public long? fkLoja { get; set; }
        public long? fkTerminal { get; set; }
        public long? fkLogTrans { get; set; }
        public long? vrValor { get; set; }
        public int? nuParcelas { get; set; }
        public bool? tgAberto { get; set; }
        public DateTime? dtSolic { get; set; }
        public DateTime? dtConf { get; set; }

        public string stErro { get; set; }
    }
}
