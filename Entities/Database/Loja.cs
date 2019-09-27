using System;

namespace Entities.Database
{
    public partial class T_Loja
    {
        public decimal i_unique { get; set; } // numeric(15, 0)
        public string nu_CNPJ { get; set; } // varchar(14)
        public string st_nome { get; set; } // varchar(99)
        public string st_social { get; set; } // varchar(99)
        public string st_endereco { get; set; } // varchar(199)
        public string st_enderecoInst { get; set; } // varchar(199)
        public string nu_inscEst { get; set; } // varchar(20)
        public string st_cidade { get; set; } // varchar(99)
        public string st_estado { get; set; } // varchar(2)
        public string nu_CEP { get; set; } // varchar(18)
        public string nu_telefone { get; set; } // varchar(20)
        public string nu_fax { get; set; } // varchar(20)
        public string st_contato { get; set; } // varchar(40)
        public int? vr_mensalidade { get; set; } // int
        public string nu_contaDeb { get; set; } // varchar(20)
        public string st_obs { get; set; } // varchar(900)
        public string st_loja { get; set; } // varchar(40)
        public char? tg_blocked { get; set; } // varchar(1)
        public int? nu_pctValor { get; set; } // int
        public int? vr_transacao { get; set; } // int
        public int? vr_minimo { get; set; } // int
        public int? nu_franquia { get; set; } // int
        public int? nu_periodoFat { get; set; } // int
        public int? nu_diavenc { get; set; } // int
        public char? tg_tipoCobranca { get; set; } // varchar(1)
        public int? nu_bancoFat { get; set; } // int
        public int? tg_isentoFat { get; set; } // int
        public string st_senha { get; set; } // varchar(16)
        public int? tg_cancel { get; set; } // int
        public int? tg_portalComSenha { get; set; } // int
        public string st_email { get; set; } // varchar(200)
        public string st_telCelular { get; set; } // varchar(20)
    }
}
