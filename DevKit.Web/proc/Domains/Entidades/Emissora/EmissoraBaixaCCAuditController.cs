using LinqToDB;
using SyCrafEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class DtoLancAuditConf
    {        
        public string operador { get; set; }
        public string data { get; set; }
        public string oper { get; set; }
        public string cartao { get; set; }
        public string vlr { get; set; }        
    }

    public class EmissoraBaixaCCAuditController : ApiControllerBase
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            var ano = Convert.ToInt32(Request.GetQueryStringValue("ano"));
            var mes = Convert.ToInt32(Request.GetQueryStringValue("mes"));

            var lst = new List<DtoLancAuditConf>();

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mon = new money();

            var tEmp = db.currentEmpresa;

            var lstUsers = db.T_Usuario.Where(y => y.st_empresa == tEmp.st_empresa).ToList();

            foreach (var item in db.LancamentosCCAudit.Where ( y=> y.nuAno == ano && y.nuMes == mes && y.fkEmpresa == tEmp.i_unique ).ToList())
            {
                var stOper = "";

                switch (item.nuOper)
                {
                    case 1: stOper = "Novo lanc. de despesa"; break;
                    case 2: stOper = "Edição de despesa"; break;
                    case 3: stOper = "Remoção de despesa"; break;
                    case 4: stOper = "Baixa manual"; break;
                }

                lst.Add(new DtoLancAuditConf
                {
                    cartao = item.stCartao,
                    data = ObtemData(item.dtLog),
                    oper = stOper,
                    operador = lstUsers.FirstOrDefault(y => y.i_unique == item.fkUser).st_nome,
                    vlr = mon.setMoneyFormat((long)item.vrValor),                    
                });
            }

            lst = lst.OrderByDescending(y => y.data).ToList();

            return Ok(
                new 
                { 
                    count = lst.Count(), 
                    results = lst,            
                });
        }        
    }
}
