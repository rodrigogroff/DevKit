using System.Web.Http;
using DataModel;

namespace DevKit.Web.Controllers
{
    public class VendaCancIsoInputDTO
    {
        public string st_nsu { get; set; }
    }

    public class VendaCancIsoOutputDTO
    {
        public string st_codResp { get; set; }
    }

    public class VendaCancServerISOController : ApiControllerBase
    {
        [AllowAnonymous]
        [HttpPut]
        [Route("api/VendaCancServerISO")]
        public IHttpActionResult Venda(VendaCancIsoInputDTO mdl)
        {
            using (var db = new AutorizadorCNDB())
            {
                var v = new VendaEmpresarialCancelamento();

                v.Run(db, mdl.st_nsu);

                return Ok(new VendaCancIsoOutputDTO
                {
                    st_codResp = v.var_codResp
                });
            }
        }
    }
}
