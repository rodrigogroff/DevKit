using System;

namespace Master.Infra.Entity.Database
{
    public partial class CategoryPromotion
    {
        public long id { get; set; }
        public long fkCategory { get; set; }        
        public string stName { get; set; }
        public long nuPctDesc { get; set; }
        public DateTime? dtCreation { get; set; }
        public DateTime? dtExpires { get; set; }
        public Boolean? bPaused { get; set; }
    }
}
