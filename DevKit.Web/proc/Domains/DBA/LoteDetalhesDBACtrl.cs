using LinqToDB;
using System.Linq;
using System.Web.Http;
using DataModel;
using System.Collections.Generic;
using System;

namespace DevKit.Web.Controllers
{
    public partial class T_LoteCartaoDetalheDTO
    {
        public string sdt_pedido, tg_emitido, nome, cpf, matricula, titularidade, via;
    }
    
    public class LoteDetalhesDBAController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var idLote = Request.GetQueryStringValue<int>("idLote");
            
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var query = (from e in db.T_LoteCartaoDetalhe
                         where e.fk_lote == idLote
                         select e).
                         ToList();

            var lstDet = new List<T_LoteCartaoDetalheDTO>();

            foreach (var item in query)
            {
                var cart = db.T_Cartao.FirstOrDefault( y=> y.i_unique == item.fk_cartao );

                var nome = "";

                if (cart.st_titularidade == "01")
                {
                    nome = db.T_Proprietario.FirstOrDefault(y => y.i_unique == cart.fk_dadosProprietario).st_nome;
                }
                else
                {
                    nome = db.T_Dependente.FirstOrDefault(y => y.fk_proprietario == cart.fk_dadosProprietario && 
                                                               y.nu_titularidade == Convert.ToInt32 (cart.st_titularidade)).st_nome;
                }

                lstDet.Add(new T_LoteCartaoDetalheDTO
                {
                    sdt_pedido = item.dt_pedido != null ? Convert.ToDateTime(item.dt_pedido).ToString() : "",
                    cpf = item.nu_cpf.ToString(),
                    tg_emitido = cart.tg_emitido == 2 ? "Emitido" : "Não Emitido ("+ cart.tg_emitido + ")",
                    nome = nome,
                    matricula = item.nu_matricula.ToString(),
                    titularidade = item.nu_titularidade.ToString(),
                    via = item.nu_via_original.ToString(),
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
