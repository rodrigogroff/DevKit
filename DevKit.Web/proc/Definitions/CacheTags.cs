using System;

namespace DevKit.Web.Controllers
{
    public class CacheHitRecord
    {
        public DateTime dt_last;
        public int hits = 0;
    }

    public static class CacheTags
    {
        // ------------------------
        // enums
        // ------------------------

        public const string EnumMonth = "EnumMonth";
        public const string EnumTipoVenda = "EnumTipoVenda";
        public const string EnumOrdemRelLojistaTrans = "EnumOrdemRelLojistaTrans";

        public const string EnumMonthReport = "EnumMonthReport";
        public const string EnumTipoVendaReport = "EnumTipoVendaReport";
        public const string EnumOrdemRelLojistaTransReport = "EnumOrdemRelLojistaTransReport";        
        
        // ------------------------
        // combos
        // ------------------------

        //public const string UserCombo = "UserCombo";

        // ------------------------
        // tables
        // ------------------------

        public const string T_Terminal = "T_Loja";
        public const string associado = "associado";
    }
}
