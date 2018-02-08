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
	public class MedicoController : ApiControllerBase
	{
        public IHttpActionResult Get()
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var filter = new MedicoFilter
            {
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
                nome = Request.GetQueryStringValue("nome"),
                nuCodigo = Request.GetQueryStringValue("codigo",0),
                especialidade = Request.GetQueryStringValue("especialidade"),
            };

            return Ok(new Medico().ComposedFilters(db, filter));                        
        }
            
        public IHttpActionResult Get(long id)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.Medico.Where(y => y.id == id).FirstOrDefault();

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);
            
            mdl.LoadAssociations(db);

            return Ok(mdl);
        }

        public IHttpActionResult Post(Medico mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Create(db, ref apiError))
                return BadRequest(apiError);

            mdl.LoadAssociations(db);

            return Ok(mdl);
        }

        public IHttpActionResult Put(long id, Medico mdl)
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
