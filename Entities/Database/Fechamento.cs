using System;

namespace Entities.Database
{
    public partial class LOG_Fechamento
    {
        public decimal i_unique { get; set; } // numeric(15, 0)
        public string st_mes { get; set; } // varchar(2)
        public string st_ano { get; set; } // varchar(4)
        public int? vr_valor { get; set; } // int
        public DateTime? dt_fechamento { get; set; } // datetime
        public int? fk_empresa { get; set; } // int
        public int? fk_loja { get; set; } // int
        public int? fk_cartao { get; set; } // int
        public int? fk_parcela { get; set; } // int
        public DateTime? dt_compra { get; set; } // datetime
        public int? nu_parcela { get; set; } // int
        public string st_cartao { get; set; } // varchar(14)
        public string st_afiliada { get; set; } // varchar(20)
    }
}
