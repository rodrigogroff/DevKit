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
    public class LOG_AuditDTO
    {
        public string data, resp, oper, log,emp;
    }

    public class AuditoriaController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            // ----------------
            // filters
            // ----------------

            var busca = Request.GetQueryStringValue("busca")?.ToUpper();
            var skip = Request.GetQueryStringValue<int>("skip");
            var take = Request.GetQueryStringValue<int>("take");            
            var idEmpresa = Request.GetQueryStringValue<int?>("idEmpresa",null);
            var cont = Request.GetQueryStringValue("cont")?.ToUpper();
            var oper = Request.GetQueryStringValue("oper")?.ToUpper();
            var sdtInicial = Request.GetQueryStringValue<string>("dtInicial");
            var sdtFinal = Request.GetQueryStringValue<string>("dtFinal");

            DateTime? dtInicial = null, dtFinal = null;

            if (!string.IsNullOrEmpty(sdtInicial))
                dtInicial = ObtemData(sdtInicial);

            if (!string.IsNullOrEmpty(sdtFinal))
                dtFinal = Convert.ToDateTime(ObtemData(sdtFinal)).AddDays(1);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var stEmpresa = "";

            if (idEmpresa != null)
                stEmpresa = db.T_Empresa.FirstOrDefault(y => y.i_unique == idEmpresa).st_empresa;

            // ----------------
            // query
            // ----------------

            var query = (from e in db.LOG_Audit select e);

            if (!string.IsNullOrEmpty(cont))
                query = query.Where(y => y.st_log.ToUpper().Contains(cont.ToUpper()));

            if (!string.IsNullOrEmpty(oper))
                query = query.Where(y => y.st_oper.ToUpper().Contains(oper.ToUpper()));

            if (!string.IsNullOrEmpty(stEmpresa))
                query = query.Where(y => y.st_empresa == stEmpresa);

            if (dtInicial != null)
                query = query.Where(y => y.dt_operacao > dtInicial);

            if (dtFinal != null)
                query = query.Where(y => y.dt_operacao < dtFinal);

            query = query.OrderByDescending(y => y.dt_operacao);

            var lst = new List<LOG_AuditDTO>();

            foreach (var item in query.Skip(skip).Take(take).ToList())
            {
                lst.Add(new LOG_AuditDTO
                {
                    data = Convert.ToDateTime(item.dt_operacao).ToString("dd/MM/yyyy HH:mm"),
                    resp = item.fk_usuario == null ? "DBA" : db.T_Usuario.FirstOrDefault(y => y.i_unique == item.fk_usuario)?.st_nome,
                    oper = item.st_oper,
                    emp = item.st_empresa,
                    log = item.st_log,
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
