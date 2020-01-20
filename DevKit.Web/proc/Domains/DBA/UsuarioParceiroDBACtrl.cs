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
            var fkParceiro = Request.GetQueryStringValue<int?>("fkParceiro");

            if (take == 0)
                take = 50;

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var query =  (  from e in db.UsuarioParceiro
                            where e.fkParceiro == fkParceiro || fkParceiro == null
                            select e);

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

            if (this.userLoggedParceiroId != "1")
                mdl.stSenha = "";

            mdl._dtCadastro = ObtemData(mdl.dtCadastro);

            return Ok(mdl);
        }

        public IHttpActionResult Post(UsuarioParceiro mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            mdl.stSenha = GetRandomString(6);

            SendEmail("ConveyNET - Criação de usuário", 
                      "Bem-vindo ao Sistema ConveyNET.\n\nSua senha inicial: " + mdl.stSenha + "\n\n" + DateTime.Now.ToString(), 
                      mdl.stEmail);

            if (!mdl.Create(db, ref apiError))
                return BadRequest(apiError);

            return Ok();
        }

        [NonAction]
        private string GetRandomString(int length)
        {
            var rand = new Random();
            var ret = "";

            for (int i = 0; i < length; i++)
                ret += rand.Next(0, 9);

            return ret;
        }

        public IHttpActionResult Put(long id, UsuarioParceiro mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (mdl.updateCommand == "trocaSenha")
            {
                var m = (from e in db.UsuarioParceiro
                         where e.id == this.userIdLoggedUsuario
                         select e).
                        FirstOrDefault();

                m.stSenha = mdl._novaSenha;

                db.Update(m);
            }
            else
            {
                if (this.userLoggedParceiroId != "1")
                    mdl.stSenha = db.UsuarioParceiro.FirstOrDefault(y => y.id == mdl.id).stSenha;

                if (!mdl.Update(db, ref apiError))
                    return BadRequest(apiError);
            }
            

            return Ok();
        }
    }
}
