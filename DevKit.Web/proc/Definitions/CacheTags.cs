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

        public const string EnumSituacoesReport = "EnumSituacoesReport";
        public const string EnumExpedicoesReport = "EnumExpedicoesReport";

        // ------------------------
        // combos
        // ------------------------

        //public const string UserCombo = "UserCombo";

        // ------------------------
        // tables
        // ------------------------

        public const string T_Terminal = "T_Loja";
        public const string T_Loja = "T_Loja";
        public const string T_Cartao = "T_Cartao";
        public const string T_Empresa = "T_Empresa";
        public const string I_Scheduler = "I_Scheduler";
        public const string associado = "associado";        
    }
}
