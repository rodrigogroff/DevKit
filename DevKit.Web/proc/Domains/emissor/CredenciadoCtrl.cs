using DataModel;
using LinqToDB;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class CredenciadoController : ApiControllerBase
	{
        public IHttpActionResult Get()
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            return Ok(new Credenciado().ComposedFilters(db, new CredenciadoFilter
            {
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
                nome = Request.GetQueryStringValue("nome"),
                nuCodigo = Request.GetQueryStringValue("codigo", 0),
                especialidade = Request.GetQueryStringValue("especialidade"),
            }));
        }
            
        public IHttpActionResult Get(long id)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.Credenciado.Where(y => y.id == id).FirstOrDefault();

            if (db.currentUser != null)
                if (!db.CredenciadoEmpresa.Any(y => y.fkCredenciado == mdl.id &&
                                                    y.fkEmpresa == db.currentUser.fkEmpresa))
                {
                    return BadRequest("Credenciado inválido");
                }
                
            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);
            
            mdl.LoadAssociations(db);

            return Ok(mdl);
        }

        public IHttpActionResult Post(Credenciado mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Create(db, ref apiError))
                return BadRequest(apiError);

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
