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
	public class EmpresaController : ApiControllerBase
	{
        public IHttpActionResult Get()
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var filter = new EmpresaFilter
            {
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
            };

            if (Request.GetQueryStringValue<bool?>("combo") == true)
            {
                var resp = new Empresa().ComposedFilters(db, filter);

                return Ok(new
                {
                    resp.count,
                    results = (from e in resp.results
                               select new BaseComboResponse
                               {
                                   id = e.id,
                                   stName = e.stNome
                               }).
                               ToList()
                });
            }
            else
                return Ok(new Empresa().ComposedFilters(db, filter));
        }
            
        public IHttpActionResult Get(long id)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.Empresa.Where(y => y.id == id).FirstOrDefault();

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);
            
            mdl.LoadAssociations(db);

            return Ok(mdl);
        }

        public IHttpActionResult Post(Empresa mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Create(db, ref apiError))
                return BadRequest(apiError);

            mdl.LoadAssociations(db);

            return Ok(mdl);
        }

        public IHttpActionResult Put(long id, Empresa mdl)
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
