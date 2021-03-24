
namespace Master.Data.Database
{
    public class Terminal
    {
        public long id { get; set; }
        public long? fkLoja { get; set; }
        public string stTerminal { get; set; }
        public string stLocal { get; set; }
    }
}
