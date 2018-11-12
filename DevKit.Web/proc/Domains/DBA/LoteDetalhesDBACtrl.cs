using LinqToDB;
using System.Linq;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class LoteDetalhesDBAController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var idLote = Request.GetQueryStringValue<int>("idLote");
            
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var query = (from e in db.T_LoteCartaoDetalhe
                         where e.fk_lote == idLote
                         select e);

            return Ok(new
            {
                count = query.Count(),
                results = query.ToList()
            });
        }
    }
}
