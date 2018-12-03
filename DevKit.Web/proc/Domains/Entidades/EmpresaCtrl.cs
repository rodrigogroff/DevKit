using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using System.Net;

namespace DevKit.Web.Controllers
{
    public class EmpresaItem
    {
        public string id, stName;
    }

    public class EmpresaController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var busca = Request.GetQueryStringValue("busca");
            var skip = Request.GetQueryStringValue<int>("skip");
            var take = Request.GetQueryStringValue<int>("take");

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var query = (from e in db.T_Empresa select e);

            if (!string.IsNullOrEmpty(busca))
                query = query.Where(y => y.st_empresa.Contains(busca));
            
            query = query.OrderBy(y => y.st_fantasia);

            var lst = new List<EmpresaItem>();

            foreach (var item in query.Skip(skip).Take(take).ToList())
            {                
                lst.Add(new EmpresaItem
                {
                    id = item.i_unique.ToString(),
                    stName = "(" + item.st_empresa + ") " + item.st_fantasia
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

            var mdl = (from e in db.T_Empresa
                       where e.i_unique == id
                       select e).
                       FirstOrDefault();

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            return Ok(new EmpresaItem
            {
                id = mdl.i_unique.ToString(),
                stName = "(" + mdl.st_empresa + ") " + mdl.st_fantasia
            });
        }
    }
}
