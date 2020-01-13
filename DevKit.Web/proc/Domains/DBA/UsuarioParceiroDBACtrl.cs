using LinqToDB;
using System.Linq;
using System.Web.Http;
using System.Net;
using DataModel;
using System;

namespace DevKit.Web.Controllers
{
    public class UsuarioParceiroDBAController : ApiControllerBase
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

            var query = (from e in db.UsuarioParceiro select e);

            if (!string.IsNullOrEmpty(busca))
                query = query.Where(y => y.stNome.Contains(busca));

            query = query.OrderBy(y => y.stNome);

            var lst = query.Skip(skip).Take(take).ToList();

            foreach (var item in lst)
            {
                item._stParceiro = db.Parceiro.FirstOrDefault(y => y.id == item.fkParceiro)?.stNome;
                item._sbAtivo = item.bAtivo == true ? "Sim" : "Não";
                item._dtCadastro = ObtemData(item.dtCadastro);
                item._dtLastLogin = ObtemData(item.dtLastLogin);
                item._tipo = item.nuTipo == 1 ? "Admin" : "Operador";
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

            var mdl = (from e in db.UsuarioParceiro
                       where e.id == id
                       select e).
                       FirstOrDefault();

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            mdl._dtCadastro = ObtemData(mdl.dtCadastro);

            return Ok(mdl);
        }

        public IHttpActionResult Post(UsuarioParceiro mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Create(db, ref apiError))
                return BadRequest(apiError);

            return Ok();
        }

        public IHttpActionResult Put(long id, UsuarioParceiro mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Update(db, ref apiError))
                return BadRequest(apiError);

            return Ok();
        }
    }
}
