using System.Web.Http;
using System;
using System.Linq;
using LinqToDB;
using DataModel;

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
                                                           y.dt_transacao > dt && y.dt_transacao < dtFim).
                                                           ToList())
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

                case "2":
                    {
                        var id_emp = Request.GetQueryStringValue("id_emp");
                        var tipoLim = Request.GetQueryStringValue("tipoLim");
                        var tipoOper = Request.GetQueryStringValue("tipoOper");
                        var valor = ObtemValor(Request.GetQueryStringValue("valor"));

                        var emp = db.T_Empresa.FirstOrDefault(y => y.i_unique == Convert.ToDecimal(id_emp));

                        if (emp == null)
                            return BadRequest();

                        foreach (var cart in db.T_Cartao.Where (y=> y.st_empresa == emp.st_empresa).ToList())
                        {
                            var cartUpd = db.T_Cartao.FirstOrDefault(y => y.i_unique == cart.i_unique);

                            if (tipoLim == "3") // ambos limites
                            {
                                if (tipoOper == "1") // fixo
                                {
                                    cartUpd.vr_limiteMensal = (int)valor;
                                    cartUpd.vr_limiteTotal = (int)valor;
                                }
                                else // percentual
                                {
                                    var pctLM = valor * cartUpd.vr_limiteMensal / 10000;

                                    cartUpd.vr_limiteMensal += (int)pctLM;

                                    var pctLT = valor * cartUpd.vr_limiteTotal / 10000;

                                    cartUpd.vr_limiteTotal += (int)pctLT;
                                }
                            }
                            else if (tipoLim == "2") // total
                            {
                                if (tipoOper == "1") // fixo
                                {
                                    cartUpd.vr_limiteTotal = (int)valor;
                                }
                                else // percentual
                                {
                                    var pctLT = valor * cartUpd.vr_limiteTotal / 10000;

                                    cartUpd.vr_limiteTotal += (int)pctLT;
                                }
                            }
                            else if (tipoLim == "1") // mensal
                            {
                                if (tipoOper == "1") // fixo
                                {
                                    cartUpd.vr_limiteMensal = (int)valor;
                                }
                                else // percentual
                                {
                                    var pctLM = valor * cartUpd.vr_limiteMensal / 10000;

                                    cartUpd.vr_limiteMensal += (int)pctLM;
                                }
                            }

                            db.Update(cartUpd);
                        }

                        return Ok();
                    }

                case "3":
                    {
                        var idEmp = Request.GetQueryStringValue<long>("id_emp");

                        var tEmp = db.T_Empresa.Find(idEmp);

                        if (tEmp != null)
                        {
                            var lst = (from e in db.T_Cartao
                                       join d in db.T_Proprietario on e.fk_dadosProprietario equals (int) d.i_unique
                                      where e.tg_emitido.ToString() == StatusExpedicao.EmExpedicao
                                      where e.st_empresa == tEmp.st_empresa
                                      select new
                                      {
                                          id =  e.i_unique.ToString(),
                                          matricula = e.st_matricula,
                                          associado = d.st_nome
                                      }).
                                      ToList();

                            return Ok(new
                            {
                                nuCartoes = lst.Count(),
                                results = lst
                            });                            
                        }

                        return Ok();
                    }
                    

                case "4":
                    {
                        var idEmp = Request.GetQueryStringValue<long>("id_emp");
                        var ids = Request.GetQueryStringValue("ids").TrimEnd(',').Split (',');

                        var tEmp = db.T_Empresa.Find(idEmp);

                        if (tEmp != null)
                        {
                            foreach (var item in ids)
                            {
                                var tCartUpd = db.T_Cartao.Find(Convert.ToInt64(item));

                                tCartUpd.tg_emitido = Convert.ToChar(StatusExpedicao.Expedido);

                                db.Update(tCartUpd);
                            }
                        }

                        return Ok();
                    }
                    
            }

            return BadRequest();            
        }
    }
}
