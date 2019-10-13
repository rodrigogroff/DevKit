
namespace Entities.Database
{    
    public partial class T_Terminal
    {
        public decimal i_unique { get; set; } // numeric(15, 0)
        public string nu_terminal { get; set; } // varchar(12)
        public int? fk_loja { get; set; } // int
        public string st_localizacao { get; set; } // varchar(250)
    }
}
