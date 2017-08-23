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
        public const string EnumMonthReport = "EnumMonthReport";

        // ------------------------
        // combos
        // ------------------------

        //public const string UserCombo = "UserCombo";

        // ------------------------
        // tables
        // ------------------------

        public const string Lojista = "Lojista";

        //public const string User = "User";
    }
}
