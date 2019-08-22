using System.Web.Http;
using DataModel;

namespace DevKit.Web.Controllers
{
    public class VendaConfIsoInputDTO
    {
        public string st_nsu { get; set; }
    }

    public class VendaConfServerISOController : ApiControllerBase
    {
        [AllowAnonymous]
        [HttpPut]
        [Route("api/VendaConfServerISO")]
        public IHttpActionResult Venda(VendaConfIsoInputDTO mdl)
        {
            using (var db = new AutorizadorCNDB())
            {
                var v = new VendaEmpresarialConfirmacao();

                v.Run(db, mdl.st_nsu);

                return Ok();
            }
        }
    }
}
