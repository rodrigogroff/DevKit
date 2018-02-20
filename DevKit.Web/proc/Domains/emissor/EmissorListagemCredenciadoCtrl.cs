using DataModel;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class EmissorListagemCredenciadoController : ApiControllerBase
	{
        public IHttpActionResult Get()
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            return Ok(new Credenciado().ComposedFilters(db, new CredenciadoFilter
            {
                fkEmpresa = db.currentUser.fkEmpresa,
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
                nome = Request.GetQueryStringValue("nome"),
                nuCodigo = Request.GetQueryStringValue<long?>("codigo", null),
                especialidade = Request.GetQueryStringValue("especialidade"),
            }));                        
        }
            
        public IHttpActionResult Get(long id)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.Credenciado.Where(y => y.id == id).FirstOrDefault();

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);
            
            mdl.LoadAssociations(db);

            return Ok(mdl);
        }
        
        public IHttpActionResult Put(long id, Credenciado mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Update(db, ref apiError))
                return BadRequest(apiError);

            mdl.LoadAssociations(db);

            return Ok();			
		}        
	}
}
