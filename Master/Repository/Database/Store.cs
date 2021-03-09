using System;

namespace Master.Infra.Entity.Database
{
    public partial class Store
    {
        public long id { get; set; }
        public string stName { get; set; }
        public string stLocalization { get; set; }
        public long fkCountry { get; set; }
    }
}
