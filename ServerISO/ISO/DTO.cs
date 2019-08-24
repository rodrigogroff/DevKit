
namespace ServerISO.DTO
{
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
