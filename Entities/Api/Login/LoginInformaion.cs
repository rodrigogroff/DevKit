
namespace Entities.Api.Login
{
    public class AssociadoLoginInformation
    {
        public string empresa { get; set; }
        public string matricula { get; set; }
        public string codAcesso { get; set; }
        public string venc { get; set; }
        public string senha { get; set; }
    }

    public class LojistaLoginInformation
    {
        public string terminal { get; set; }
        public string senha { get; set; }
    }
}
