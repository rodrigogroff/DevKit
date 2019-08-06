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
    public class DBAUsuarioDTO
    {
        public string id,nome, empresa, sit, ultLogin;
    }

    public class DBAUsuarioController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var busca = Request.GetQueryStringValue("busca")?.ToUpper();
            var id_empresa = Request.GetQueryStringValue<int?>("idEmpresa");
            var skip = Request.GetQueryStringValue<int>("skip");
            var take = Request.GetQueryStringValue<int>("take");
            
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var query = (from e in db.T_Usuario where e.st_empresa != "000000" select e);

            if (!string.IsNullOrEmpty(busca))
                query = query.Where(y => y.st_nome.ToUpper().Contains(busca));

            if (id_empresa > 0)
            {
                var stEmp = db.T_Empresa.FirstOrDefault(y => y.i_unique == id_empresa).st_empresa;

                query = query.Where(y => y.st_empresa == stEmp);
            }                

            query = query.OrderBy(y => y.st_empresa);

            var lst = new List<DBAUsuarioDTO>();

            foreach (var item in query.Skip(skip).Take(take).ToList())
                lst.Add(new DBAUsuarioDTO
                {
                    id = item.i_unique.ToString(),
                    empresa = item.st_empresa,
                    nome = item.st_nome,
                    sit = item.tg_bloqueio.ToString() == "0" ? "Ativo" : "Bloqueado",
                    ultLogin = db.LOG_Audit.FirstOrDefault ( y=> y.fk_usuario == item.i_unique && y.st_oper == "Login")?.dt_operacao?.ToString("dd/MM/yyyy HH:mm"),
                });

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

            var mdl = (from e in db.T_Usuario where e.i_unique == id select e).FirstOrDefault();

            return Ok(mdl);
        }

        /*
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

            if (mdl.nu_diaFech > 28)
                return BadRequest("Dia de fechamento precisa estar entre 1 e 28");

            if (mdl.nu_diaFech == 0)
                return BadRequest("Dia de fechamento não pode estar zerado");

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
        */
    }
}
