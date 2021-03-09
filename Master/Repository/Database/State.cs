
namespace Master.Infra.Entity.Database
{
    public partial class State
    {
        public long id { get; set; }
        public string stName { get; set; }
        public long fkCountry { get; set; }
    }
}
