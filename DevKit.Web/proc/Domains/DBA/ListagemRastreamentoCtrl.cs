using LinqToDB;
using System.Linq;
using System.Web.Http;
using System.Collections.Generic;
using System;

namespace DevKit.Web.Controllers
{
    public partial class RastreamentoDTO
    {
        public string dtPedido, empresa, nome, mat, dtEnvio, sedex, id;
    }

    public class ListagemRastreamentoController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var skip = Request.GetQueryStringValue<int>("skip");
            var take = Request.GetQueryStringValue<int>("take");
            var mat = Request.GetQueryStringValue<int>("mat");
            var nome = Request.GetQueryStringValue("nome");
            var dtInicial = ObtemData(Request.GetQueryStringValue("dtInicial"));
            var dtFinal = ObtemData(Request.GetQueryStringValue("dtFinal"));

            /*
             * var dtEnvio = ObtemData(Request.GetQueryStringValue("dtEnvio"));
             * var sedex = Request.GetQueryStringValue("sedex");
             */

            var idEmpresa = Request.GetQueryStringValue<int?>("idEmpresa", null);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var query = (from e in db.T_LoteCartaoDetalhe  select e);

            if (idEmpresa != null)
                query = query.Where(y => y.fk_empresa == idEmpresa);

            if (!string.IsNullOrEmpty(nome))
            {
                query = query.Where(y => y.st_nome_cartao.ToUpper().Contains(nome.ToUpper()));
            }

            if (mat > 0)
            {
                query = query.Where(y => y.nu_matricula == mat);
            }

            if (dtInicial != null)
            {
                query = query.Where(y => y.dt_pedido >= dtInicial);
            }

            if (dtFinal != null)
            {
                query = query.Where(y => y.dt_pedido <= dtFinal);
            }

            query = from e in query
                    orderby e.dt_pedido descending
                    select e;

            var lstDet = new List<RastreamentoDTO>();

            var lstEmpresas = db.T_Empresa.ToList();

            foreach (var item in query.ToList().Skip(skip).Take(take))
            {
                lstDet.Add(new RastreamentoDTO
                {
                    dtPedido = Convert.ToDateTime(item.dt_pedido).ToString("dd/MM/yyyy HH:mm"),
                    dtEnvio = "",
                    empresa = lstEmpresas.FirstOrDefault( y=> y.i_unique == item.fk_empresa )?.st_empresa,
                    id = item.i_unique.ToString(),
                    mat = item.nu_matricula.ToString(),
                    sedex = "",
                    nome = item.st_nome_cartao
                });
            }

            return Ok(new
            {
                count = query.Count(),
                results = lstDet.ToList()
            });
        }
    }
}
