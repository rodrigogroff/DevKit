
namespace Master.Infra.Entity.Database
{
    public partial class Product
    {
        public long id { get; set; }
        public string stName { get; set; }
        public long fkCountry { get; set; }
        public long fkBrand { get; set; }
        public long fkCategory { get; set; }
        public long nuPrice { get; set; }
        public string stDesc { get; set; }
    }
}
