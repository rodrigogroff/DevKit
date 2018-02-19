using DataModel;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class EmpresaSecaoComboController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            return Ok(new EmpresaSecao().ComboFilters(db, new BaseFilter
            {
                fkEmpresa = db.currentUser.fkEmpresa,
                busca = Request.GetQueryStringValue("busca", "").ToUpper()
            }));
        }

        public IHttpActionResult Get(long id)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = (from e in db.EmpresaSecao
                       where e.id == id
                       select new BaseComboResponse
                       {
                           id = e.id,
                           stName = e.nuEmpresa.ToString()
                       }).
                       FirstOrDefault();

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            return Ok(mdl);
        }
    }
}
