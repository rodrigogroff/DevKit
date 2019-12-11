using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using System.Net;

namespace DevKit.Web.Controllers
{
    public class ParceiroController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var busca = Request.GetQueryStringValue("busca");
            var skip = Request.GetQueryStringValue<int>("skip");
            var take = Request.GetQueryStringValue<int>("take");

            if (take == 0)
                take = 50;

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var query = (from e in db.Parceiro select e);

            if (!string.IsNullOrEmpty(busca))
                query = query.Where(y => y.stNome.Contains(busca));
            
            query = query.OrderBy(y => y.stNome);

            var lst = new List<EmpresaItem>();

            foreach (var item in query.Skip(skip).Take(take).ToList())
            {                
                lst.Add(new EmpresaItem
                {
                    id = item.id.ToString(),
                    stName = item.stNome
                });
            }

            return Ok(new
            {
                count = query.Count(),
                results = lst
            });
        }

        public IHttpActionResult Get(long id)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = (from e in db.Parceiro
                       where e.id == id
                       select e).
                       FirstOrDefault();

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            return Ok(new EmpresaItem
            {
                id = mdl.id.ToString(),
                stName = mdl.stNome
            });
        }
    }
}
