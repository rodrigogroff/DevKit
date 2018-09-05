using System.Web.Http;
using System;
using System.Linq;
using LinqToDB;
using DataModel;
using System.Collections.Generic;
using SyCrafEngine;

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

                        foreach (var cart in db.T_Cartao.Where(y => y.st_empresa == emp.st_empresa).ToList())
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
                                       join d in db.T_Proprietario on e.fk_dadosProprietario equals (int)d.i_unique
                                       where e.tg_emitido.ToString() == StatusExpedicao.EmExpedicao
                                       where e.st_empresa == tEmp.st_empresa
                                       select new
                                       {
                                           id = e.i_unique.ToString(),
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
                        var ids = Request.GetQueryStringValue("ids").TrimEnd(',').Split(',');

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

                case "10":
                    {
                        var idEmp = Request.GetQueryStringValue<long>("id_emp");
                        var dtInicial = ObtemData(Request.GetQueryStringValue("dtInicial"));
                        var dtFinal = ObtemData(Request.GetQueryStringValue("dtFinal")).Value.AddDays(1);
                        var valor = ObtemValor(Request.GetQueryStringValue("valor"));

                        var lstTrans = db.LOG_Transacoes.
                                            Where(y => y.fk_empresa == idEmp &&
                                                        y.tg_confirmada.ToString() == TipoConfirmacao.Confirmada &&
                                                        y.dt_transacao > dtInicial &&
                                                        y.dt_transacao < dtFinal).
                                            ToList();

                        var lst = new List<LancDespesa>();

                        var lstIdsCartoes = lstTrans.Select(y => y.fk_cartao).Distinct().ToList();
                        var lstCartoes = db.T_Cartao.Where(y => lstIdsCartoes.Contains((int)y.i_unique)).ToList();
                        var lstIdsProps = lstCartoes.Select(y => y.fk_dadosProprietario).Distinct().ToList();
                        var lstProps = db.T_Proprietario.Where(y => lstIdsProps.Contains((int)y.i_unique)).ToList();

                        var mon = new money();
                        var saldo = new SaldoDisponivel
                        {
                            lstParcelas = db.T_Parcelas.Where(y => lstIdsCartoes.Contains(y.fk_cartao) && y.nu_parcela > 0).ToList()
                        };

                        var lstIdsTrans = saldo.lstParcelas.Select(y => y.fk_log_transacoes).Distinct().ToList();
                        saldo.lstTrans = db.LOG_Transacoes.Where(y => lstIdsTrans.Contains((int)y.i_unique)).ToList();

                        foreach (var cart in lstCartoes)
                        {
                            var prop = lstProps.FirstOrDefault(y => y.i_unique == cart.fk_dadosProprietario);
                            if (prop == null) continue;

                            long vrDisp = (long)cart.vr_limiteMensal,
                                vrDispTotal = (long)cart.vr_limiteTotal;

                            saldo.ObterEmLista(db, cart, ref vrDisp, ref vrDispTotal);

                            lst.Add(new LancDespesa
                            {
                                matricula = cart.st_matricula,
                                associado = prop.st_nome,
                                valor = mon.setMoneyFormat(valor),
                                fkCartao = cart.i_unique.ToString(),
                                saldo = mon.setMoneyFormat(vrDisp),
                                falta = vrDisp < valor,
                            });
                        }

                        return Ok(new
                        {
                            total = lst.Count(),
                            results = lst
                        });
                    }

                case "11":
                    {
                        var idEmp = Request.GetQueryStringValue<long>("id_emp");
                        var lista = Request.GetQueryStringValue("lista").TrimEnd(';').Split(';');

                        var term = db.T_Terminal.FirstOrDefault(y => y.nu_terminal == "00005000");

                        var lstIdsCartoes = new List<long>();

                        foreach (var item in lista)
                        {
                            var v = item.Split('|');
                            lstIdsCartoes.Add(Convert.ToInt64(v[0]));
                        }

                        var lstCartoes = db.T_Cartao.Where(y => lstIdsCartoes.Contains((long)y.i_unique)).ToList();

                        var saldo = new SaldoDisponivel();

                        foreach (var item in lista)
                        {
                            var v = item.Split('|');
                            var c = lstCartoes.FirstOrDefault(y => y.i_unique == Convert.ToInt64(v[0]));

                            var valor = ObtemValor(v[1]);

                            long vrDisp = (long)c.vr_limiteMensal,
                                 vrDispTotal = (long)c.vr_limiteTotal;

                            saldo.Obter(db, c, ref vrDisp, ref vrDispTotal);

                            long dif = vrDisp - valor;

                            if (dif < 0)
                            {
                                c.vr_extraCota = -(int)dif;

                                db.Update(c);
                            }

                            var l_nsu = new LOG_NSU
                            {
                                dt_log = DateTime.Now
                            };

                            l_nsu.i_unique = Convert.ToInt32(db.InsertWithIdentity(l_nsu));

                            var l_tr = new LOG_Transaco
                            {
                                fk_terminal = term != null ? (int)term.i_unique : (int?)null,
                                fk_empresa = (int)idEmp,
                                fk_cartao = c != null ? (int)c.i_unique : (int?)null,
                                vr_total = Convert.ToInt32(valor),
                                nu_parcelas = Convert.ToInt32(1),
                                nu_nsu = Convert.ToInt32(l_nsu.i_unique),
                                dt_transacao = DateTime.Now,
                                nu_cod_erro = 0,
                                nu_nsuOrig = 0,
                                en_operacao = OperacaoCartao.VENDA_EMPRESARIAL,
                                st_msg_transacao = "Lanc. Adm",
                                fk_loja = term != null ? (int)term.fk_loja : (int?)null,
                                tg_confirmada = Convert.ToChar(TipoConfirmacao.Confirmada),
                                tg_contabil = '1',
                                st_doc = "",                                
                            };

                            l_tr.i_unique = Convert.ToInt32(db.InsertWithIdentity(l_tr));

                            var parc = new T_Parcela
                            {
                                nu_nsu = (int)l_nsu.i_unique,
                                fk_log_transacoes = (int)l_tr.i_unique,                                
                                fk_empresa = (int)idEmp,
                                fk_cartao = (int)c.i_unique,
                                dt_inclusao = DateTime.Now,
                                nu_parcela = 1,
                                vr_valor = Convert.ToInt32(valor),
                                nu_indice = 1,
                                tg_pago = '0',
                                fk_loja = (int)term.fk_loja,
                                nu_tot_parcelas = 1,
                                fk_terminal = (int)term.i_unique
                            };

                            parc.i_unique = Convert.ToInt32(db.InsertWithIdentity(parc));                            
                        }

                        return Ok();                        
                    }

            }

            return BadRequest();            
        }
    }
}
