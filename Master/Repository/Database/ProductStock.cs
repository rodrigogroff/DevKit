using System;

namespace Master.Infra.Entity.Database
{
    public partial class ProductStock
    {
        public long id { get; set; }
        public long fkCountry { get; set; }
        public long fkProduct { get; set; }
        public long fkStore { get; set; }
    }
}
