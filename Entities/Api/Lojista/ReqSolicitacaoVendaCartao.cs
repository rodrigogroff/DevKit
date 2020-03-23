namespace Entities.Api.Lojista
{
    public class ReqSolicitacaoVendaCartao
    {
        public string empresa { get; set; }
        public string matricula { get; set; }
        public string codAcesso { get; set; }
        public string venc { get; set; }
    }
}
