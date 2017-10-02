using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class RelAssociadosItem
    {
        public string associado, cartao, cpf;
    }

    public class RelAssociadosController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var busca = Request.GetQueryStringValue("busca");
            var skip = Request.GetQueryStringValue<int>("skip");
            var take = Request.GetQueryStringValue<int>("take");
            var idEmpresa = Request.GetQueryStringValue<int?>("idEmpresa", null);
            var bloqueado = Request.GetQueryStringValue<bool?>("bloqueado");

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var stEmpresa = "";

            if (idEmpresa != null)
            {
                stEmpresa = (from e in db.T_Empresa
                             where (int)e.i_unique == idEmpresa
                             select e).
                             FirstOrDefault().
                             st_empresa;
            }

            var query = (from e in db.T_Cartao select e);

            if (bloqueado != null)
            {
                if (bloqueado == false)
                    query = (from e in query
                             where e.tg_status.ToString() == "0"
                             select e);
                else
                    query = (from e in query
                             where e.tg_status.ToString() == "1"
                             select e);
            }

            if (busca != null)
            {
                query = (from e in query
                         join associado in db.T_Proprietario on e.fk_dadosProprietario equals (int) associado.i_unique
                         where associado.st_nome.Contains(busca)
                         select e);
            }

            if (stEmpresa != "")
            {
                query = (from e in query
                         where e.st_empresa == stEmpresa
                         select e);                     
            }

            var res = new List<RelAssociadosItem>();

            query = query.OrderBy(y => y.st_matricula);

            foreach (var item in query.Skip(skip).Take(take).ToList())
            {
                var assoc = (from e in db.T_Proprietario
                             where e.i_unique == item.fk_dadosProprietario
                             select e).
                             FirstOrDefault();

                if (assoc != null)
                    res.Add(new RelAssociadosItem
                    {
                        associado = assoc.st_nome,
                        cartao = "E" + item.st_empresa + "M" + item.st_matricula,
                        cpf = assoc.st_cpf
                    });
            }

            return Ok(new { count = query.Count(), results = res });
        }
    }
}
