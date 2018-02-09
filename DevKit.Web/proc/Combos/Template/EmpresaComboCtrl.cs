using DataModel;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class EmpresaComboController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var ret = new Empresa().ComboFilters(db, new EmpresaFilter
            {
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 0),
                busca = Request.GetQueryStringValue("busca")
            });

            return Ok(ret);
        }

        public IHttpActionResult Get(long id)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = (from e in db.Empresa where e.id == id
                       select new BaseComboResponse
                       {
                           id = e.id,
                           stName = e.nuEmpresa + " - " + e.stNome
                       }).
                       FirstOrDefault();

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            return Ok(mdl);
        }
    }
}
