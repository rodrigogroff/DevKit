
using System.Collections.Generic;

namespace Entities.Api.Associado
{
    public class AssociadoSolicitacao
    {
        public long id { get; set; }
        public string stLoja { get; set; }
        public string stSenha { get; set; }
        public string stCartao { get; set; }
        public string stValor { get; set; }
        public string nuParcelas { get; set; }
        public string dtSolic { get; set; }
        public List<string> lstParcelas { get; set; }
    }

    public class VendaIsoInputDTO
    {
        public string st_empresa { get; set; }
        public string st_matricula { get; set; }
        public string st_titularidade { get; set; }
        public string st_codLoja { get; set; }
        public string st_terminal { get; set; }
        public string nu_nsuOrig { get; set; }
        public string nu_parcelas { get; set; }
        public string vr_valor { get; set; }
        public string st_valores { get; set; }
        public string st_senha { get; set; }
    }

    public class VendaIsoOutputDTO
    {
        public string st_codResp { get; set; }
        public string st_msg { get; set; }
        public string st_nsuRcb { get; set; }
        public string st_via { get; set; }
        public string st_nomeCliente { get; set; }
    }

    public class VendaConfIsoInputDTO
    {
        public string st_nsu { get; set; }
    }

    public class VendaCancIsoInputDTO
    {
        public string st_nsu { get; set; }
    }

    public class VendaCancIsoOutputDTO
    {
        public string st_codResp { get; set; }
    }

    public class VendaDesfazIsoInputDTO
    {
        public string st_nsu { get; set; }
    }

    public class VendaDesfazIsoOutputDTO
    {
        public string st_codResp { get; set; }
    }
}
