using LinqToDB;
using DataModel;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using System.Net;
using System.IO;
using System.Text;
using DataModel;

namespace DevKit.Web.Controllers
{
    public class ExpDBADTO
    {
        public string cartao, empresa, dtPedido, portador, lote;
    }

    public class ExpedicaoDBAController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var sdtInicial = Request.GetQueryStringValue<string>("dtInicial");
            var sdtFinal = Request.GetQueryStringValue<string>("dtFinal");
            var skip = Request.GetQueryStringValue<int>("skip");
            var take = Request.GetQueryStringValue<int>("take");

            var idEmpresa = Request.GetQueryStringValue<int?>("idEmpresa", null);
            var ordem = Request.GetQueryStringValue<int>("ordem");

            DateTime? dtInicial = null, dtFinal = null;

            if (!string.IsNullOrEmpty(sdtInicial))
                dtInicial = ObtemData(sdtInicial);

            if (!string.IsNullOrEmpty(sdtFinal))
                dtFinal = ObtemData(sdtFinal);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var query = from e in db.T_LoteCartaoDetalhe
                        join cart in db.T_Cartao on e.fk_cartao equals (int)cart.i_unique
                        where idEmpresa == null || e.fk_empresa == idEmpresa
                        where cart.tg_emitido.ToString() == StatusExpedicao.EmExpedicao 
                        select e;

            if (dtInicial != null && dtFinal != null)
                query = from e in query
                    where e.dt_pedido > dtInicial && e.dt_pedido < dtFinal
                    where idEmpresa == null || e.fk_empresa == idEmpresa
                    select e;
            else if (dtInicial != null && dtFinal == null)
                query = from e in query
                        where e.dt_pedido > dtInicial 
                        where idEmpresa == null || e.fk_empresa == idEmpresa
                        select e;
            
            if ( ordem == 1)
            {
                query = from e in query
                        join emp in db.T_Empresa on (int)e.fk_empresa equals emp.i_unique
                        orderby emp.st_fantasia
                        select e;
            }
            else
            {
                query = query.OrderBy(y => y.dt_pedido);
            }

            var lst = new List<ExpDBADTO>();

            foreach (var item in query.Skip(skip).Take(take).ToList())
            {
                var emp = db.T_Empresa.FirstOrDefault(y => y.i_unique == item.fk_empresa);
                var cart = db.T_Cartao.FirstOrDefault(y=> y.i_unique == item.fk_cartao);

                var portador = "";

                if (cart.st_titularidade == "01")
                {
                    portador = db.T_Proprietario.FirstOrDefault(y => y.i_unique == cart.fk_dadosProprietario).st_nome;
                }
                else
                {
                    portador = db.T_Dependente.FirstOrDefault(y => y.fk_proprietario == cart.fk_dadosProprietario && y.nu_titularidade.ToString() == cart.st_titularidade).st_nome;
                }

                var dt = Convert.ToDateTime(item.dt_pedido).ToString("dd/MM/yyyy HH:mm");

                if (dt == "01/01/0001 00:00")
                    dt = "";

                lst.Add(new ExpDBADTO
                {
                    cartao = "M" + cart.st_matricula + " T" +cart.st_titularidade,
                    empresa = cart.st_empresa + " - " + emp.st_fantasia,
                    portador = portador,
                    lote = item.fk_lote.ToString(),
                    dtPedido = dt
                });
            }
     
            return Ok(new
            {
                count = query.Count(),
                results = lst
            });
        }
    }
}
