
using System.Collections.Generic;

namespace Entities.Api.Lojista
{
    public class ReqSolicitacaoVendaCartao
    {
        public string empresa { get; set; }
        public string matricula { get; set; }
        public string codAcesso { get; set; }
        public string venc { get; set; }
    }

    public class ReqSolicitacaoVendaPOS
    {
        public string empresa { get; set; }
        public string matricula { get; set; }
        public string codAcesso { get; set; }
        public string venc { get; set; }
        public string valor { get; set; }
        public string parcelas { get; set; }
        public string senha { get; set; }
        public string parcelas_str { get; set; }
    }

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

    public class LojistaAutorizacaoDTO
    {
        public long id { get; set; }
        public string dt { get; set; }
        public string valor { get; set; }
        public string parcs { get; set; }
        public string cartao { get; set; }
        public string sit { get; set; }
    }

    public class LojistaAutorizacoesDTO
    {
        public List<LojistaAutorizacaoDTO> results { get; set; }
    }
}
