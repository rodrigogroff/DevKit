using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using System.Net;

namespace DevKit.Web.Controllers
{
    public class TerminalLojaItem
    {
        public string id, stName;
    }

    public class TerminalLojaController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var busca = Request.GetQueryStringValue("busca");
            var skip = Request.GetQueryStringValue<int>("skip");
            var take = Request.GetQueryStringValue<int>("take");

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var query = (from e in db.T_Terminal
                         where e.fk_loja == db.currentLojista.i_unique
                         select e);

            query = query.OrderBy(y => y.nu_terminal);

            var lst = new List<TerminalLojaItem>();

            foreach (var item in query.Skip(skip).Take(take).ToList())
            {                
                lst.Add(new TerminalLojaItem
                {
                    id = item.i_unique.ToString(),
                    stName = item.nu_terminal
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

            var mdl = (from e in db.T_Terminal
                       where e.i_unique == id
                       where e.fk_loja == db.currentLojista.i_unique 
                       select e).
                       FirstOrDefault();

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            return Ok(new TerminalLojaItem
            {
                id = mdl.i_unique.ToString(),
                stName = mdl.nu_terminal
            });
        }
    }
}
