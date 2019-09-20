using System;

namespace Entities.Database
{
    public partial class T_Parcelas
    {
        public decimal i_unique { get; set; } // numeric(15, 0)
        public int? nu_nsu { get; set; } // int
        public int? fk_empresa { get; set; } // int
        public int? fk_cartao { get; set; } // int
        public DateTime? dt_inclusao { get; set; } // datetime
        public int? nu_parcela { get; set; } // int
        public int? vr_valor { get; set; } // int
        public int? nu_indice { get; set; } // int
        public char? tg_pago { get; set; } // varchar(1)
        public int? fk_loja { get; set; } // int
        public int? nu_tot_parcelas { get; set; } // int
        public int? fk_terminal { get; set; } // int
        public int? fk_log_transacoes { get; set; } // int
    }
}
