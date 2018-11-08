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
                        #region - code - 

                        var dtOntem = DateTime.Now.AddDays(-1);

                        var dt = new DateTime(dtOntem.Year, dtOntem.Month, dtOntem.Day);
                        var dtFim = dt.AddDays(1);

                        return Ok(new
                        {
                            di = dt.ToString("dd/MM/yyyy"),
                        });

                        #endregion
                    }

                case "1":
                    {
                        #region - code - 

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

                        #endregion
                    }

                case "2":
                    {
                        #region - code -

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

                        #endregion
                    }

                case "3":
                    {
                        #region - code - 

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

                        #endregion
                    }

                case "4":
                    {
                        #region - code - 

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

                        #endregion
                    }

                case "10":
                    {
                        #region - code - 

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

                        #endregion
                    }

                case "11":
                    {
                        #region - code - 

                        var idEmp = Request.GetQueryStringValue<long>("id_emp");
                        var fkCartao = Request.GetQueryStringValue<long>("fkCartao");
                        var _valor = Request.GetQueryStringValue<string>("valor");

                        var term = db.T_Terminal.FirstOrDefault(y => y.nu_terminal == "00005000");

                        var c = db.T_Cartao.Where(y => y.i_unique == fkCartao).FirstOrDefault();

                        var saldo = new SaldoDisponivel();

                        {
                            var valor = ObtemValor(_valor);

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
                                tg_contabil = '3',
                                st_doc = "LANC DESP",
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

                        #endregion
                    }
                    
                case "12":
                    {
                        #region - code - 

                        var idEmp = Request.GetQueryStringValue<long>("id_emp");

                        var tEmp = db.T_Empresa.Find(idEmp);

                        if (tEmp != null)
                        {
                            var lst = (from e in db.T_Cartao
                                       join d in db.T_Proprietario on e.fk_dadosProprietario equals (int)d.i_unique
                                       where e.tg_emitido.ToString() == StatusExpedicao.NaoExpedido
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

                        #endregion 
                    }

                case "13":
                    {
                        #region - code - 

                        var lst = (from e in db.T_Cartao
                                    join d in db.T_Proprietario on e.fk_dadosProprietario equals (int)d.i_unique
                                    where e.tg_emitido.ToString() == StatusExpedicao.NaoExpedido
                                    select new
                                    {
                                        id = e.i_unique.ToString(),
                                        empresa = e.st_empresa,
                                        matricula = e.st_matricula,
                                        associado = d.st_nome,
                                        selecionado = false,
                                    }).
                                    ToList();

                        var lstEmp = lst.Select(y => y.empresa).Distinct().ToList();

                        var lstEmbDb = db.T_Empresa.Where(y => lstEmp.Contains(y.st_empresa)).ToList();

                        var lstRet = new List<object>();

                        foreach (var item in lstEmp)
                        {
                            var e = lstEmbDb.FirstOrDefault(y => y.st_empresa == item);

                            if (e != null)
                                lstRet.Add(new 
                                {
                                    sigla = e.st_empresa,
                                    nome = e.st_fantasia,
                                    cartoes = lst.Count ( y=> y.empresa == item),
                                });
                        }

                        return Ok(new
                        {
                            nuCartoes = lstRet.Count(),
                            results = lstRet
                        });

                        #endregion
                    }

                case "14":
                    {
                        var lstEmp = Request.GetQueryStringValue("list").TrimEnd(';').Split(';').ToList();

                        var lstEmpDb = (from e in db.T_Empresa
                                        where lstEmp.Contains(e.st_empresa)
                                        select e).
                                        ToList();

                        var lst = (from e in db.T_Cartao
                                   join d in db.T_Proprietario on e.fk_dadosProprietario equals (int)d.i_unique
                                   where e.tg_emitido.ToString() == StatusExpedicao.NaoExpedido
                                   where lstEmp.Contains (e.st_empresa)
                                   select new
                                   {
                                       id = e.i_unique.ToString(),
                                       empresa = e.st_empresa,
                                       matricula = e.st_matricula,
                                       titularidade = e.st_titularidade,
                                       associado = d.st_nome,
                                       selecionado = false,
                                       cpf = d.st_cpf,
                                       via = e.nu_viaCartao,
                                       nome = d.st_nome
                                   }).
                                   ToList();

                        // ------------------
                        // cria lote
                        // ------------------

                        var novoLote = new T_LoteCartao
                        {
                            dt_abertura = DateTime.Now,
                            fk_empresa  = null,
                            nu_cartoes = lst.Count(),
                            tg_sitLote = 1
                        };

                        novoLote.i_unique = Convert.ToDecimal(db.InsertWithIdentity(novoLote));

                        // ------------------
                        // cria os detalhes
                        // ------------------

                        foreach (var item in lst)
                        {
                            db.Insert(new T_LoteCartaoDetalhe
                            {
                                fk_lote = Convert.ToInt32(novoLote.i_unique),
                                fk_cartao = Convert.ToInt32(item.id),
                                fk_empresa = Convert.ToInt32(lstEmpDb.FirstOrDefault(y => y.st_empresa == item.empresa).i_unique),
                                nu_cpf = item.cpf,
                                nu_matricula = Convert.ToInt32(item.matricula),
                                nu_titularidade = Convert.ToInt32(item.titularidade),
                                nu_via_original = item.via,
                                st_nome_cartao = item.nome                                
                            });
                        }
                        
                        return Ok();
                    }

                case "100":
                    {
                        #region - code - 

                        var dtNow = DateTime.Now;
                        var dtIni = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, 0, 0, 0);
                        var dtFim = dtIni.AddDays(1);

                        var queryX = db.LOG_Transacoes.Where(y => y.dt_transacao > dtIni && y.dt_transacao < dtFim);

                        object a, b, c, d, e, f, g;

                        #region - A - 
                        {
                            var pendentesSitef = queryX.Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Pendente).
                                                        Where(y => y.tg_contabil.ToString() == TipoCaptura.SITEF).
                                                        Count();

                            var confirmadasSitef = queryX.Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Confirmada).
                                                            Where(y => y.tg_contabil.ToString() == TipoCaptura.SITEF).
                                                            Count();

                            var confirmadasPortal = queryX.Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Confirmada).
                                                            Where(y => y.tg_contabil.ToString() == TipoCaptura.PORTAL).
                                                            Count();

                            var totalConfirmadas = confirmadasSitef + confirmadasPortal;

                            a = new
                            {
                                pendentesSitef,
                                confirmadasSitef,
                                confirmadasPortal,
                                totalConfirmadas,
                            };
                        }
                        #endregion

                        #region - B -
                        {
                            var confirmadasSitef = queryX.Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Confirmada).
                                                            Where(y => y.tg_contabil.ToString() == TipoCaptura.SITEF).
                                                            Count();

                            var confirmadasPortal = queryX.Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Confirmada).
                                                            Where(y => y.tg_contabil.ToString() == TipoCaptura.PORTAL).
                                                            Count();

                            b = new
                            {
                                totalSITEF = confirmadasSitef,
                                totalPORTAL = confirmadasPortal
                            };
                        }
                        #endregion

                        #region - C -
                        {
                            var totalSitef = queryX.Where(y => y.tg_contabil.ToString() == TipoCaptura.SITEF).Count();

                            var confSitef = queryX.Where(y => y.tg_contabil.ToString() == TipoCaptura.SITEF && 
                                                                y.tg_confirmada.ToString() == TipoConfirmacao.Confirmada).Count();

                            var pendSitef = queryX.Where(y => y.tg_contabil.ToString() == TipoCaptura.SITEF &&
                                                                y.tg_confirmada.ToString() == TipoConfirmacao.Pendente).Count();

                            var totalPortal = queryX.Where(y => y.tg_contabil.ToString() == TipoCaptura.PORTAL).Count();

                            var confPortal = queryX.Where(y => y.tg_contabil.ToString() == TipoCaptura.PORTAL &&
                                                                y.tg_confirmada.ToString() == TipoConfirmacao.Confirmada).Count();

                            c = new
                            {
                                totalSitef,
                                confSitef,
                                pendSitef,
                                totalPortal,
                                confPortal
                            };
                        }
                        #endregion

                        #region - D -
                        {
                            var totalSitef = queryX.Where(y => y.tg_contabil.ToString() == TipoCaptura.SITEF).Count();
                            var totalPortal = queryX.Where(y => y.tg_contabil.ToString() == TipoCaptura.PORTAL).Count();

                            d = new
                            {
                                totTransacoesSITEF = totalSitef,
                                totTransacoesPORTAL = totalPortal,
                            };
                        }
                        #endregion

                        var queryPrincipal = db.LOG_Transacoes.
                                    Where(y => y.dt_transacao > dtIni && y.dt_transacao < dtFim).
                                    Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Confirmada ||
                                              y.tg_confirmada.ToString() == TipoConfirmacao.Pendente).
                                    ToList();

                        var queryConf = queryPrincipal.
                                Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Confirmada).
                                ToList();

                        var queryPendentes = queryPrincipal.
                                    Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Pendente).
                                    ToList();

                        var queryCanceladas = queryPrincipal.
                                    Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Cancelada).
                                    ToList();

                        #region - E -
                        {
                            var empresas = queryConf.Select(y => y.fk_empresa).Distinct().ToList();

                            var lstCountEmpresas = new List<string>();

                            foreach (var item in empresas)
                            {
                                lstCountEmpresas.Add(
                                    queryConf.Where(y => y.fk_empresa == (int)item).
                                        Sum(y => y.vr_total).ToString().PadLeft(12, '0') + "." +
                                        item.ToString().PadLeft(6, '0'));
                            }

                            lstCountEmpresas.Sort();
                            lstCountEmpresas.Reverse();

                            var lstEmpresas = lstCountEmpresas.Skip(0).Take(5).ToList();

                            var lst = new List<object>();
                            var mon = new money();

                            foreach (var item in lstEmpresas)
                            {
                                var spl = item.Split('.');

                                var vol = spl[0].TrimStart('0');
                                var idEmp = spl[1].TrimStart('0');

                                var tEmp = db.T_Empresa.FirstOrDefault(y => y.i_unique.ToString() == idEmp);

                                lst.Add(new
                                {
                                    empresa = tEmp.st_fantasia,
                                    _empresa = tEmp.st_fantasia.Split (' ')[0],
                                    financ = mon.formatToMoney(vol),
                                    _financ = vol,
                                    confirmadas = queryConf.Count(y => y.fk_empresa.ToString() == idEmp),
                                    pendentes = queryPendentes.Count(y => y.fk_empresa.ToString() == idEmp),
                                    canceladas = queryCanceladas.Count(y => y.fk_empresa.ToString() == idEmp),
                                });
                            }

                            e = new
                            {
                                list = lst
                            };
                        }
                        #endregion

                        #region - F - 
                        {
                            var lojas = queryConf.Select(y => y.fk_loja).Distinct().ToList();

                            var lstCountLojas = new List<string>();

                            foreach (var item in lojas)
                            {
                                lstCountLojas.Add(
                                    queryConf.Where(y => y.fk_loja == (int)item).
                                        Sum(y => y.vr_total).ToString().PadLeft(12, '0') + "." +
                                        item.ToString().PadLeft(6, '0'));
                            }

                            lstCountLojas.Sort();
                            lstCountLojas.Reverse();

                            var lstLojas = lstCountLojas.Skip(0).Take(5).ToList();

                            var lst = new List<object>();
                            var mon = new money();

                            var total = 0;

                            foreach (var item in lstLojas)
                            {
                                var spl = item.Split('.');

                                var vol = spl[0].TrimStart('0');
                                var idLoja = spl[1].TrimStart('0');

                                var tLoja = db.T_Loja.FirstOrDefault(y => y.i_unique.ToString() == idLoja);

                                lst.Add(new
                                {
                                    loja = tLoja.st_nome,
                                    financ = mon.formatToMoney(vol),
                                    confirmadas = queryConf.Count(y => y.fk_loja.ToString() == idLoja),
                                    pendentes = queryPendentes.Count(y => y.fk_loja.ToString() == idLoja),
                                    canceladas = queryCanceladas.Count(y => y.fk_loja.ToString() == idLoja),
                                });
                            }                            

                            f = new
                            {
                                total = queryConf.Count(),
                                list = lst
                            };
                        }
                        #endregion

                        #region - G - 
                        {
                            var lojas = queryConf.Select(y => y.fk_loja).Distinct().ToList();

                            var lstCountLojas = new List<string>();

                            foreach (var item in lojas)
                            {
                                lstCountLojas.Add(
                                    queryConf.Where(y => y.fk_loja == (int)item).
                                        Sum(y => y.vr_total).ToString().PadLeft(12, '0') + "." +
                                        item.ToString().PadLeft(6, '0'));
                            }

                            lstCountLojas.Sort();
                            lstCountLojas.Reverse();

                            var lstLojas = lstCountLojas.Skip(0).Take(5).ToList();

                            var lst = new List<object>();
                            var mon = new money();

                            long total = 0;

                            foreach (var item in lstLojas)
                            {
                                var spl = item.Split('.');

                                var vol = spl[0].TrimStart('0');
                                var idLoja = spl[1].TrimStart('0');

                                var tLoja = db.T_Loja.FirstOrDefault(y => y.i_unique.ToString() == idLoja);

                                lst.Add(new
                                {
                                    loja = tLoja.st_nome,
                                    valor = mon.formatToMoney(vol),
                                });

                                total += Convert.ToInt64(vol);
                            }

                            g = new
                            {
                                total = mon.setMoneyFormat((long)queryConf.Select(y=>y.vr_total).Sum()),
                                list = lst
                            };
                        }
                        #endregion

                        return Ok( new
                            {
                                a,b,c,d,e,f,g
                            });

                        #endregion
                    }
            }

            return BadRequest();            
        }
    }
}
