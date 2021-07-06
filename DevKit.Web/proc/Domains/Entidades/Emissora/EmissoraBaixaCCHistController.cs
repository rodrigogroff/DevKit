using LinqToDB;
using SyCrafEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class DtoBaixaHist
    {
        public long id { get; set; }
        public string data { get; set; }
        public long records { get; set; }
        public string vlrSaldo { get; set; }
        public string vlrTot { get; set; }
        public string vlrPend { get; set; }
    }

    public class EmissoraBaixaCCHistController : ApiControllerBase
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            var ano = Convert.ToInt32(Request.GetQueryStringValue("ano"));
            var mes = Convert.ToInt32(Request.GetQueryStringValue("mes"));

            var lst = new List<DtoBaixaHist>();

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mon = new money();

            var tEmp = db.currentEmpresa;

            var queryLancs = db.LancamentosCC.Where(y => y.fkEmpresa == tEmp.i_unique).Where(y => y.nuAno == ano).Where(y => y.nuMes == mes).ToList();
            var queryFechs = db.LOG_Fechamento.Where(y => y.fk_empresa == tEmp.i_unique).Where(y => y.st_ano == ano.ToString()).Where(y => y.st_mes == mes.ToString().PadLeft(2,'0')).ToList();

            var query = db.LancamentosCCBaixa.Where(y => y.fkEmpresa == tEmp.i_unique).Where(y => y.nuYear == ano).Where(y => y.nuMonth == mes).OrderByDescending( y=> y.id).ToList();

            foreach (var item in query)
            {
                var tots = queryLancs.Where(y => y.fkBaixa == null).Sum(y => (long)y.vrValor) +
                    queryFechs.Sum(y => (long)y.vr_valor);                

                var baixa = queryLancs.Where(y => y.fkBaixa == item.id).Sum(y => (long)y.vrValor);

                var pend = tots - baixa;               
                    
                lst.Add(new DtoBaixaHist
                {
                    data = Convert.ToDateTime(item.dtLog).ToString("dd/MM/yyyy HH:mm"),
                    id = item.id,
                    records = item.nuRecords != null ? (long)item.nuRecords : 0,
                    vlrSaldo = mon.setMoneyFormat(tots),
                    vlrTot = mon.setMoneyFormat(baixa),
                    vlrPend = mon.setMoneyFormat(pend),
                });
            }

            return Ok( new 
            { 
                count = lst.Count(), 
                results = lst,                
            });
        }

        [HttpPost]
        public IHttpActionResult Post(DtoBaixaHist mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            foreach (var item in db.LancamentosCC.Where(y => y.fkBaixa == mdl.id).ToList())
                db.Delete(item);

            db.Delete ( db.LancamentosCCBaixa.FirstOrDefault(y => y.id == mdl.id) );

            return Ok();
        }
    }
}
