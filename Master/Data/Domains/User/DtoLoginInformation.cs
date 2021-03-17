
namespace Master.Data.Domains.User
{
    public class DtoLoginInformation : DtoBase
    {
        public string empresa { get; set; }
        public string matricula { get; set; }
        public string codAcesso { get; set; }
        public string venc { get; set; }
        public string email { get; set; }
        public string login { get; set; }
        public string senha { get; set; }
        public string userType { get; set; }
    }
}
