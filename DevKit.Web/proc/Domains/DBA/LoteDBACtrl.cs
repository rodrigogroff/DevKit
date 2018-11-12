using LinqToDB;
using DataModel;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using System.Net;

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

            foreach (var item in lst)
            {
                switch (item.tg_sitLote)
                {
                    case 1: item.situacao = "Em Aberto"; break;
                    case 2: item.situacao = "Produção na gráfica"; break;
                    case 3: item.situacao = "Cartões ativados"; break;
                }

                item.data = Convert.ToDateTime(item.dt_abertura).ToString("dd/MM/yyyy HH:mm");
                item.cartoes = item.nu_cartoes.ToString();
                item.empresas = "";

                var lstDetalhes = db.T_LoteCartaoDetalhe.Where(y => y.fk_lote == item.i_unique).ToList();

                var lstEmps = lstDetalhes.Select(y => y.fk_empresa).Distinct();

                foreach (var it in lstEmps)
                {
                    var e = db.T_Empresa.FirstOrDefault(y => y.i_unique == it).st_empresa;

                    item.empresas += e + ", ";
                }

                item.empresas = item.empresas.Trim().TrimEnd(',');
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
