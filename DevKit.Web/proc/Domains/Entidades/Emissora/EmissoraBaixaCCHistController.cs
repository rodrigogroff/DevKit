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
        public string vlrTot { get; set; }
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

            var query = db.LancamentosCCBaixa.Where(y => y.fkEmpresa == tEmp.i_unique).Where(y => y.nuYear == ano).Where(y => y.nuMonth == mes).ToList();

            foreach (var item in query)
            {
                lst.Add(new DtoBaixaHist
                {
                    data = Convert.ToDateTime(item.dtLog).ToString("dd/MM/yyyy HH:mm"),
                    id = item.id,
                    records = db.LancamentosCC.Where(y => y.fkBaixa == item.id).Count(),
                    vlrTot = mon.setMoneyFormat(db.LancamentosCC.Where(y => y.fkBaixa == item.id).Sum(y => (long)y.vrValor)),
                });
            }

            return Ok(new { count = lst.Count(), results = lst });
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
