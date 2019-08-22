using System.Web.Http;
using DataModel;

namespace DevKit.Web.Controllers
{
    public class VendaDesfazIsoInputDTO
    {
        public string st_nsu { get; set; }
    }

    public class VendaDesfazIsoOutputDTO
    {
        public string st_codResp { get; set; }
    }

    public class VendaDesfazServerISOController : ApiControllerBase
    {
        [AllowAnonymous]
        [HttpPut]
        [Route("api/VendaDesfazServerISO")]
        public IHttpActionResult Venda(VendaDesfazIsoInputDTO mdl)
        {
            using (var db = new AutorizadorCNDB())
            {
                var v = new VendaEmpresarialDesfazimento();

                v.Run(db, mdl.st_nsu);

                return Ok(new VendaDesfazIsoOutputDTO
                {
                    st_codResp = v.var_codResp
                });
            }
        }
    }
}
