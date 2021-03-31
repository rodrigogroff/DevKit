using System.ComponentModel.DataAnnotations.Schema;

namespace DataModel
{
    public partial class T_Empresa
    {
        public bool _tg_bloq;
        public bool _tg_isentoFat;

        [NotMapped]
        public string updateCommand { get; set; }

        [NotMapped]
        public EmpresaDespesa anexedDespesa { get; set; }
        public EmpresaDespesa anexedDespesaRec { get; set; }
    }
}
