using DataModel;
using LinqToDB;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class LotesGraficaController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var novoLote = Request.GetQueryStringValue<bool?>("novoLote", null);

            if (novoLote == null)
            {
                var filter = new LoteGraficaFilter
                {
                    skip = Request.GetQueryStringValue("skip", 0),
                    take = Request.GetQueryStringValue("take", 15),
                    nuCodigo = Request.GetQueryStringValue<long?>("codigo", null),
                };

                return Ok(new LoteGrafica().ComposedFilters(db, filter));
            }
            else 
            {
                // pesquisa para novo lote
                return Ok(new LoteGrafica().NovoLoteQuery(db));
            }            
        }

        public IHttpActionResult Get(long id)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.LoteGrafica.Where(y => y.id == id).FirstOrDefault();

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);
            
            mdl.LoadAssociations(db);

            return Ok(mdl);
        }

		public IHttpActionResult Post(LoteGrafica mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Create(db, ref apiError))
				return BadRequest(apiError);

            mdl.LoadAssociations(db);

            return Ok();
		}

		public IHttpActionResult Put(long id, LoteGrafica mdl)
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
