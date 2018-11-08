using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using System.Net;
using DataModel;
using System;

namespace DevKit.Web.Controllers
{
    public class LoteDBAController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var busca = Request.GetQueryStringValue("busca");
            var skip = Request.GetQueryStringValue<int>("skip");
            var take = Request.GetQueryStringValue<int>("take");

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var query = (from e in db.T_LoteCartao select e);

            query = query.OrderByDescending(y => y.dt_abertura);

            var lst = new List<T_LoteCartao>();

            foreach (var item in query.Skip(skip).Take(take).ToList())
                lst.Add(item);

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

            var mdl = (from e in db.T_LoteCartao where e.i_unique == id select e).FirstOrDefault();

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            return Ok(mdl);
        }

        [HttpPut]
        public IHttpActionResult Put(T_LoteCartao mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            db.Update(mdl);

            return Ok();
        }

        [HttpPost]
        public IHttpActionResult Post(T_LoteCartao mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            mdl.i_unique = Convert.ToInt64(db.InsertWithIdentity(mdl));

            return Ok(mdl);
        }
    }
}
