using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using System.Net;
using DataModel;
using System;
using SyCrafEngine;

namespace DevKit.Web.Controllers
{
    public class EmpresaDBAController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var busca = Request.GetQueryStringValue("busca");
            var skip = Request.GetQueryStringValue<int>("skip");
            var take = Request.GetQueryStringValue<int>("take");

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var query = (from e in db.T_Empresa select e);

            if (!string.IsNullOrEmpty(busca))
                query = query.Where(y => y.st_fantasia.Contains(busca));

            query = query.OrderBy(y => y.st_fantasia);

            var lst = new List<T_Empresa>();

            foreach (var item in query.Skip(skip).Take(take).ToList())
                lst.Add(item);

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

            var mdl = (from e in db.T_Empresa where e.i_unique == id select e).FirstOrDefault();

            if (mdl.vr_mensalidade != null)
                mdl.svrMensalidade = new money().setMoneyFormat((long)mdl.vr_mensalidade);

            if (mdl.vr_cartaoAtivo != null)
                mdl.svrCartaoAtivo = new money().setMoneyFormat((long)mdl.vr_cartaoAtivo);

            if (mdl.vr_minimo != null)
                mdl.svrMinimo = new money().setMoneyFormat((long)mdl.vr_minimo);

            if (mdl.vr_transacao != null)
                mdl.svrTransacao = new money().setMoneyFormat((long)mdl.vr_transacao);

            mdl.snuFranquia = mdl.nu_franquia?.ToString();

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            return Ok(mdl);
        }

        [HttpPut]
        public IHttpActionResult Put(T_Empresa mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mon = new money();

            if (!string.IsNullOrEmpty(mdl.svrMensalidade))
                mdl.vr_mensalidade = (int) mon.getNumericValue(mdl.svrMensalidade);

            if (!string.IsNullOrEmpty(mdl.svrCartaoAtivo))
                mdl.vr_cartaoAtivo = (int)mon.getNumericValue(mdl.svrCartaoAtivo);

            if (!string.IsNullOrEmpty(mdl.svrMinimo))
                mdl.vr_minimo = (int)mon.getNumericValue(mdl.svrMinimo);

            if (!string.IsNullOrEmpty(mdl.svrTransacao))
                mdl.vr_transacao = (int)mon.getNumericValue(mdl.svrTransacao);

            if (!string.IsNullOrEmpty(mdl.snuFranquia))
                mdl.nu_franquia = (int)mon.getNumericValue(mdl.snuFranquia);

            db.Update(mdl);

            return Ok();
        }

        [HttpPost]
        public IHttpActionResult Post(T_Empresa mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mon = new money();

            if (!string.IsNullOrEmpty(mdl.svrMensalidade))
                mdl.vr_mensalidade = (int)mon.getNumericValue(mdl.svrMensalidade);

            if (!string.IsNullOrEmpty(mdl.svrCartaoAtivo))
                mdl.vr_cartaoAtivo = (int)mon.getNumericValue(mdl.svrCartaoAtivo);

            if (!string.IsNullOrEmpty(mdl.svrMinimo))
                mdl.vr_minimo = (int)mon.getNumericValue(mdl.svrMinimo);

            if (!string.IsNullOrEmpty(mdl.svrTransacao))
                mdl.vr_transacao = (int)mon.getNumericValue(mdl.svrTransacao);

            if (!string.IsNullOrEmpty(mdl.snuFranquia))
                mdl.nu_franquia = (int)mon.getNumericValue(mdl.snuFranquia);

            mdl.i_unique = Convert.ToInt64(db.InsertWithIdentity(mdl));

            return Ok(mdl);
        }
    }
}
