using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Database
{
    public partial class T_Cartao
    {
        public decimal i_unique { get; set; } // numeric(15, 0)
        public string st_empresa { get; set; } // varchar(6)
        public string st_matricula { get; set; } // varchar(6)
        public string st_titularidade { get; set; } // varchar(2)
        public string st_senha { get; set; } // varchar(16)
        public char? tg_tipoCartao { get; set; } // varchar(1)
        public string st_venctoCartao { get; set; } // varchar(4)
        public char? tg_status { get; set; } // varchar(1)
        public DateTime? dt_utlPagto { get; set; } // datetime
        public int? nu_senhaErrada { get; set; } // int
        public DateTime? dt_inclusao { get; set; } // datetime
        public DateTime? dt_bloqueio { get; set; } // datetime
        public char? tg_motivoBloqueio { get; set; } // varchar(1)
        public string st_banco { get; set; } // varchar(4)
        public string st_agencia { get; set; } // varchar(4)
        public string st_conta { get; set; } // varchar(10)
        public string st_matriculaExtra { get; set; } // varchar(10)
         public string st_celCartao { get; set; } // varchar(13)
        public int? fk_dadosProprietario { get; set; } // int
        public int? fk_infoAdicionais { get; set; } // int
        public int? nu_viaCartao { get; set; } // int
        public int? vr_limiteTotal { get; set; } // int
        public int? vr_limiteMensal { get; set; } // int
        public int? vr_limiteRotativo { get; set; } // int
        public int? vr_extraCota { get; set; } // int
        public int? vr_educacional { get; set; } // int
        public int? vr_disp_educacional { get; set; } // int
        public int? vr_edu_diario { get; set; } // int
        public string st_aluno { get; set; } // varchar(40)
        public int? tg_emitido { get; set; } // int
        public int? vr_edu_disp_virtual { get; set; } // int
        public int? nu_rankVirtual { get; set; } // int
        public int? vr_edu_total_ranking { get; set; } // int
        public int? nu_webSenhaErrada { get; set; } // int
        public int? tg_semanaCompleta { get; set; } // int
        public int? tg_excluido { get; set; } // int
        public bool? tg_convenioComSaldo { get; set; }
        public long? vr_saldoConvenio { get; set; }
    }
}
