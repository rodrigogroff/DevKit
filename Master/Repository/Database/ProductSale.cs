using System;

namespace Master.Infra.Entity.Database
{
    public partial class ProductSale
    {
        public long id { get; set; }        
        public long fkCountry { get; set; }
        public long fkProduct { get; set; }
        public long fkCategory { get; set; }
        public long fkUser { get; set; }
        public long fkStore { get; set; }
        public long nuPrice { get; set; }
        public long nuYear { get; set; }
        public long nuMonth { get; set; }
        public long nuDay { get; set; }
        public DateTime? dtSale { get; set; }
    }
}
