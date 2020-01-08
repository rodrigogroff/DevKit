using LinqToDB;
using System.Linq;
using System.Web.Http;
using System.Net;
using DataModel;
using System;

namespace DevKit.Web.Controllers
{
    public class ParceiroDBAController : ApiControllerBase
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

            return Ok(new
            {
                count = query.Count(),
                results = query.Skip(skip).Take(take).ToList()
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

            mdl.sdtCadastro = ObtemData(mdl.dtCadastro);

            return Ok(mdl);
        }

        public IHttpActionResult Post(Parceiro mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            mdl.dtCadastro = DateTime.Now;

            if (!mdl.Create(db, ref apiError))
                return BadRequest(apiError);

            return Ok();
        }

        public IHttpActionResult Put(long id, Parceiro mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Update(db, ref apiError))
                return BadRequest(apiError);

            return Ok();
        }
    }
}
