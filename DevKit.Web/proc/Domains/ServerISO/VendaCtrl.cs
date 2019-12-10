using System.Web.Http;
using DataModel;

namespace DevKit.Web.Controllers
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

    public class VendaServerISOController : ApiControllerBase
    {
        [AllowAnonymous]
        [HttpPut]
        [Route("api/VendaServerISO")]
        public IHttpActionResult Venda(VendaIsoInputDTO mdl)
        {
            using (var db = new AutorizadorCNDB())
            {
                var v = new VendaEmpresarial { IsSitef = true };

                v.input_cont_pe.st_empresa = mdl.st_empresa;
                v.input_cont_pe.st_matricula = mdl.st_matricula;
                v.input_cont_pe.st_titularidade = mdl.st_titularidade;
                v.input_cont_pe.st_codLoja = mdl.st_codLoja;
                v.input_cont_pe.st_terminal = mdl.st_terminal;
                v.var_nu_nsuOrig = mdl.nu_nsuOrig;                
                v.input_cont_pe.nu_parcelas = mdl.nu_parcelas;
                v.input_cont_pe.vr_valor = mdl.vr_valor;
                v.input_cont_pe.st_valores = mdl.st_valores;
                v.input_cont_pe.st_senha = mdl.st_senha;

                v.Run(db);

                return Ok(new VendaIsoOutputDTO
                {
                    st_codResp = v.var_codResp.Substring(2, 2),
                    st_msg = v.output_st_msg,
                    st_nsuRcb = v.output_cont_pr.st_nsuRcb,
                    st_via = v.output_cont_pr.st_via,
                    st_nomeCliente = v.output_cont_pr.st_nomeCliente
                });
            }
        }

        [AllowAnonymous]
        [HttpPut]
        [Route("api/VendaServerWeb")]
        public IHttpActionResult VendaWeb(VendaIsoInputDTO mdl)
        {
            using (var db = new AutorizadorCNDB())
            {
                var v = new VendaEmpresarial { IsSitef = false };

                v.input_cont_pe.st_empresa = mdl.st_empresa;
                v.input_cont_pe.st_matricula = mdl.st_matricula;
                v.input_cont_pe.st_titularidade = mdl.st_titularidade;
                v.input_cont_pe.st_codLoja = mdl.st_codLoja;
                v.input_cont_pe.st_terminal = mdl.st_terminal;                
                v.input_cont_pe.nu_parcelas = mdl.nu_parcelas;
                v.input_cont_pe.vr_valor = mdl.vr_valor;
                v.input_cont_pe.st_valores = mdl.st_valores;
                v.input_cont_pe.st_senha = mdl.st_senha;

                v.Run(db);

                return Ok(new VendaIsoOutputDTO
                {
                    st_codResp = v.var_codResp.Substring(2, 2),
                    st_msg = v.output_st_msg,
                    st_nsuRcb = v.output_cont_pr.st_nsuRcb,
                    st_via = v.output_cont_pr.st_via,
                    st_nomeCliente = v.output_cont_pr.st_nomeCliente
                });
            }
        }
    }
}
