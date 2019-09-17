using System;

namespace Entities.Database
{
    public partial class T_Empresa
    {
        public decimal i_unique { get; set; } // numeric(15, 0)
        public string st_empresa { get; set; } // varchar(6)
        public string nu_CNPJ { get; set; } // varchar(14)
        public string st_fantasia { get; set; } // varchar(99)
        public string st_social { get; set; } // varchar(99)
        public string st_endereco { get; set; } // varchar(99)
        public string st_cidade { get; set; } // varchar(99)
        public string st_estado { get; set; } // varchar(2)
        public string nu_CEP { get; set; } // varchar(14)
        public string nu_telefone { get; set; } // varchar(20)
        public int? nu_cartoes { get; set; } // int
        public int? nu_parcelas { get; set; } // int
        public char? tg_blocked { get; set; } // varchar(1)
        public int? fk_admin { get; set; } // int
        public string nu_contaDeb { get; set; } // varchar(20)
        public int? vr_mensalidade { get; set; } // int
        public int? nu_pctValor { get; set; } // int
        public int? vr_transacao { get; set; } // int
        public int? vr_minimo { get; set; } // int
        public int? nu_franquia { get; set; } // int
        public int? nu_periodoFat { get; set; } // int
        public int? nu_diaVenc { get; set; } // int
        public char? tg_tipoCobranca { get; set; } // varchar(1)
        public int? nu_bancoFat { get; set; } // int
        public int? vr_cartaoAtivo { get; set; } // int
        public int? tg_isentoFat { get; set; } // int
        public string st_obs { get; set; } // varchar(400)
        public int? tg_bloq { get; set; } // int
        public string st_homepage { get; set; } // varchar(500)
        public int? nu_diaFech { get; set; } // int
        public string st_horaFech { get; set; } // varchar(500)
        public bool? tg_convenioComSaldo { get; set; } // varchar(500)
    }

    public class User
    {

    }
}
