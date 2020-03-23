namespace Entities.Api.Lojista
{
    public class ReqSolicitacaoVenda
    {
        public string empresa { get; set; }
        public string matricula { get; set; }
        public string codAcesso { get; set; }
        public string venc { get; set; }
        public string valor { get; set; }
        public string parcelas { get; set; }
        public string parcelas_str { get; set; }
    }
}
