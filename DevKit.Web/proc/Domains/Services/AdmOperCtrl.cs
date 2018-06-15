using System.Web.Http;
using System;
using System.Linq;
using LinqToDB;

namespace DevKit.Web.Controllers
{
    public class AdmOperController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest("Não autorizado!");

            var op = Request.GetQueryStringValue("op");

            switch (op)
            {
                case "0":
                    {
                        var dtOntem = DateTime.Now.AddDays(-1);

                        var dt = new DateTime(dtOntem.Year, dtOntem.Month, dtOntem.Day);
                        var dtFim = dt.AddDays(1);

                        return Ok(new
                        {
                            di = dt.ToString("dd/MM/yyyy"),
                        });
                    }

                case "1":
                    {
                        var di = Request.GetQueryStringValue("di");
                        var df = Request.GetQueryStringValue("df");

                        var dtOntem = DateTime.Now.AddDays(-1);                        

                        var dt = new DateTime(dtOntem.Year, dtOntem.Month, dtOntem.Day);
                        var dtFim = dt.AddDays(1);

                        if (!string.IsNullOrEmpty(di) && !string.IsNullOrEmpty(df))
                        {
                            dt = Convert.ToDateTime(ObtemData(di));
                            dtFim = Convert.ToDateTime(ObtemData(df)).AddDays(1);
                        }
                        else if (di != "")
                        {
                            dt = Convert.ToDateTime(ObtemData(di));
                            dtFim = dt.AddDays(1);
                        }
                        
                        int hits = 0;

                        foreach (var item in db.LOG_Transacoes.
                                                Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Pendente &&
                                                           y.dt_transacao > dt && y.dt_transacao < dtFim))
                        {
                            var itUpd = db.LOG_Transacoes.FirstOrDefault(y => y.i_unique == item.i_unique);
                            itUpd.tg_confirmada = Convert.ToChar(TipoConfirmacao.Confirmada);
                            db.Update(itUpd);
                            hits++;
                        }

                        return Ok(new
                        {
                            resp = hits.ToString()
                        });
                    }
            }

            return BadRequest();            
        }
    }
}
