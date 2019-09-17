using System;

namespace Entities.Database
{
    public partial class T_Proprietario
    {
        public decimal i_unique { get; set; } // numeric(15, 0)
        public string st_cpf { get; set; } // varchar(20)
        public string st_nome { get; set; } // varchar(99)
        public string st_endereco { get; set; } // varchar(900)
        public string st_numero { get; set; } // varchar(29)
        public string st_complemento { get; set; } // varchar(29)
        public string st_bairro { get; set; } // varchar(99)
        public string st_cidade { get; set; } // varchar(99)
        public string st_UF { get; set; } // varchar(2)
        public string st_cep { get; set; } // varchar(20)
        public string st_ddd { get; set; } // varchar(3)
        public string st_telefone { get; set; } // varchar(20)
        public DateTime? dt_nasc { get; set; } // datetime
        public string st_email { get; set; } // varchar(199)
        public int? vr_renda { get; set; } // int
        public string st_senhaEdu { get; set; } // varchar(16)
    }
}
