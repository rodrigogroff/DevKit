
namespace Entities.Api.User
{
    public class DtoLoginInformation : DtoBase
    {
        public long? empresa { get; set; }
        public long? matricula { get; set; }
        public string codAcesso { get; set; }
        public string vencimento { get; set; }
        public string senha { get; set; }
        public string userType { get; set; }
    }
}
