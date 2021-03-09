
namespace Master.Infra.Entity.Database
{
    public partial class Category
    {
        public long id { get; set; }
        public string stName { get; set; }
        public long fkCountry { get; set; }
        public long fkMainCategory { get; set; }
    }
}
