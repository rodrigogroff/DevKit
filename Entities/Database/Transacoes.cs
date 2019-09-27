using System;

namespace Entities.Database
{
    public partial class LOG_Transacoes
    {
        public decimal i_unique { get; set; } // numeric(15, 0)
        public int? fk_terminal { get; set; } // int
        public DateTime? dt_transacao { get; set; } // datetime
        public int? nu_nsu { get; set; } // int
        public int? fk_empresa { get; set; } // int
        public int? fk_cartao { get; set; } // int
        public int? vr_total { get; set; } // int
        public int? nu_parcelas { get; set; } // int
        public int? nu_cod_erro { get; set; } // int
        public char? tg_confirmada { get; set; } // varchar(1)
        public int? nu_nsuOrig { get; set; } // int
        public string en_operacao { get; set; } // varchar(10)
        public string st_msg_transacao { get; set; } // varchar(50)
        public char? tg_contabil { get; set; } // varchar(1)
        public int? fk_loja { get; set; } // int
        public int? vr_saldo_disp { get; set; } // int
        public int? vr_saldo_disp_tot { get; set; } // int
        public string st_doc { get; set; } // varchar(20)
    }
}
