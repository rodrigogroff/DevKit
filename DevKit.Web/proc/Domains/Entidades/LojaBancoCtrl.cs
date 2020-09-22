using LinqToDB;
using System.Linq;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class LojaBancoUpd
    {
        public int fk_banco = 0;

        public string   codigo = "", 
                        st_agencia = "", 
                        st_conta = "";
    }

    public class LojaBancoController : ApiControllerBase
    {                
        [HttpPut]
        public IHttpActionResult Put(LojaBancoUpd mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var upd = db.T_Loja.FirstOrDefault(y => y.st_loja == mdl.codigo);

            if (upd != null)
            {
                if (mdl.fk_banco > 0 && !string.IsNullOrEmpty(mdl.st_agencia) && !string.IsNullOrEmpty(mdl.st_conta))
                {
                    upd.fk_banco = mdl.fk_banco;
                    upd.st_agencia = mdl.st_agencia;
                    upd.st_conta = mdl.st_conta;

                    db.Update(upd);
                }
            }

            return Ok();
        }
    }
}
