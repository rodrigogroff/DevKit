using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class LojistaController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var terminal = Request.GetQueryStringValue("terminal");
            var senha = Request.GetQueryStringValue("senha");

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var lojista = (from e in db.T_Loja
                           where e.st_loja == terminal &&
                                 e.st_senha == senha
                           select e).
                           FirstOrDefault();

            if (lojista == null)
                return BadRequest();

            return Ok(new
            {
                count = 0,
                results = new List<Lojista>
                {
                    new Lojista
                    {
                       nome = lojista.st_nome,
                       endereco = lojista.st_endereco.Replace ("{$SE3}","")
                    }
                }
            });
        }
    }
}
