using DataModel;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class AutorizaProcController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var resp = new Credenciado().AutorizaProcedimento(db, new AutorizaProcedimentoParams
            {
                emp = Request.GetQueryStringValue("emp", 0),
                mat = Request.GetQueryStringValue("mat", 0),
                ca = Request.GetQueryStringValue("ca"),
                senha = Request.GetQueryStringValue("senha"),
                tuss = Request.GetQueryStringValue<long>("tuss", 0),
                titVia = Request.GetQueryStringValue("titVia")?.PadLeft(4, '0'),
            });

            if (resp == "")
                return Ok();
            else
                return BadRequest(resp);
        }
    }
}
