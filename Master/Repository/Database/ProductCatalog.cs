using System;

namespace Master.Infra.Entity.Database
{
    public partial class ProductCatalogLink
    {
        public long id { get; set; }        
        public long? fkProduct { get; set; }
        public long? fkProductCatalog { get; set; }
    }
}
