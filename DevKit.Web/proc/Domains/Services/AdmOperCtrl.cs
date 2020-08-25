using System.Web.Http;
using System;
using System.Linq;
using LinqToDB;
using DataModel;
using System.Collections.Generic;
using SyCrafEngine;
using System.IO;
using DocumentFormat.OpenXml.Bibliography;
using System.Text;

namespace DevKit.Web.Controllers
{
    public class PointConvey
    {
        public long x { get; set; }
        public long y { get; set; }
    }

    public class PointConveyLabel
    {
        public long x { get; set; }
        public string label { get; set; }
    }

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
                        var nsu = Request.GetQueryStringValue("nsu");

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
                            if (!string.IsNullOrEmpty(nsu))
                                if (item.nu_nsu != Convert.ToInt32(nsu))
                                    continue;

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

                        if (userLoggedParceiroId != "1")
                            return BadRequest("Não autorizado!");

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
                                           titularidade = e.st_titularidade,
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

                case "4": // ativação manual empresa
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

                                tCartUpd.tg_emitido = Convert.ToInt32(StatusExpedicao.Expedido);

                                var loteDet = db.T_LoteCartaoDetalhe.
                                                    Where(y => y.fk_cartao == Convert.ToInt64(item)).
                                                    OrderByDescending(y => y.i_unique).
                                                    FirstOrDefault();

                                if (loteDet != null)
                                {
                                    loteDet.dt_ativacao = DateTime.Now;
                                    db.Update(loteDet);
                                }

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

                        if (userLoggedParceiroId != "1")
                            return BadRequest("Não autorizado!");

                        var lstIdEmpresas = ObtemDBAListaEmpresasParceiroId();
                        var lstStrEmpresas = ObtemDBAListaEmpresasParceiroStr();

                        var lstCarts = (from e in db.T_Cartao
                                        where e.tg_emitido.ToString() == StatusExpedicao.NaoExpedido
                                        where lstStrEmpresas.Contains(e.st_empresa)
                                        where e.tg_status.ToString() == CartaoStatus.Habilitado
                                        select (int)e.i_unique).
                                        ToList();

                        var lstLotesAbertos = db.T_LoteCartao.
                                                Where(y => y.tg_sitLote == 1).
                                                Where ( y=> lstIdEmpresas.Contains((long)y.fk_empresa)).
                                                Select(y => (int)y.i_unique).
                                                ToList();

                        var lstLoteDets = db.T_LoteCartaoDetalhe.
                                                Where(y => lstLotesAbertos.Contains((int)y.fk_lote)).
                                                Select(y => (int)y.fk_cartao).
                                                ToList();

                        var lstCartsDisp = new List<int>();

                        for (int i = 0; i < lstCarts.Count(); i++)
                            if (!lstLoteDets.Contains(lstCarts[i]))
                                lstCartsDisp.Add(lstCarts[i]);
                                                       
                        var lst = (from e in db.T_Cartao
                                    join d in db.T_Proprietario on e.fk_dadosProprietario equals (int)d.i_unique
                                    where lstCartsDisp.Contains ((int)e.i_unique)
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


                        int tC = 0;

                        foreach (var item in lstEmp)
                        {
                            var e = lstEmbDb.FirstOrDefault(y => y.st_empresa == item);

                            if (e != null)
                            {
                                var co = lst.Count(y => y.empresa == item);

                                lstRet.Add(new
                                {
                                    sigla = e.st_empresa,
                                    nome = e.st_fantasia,
                                    cartoes = co,
                                });

                                tC += co;
                            }
                        }

                        return Ok(new
                        {
                            nuCartoes = tC,
                            results = lstRet
                        });

                        #endregion
                    }

                case "14":
                    {
                        #region - code -

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
                                st_nome_cartao = item.nome,
                                dt_pedido = DateTime.Now,
                            });
                        }
                        
                        return Ok();

                        #endregion
                    }

                case "20": // ativação via leitor
                    {
                        #region - code -  

                        if (userLoggedParceiroId != "1")
                            return BadRequest("Não autorizado!");

                        var cartao = Request.GetQueryStringValue("cartao");

                        // "826766000002000001012650716"

                        var trilha = cartao.Replace("826766", ";").Split(';')[1];

                        // "000002000001012650716"

                        var emp = trilha.Substring(0, 6);
                        var mat = trilha.Substring(6, 6);
                        var tit = trilha.Substring(12, 2);

                        var t_cartao = db.T_Cartao.FirstOrDefault(y =>  y.st_empresa == emp && 
                                                                        y.st_matricula == mat && 
                                                                        y.st_titularidade == tit    );

                        if (t_cartao != null)
                        {
                            // encontra o ultimo lote_detalhe

                            var ltDet = db.T_LoteCartaoDetalhe.
                                            Where(y => y.fk_cartao == t_cartao.i_unique).
                                            OrderByDescending(y => y.i_unique).
                                            FirstOrDefault();

                            if (ltDet != null)
                            {
                                ltDet.dt_ativacao = DateTime.Now;

                                db.Update(ltDet);
                            }

                            if (t_cartao.tg_emitido.ToString() == StatusExpedicao.EmExpedicao)
                            {
                                t_cartao.tg_emitido = Convert.ToInt32(StatusExpedicao.Expedido);
                                db.Update(t_cartao);

                                return Ok(new {
                                    data = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                    cartao = emp + "." + mat + "." + tit,
                                    mensagem = "Cartão ativado com sucesso!"
                                });
                            }
                            else
                            {
                                if (t_cartao.tg_emitido.ToString() == StatusExpedicao.Expedido)
                                {
                                    return Ok(new
                                    {
                                        data = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        cartao = emp + "." + mat + "." + tit,
                                        mensagem = "Cartão já expedido!"
                                    });
                                }                                    
                                else
                                {
                                    if (t_cartao.tg_emitido.ToString() == StatusExpedicao.NaoExpedido)
                                        return Ok(new
                                    {
                                        data = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                        cartao = emp + "." + mat + "." + tit,
                                        mensagem = "Cartão não enviado para gráfica!"
                                        });
                                }
                            }                                
                        }

                        return Ok();

                        #endregion 
                    }

                case "21": // ativação em lote
                    {
                        #region - code -  

                        var idLote = Request.GetQueryStringValue("lote");

                        var lote = db.T_LoteCartao.FirstOrDefault( y=> y.i_unique.ToString() == idLote);

                        if (lote != null)
                        {
                            lote.dt_ativacao = DateTime.Now;
                            lote.tg_sitLote = 3;

                            db.Update(lote);

                            var dbCarts = db.T_LoteCartaoDetalhe.Where(y => y.fk_lote.ToString() == idLote).ToList();

                            foreach (var item in dbCarts)
                            {
                                var cart = db.T_Cartao.FirstOrDefault(y => y.i_unique == item.fk_cartao);

                                if (cart != null)
                                {
                                    cart.tg_emitido = Convert.ToInt32(StatusExpedicao.Expedido);

                                    db.Update(cart);
                                }

                                item.dt_ativacao = DateTime.Now;

                                db.Update(item);
                            }
                        }
                        
                        return Ok();

                        #endregion 
                    }

                case "22": // remover lote
                    {
                        #region - code -

                        var idLote = Convert.ToInt32(Request.GetQueryStringValue("lote"));

                        var lote = db.T_LoteCartao.FirstOrDefault(y => y.i_unique == idLote);

                        foreach (var item in db.T_LoteCartaoDetalhe.Where ( y=> y.fk_lote == idLote).Select ( y=> y.i_unique).ToList())
                        {
                            var del = db.T_LoteCartaoDetalhe.Find(item);

                            db.Delete(del);
                        }

                        db.Delete(lote);

                        return Ok();
                        
                        #endregion
                    }

                case "100": // dashboard
                    {
                        #region - code - 

                        var dtNow = DateTime.Now;
                        var dtIni = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, 0, 0, 0);
                        var dtFim = dtIni.AddDays(1);

                        var queryX = db.LOG_Transacoes.Where(y => y.dt_transacao > dtIni && y.dt_transacao < dtFim);

                        if (Convert.ToInt32(userLoggedParceiroId) > 1)
                        {
                            var lst = ObtemDBAListaEmpresasParceiroId();

                            queryX = queryX.Where(y => lst.Contains((long)y.fk_empresa));
                        }

                        object a, b, c, d, e, f, g, h, i, j;

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

                            var confirmadasMobile = queryX.Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Confirmada).
                                                            Where(y => y.tg_contabil.ToString() == TipoCaptura.MOBILE).
                                                            Count();

                            var totalConfirmadas = confirmadasSitef + confirmadasPortal + confirmadasMobile;

                            a = new
                            {
                                pendentesSitef,
                                confirmadasSitef,
                                confirmadasPortal,
                                confirmadasMobile,
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

                            var confirmadasMobile = queryX.Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Confirmada).
                                                            Where(y => y.tg_contabil.ToString() == TipoCaptura.MOBILE).
                                                            Count();

                            b = new
                            {
                                totalSITEF = confirmadasSitef,
                                totalPORTAL = confirmadasPortal,
                                totalMOBILE = confirmadasMobile,
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

                            var totalMobile = queryX.Where(y => y.tg_contabil.ToString() == TipoCaptura.MOBILE).Count();

                            c = new
                            {
                                totalSitef,
                                confSitef,
                                pendSitef,
                                totalPortal,
                                confPortal,
                                totalMobile
                            };
                        }
                        #endregion

                        #region - D -
                        {
                            var totalSitef = queryX.Where(y => y.tg_contabil.ToString() == TipoCaptura.SITEF).Count();
                            var totalPortal = queryX.Where(y => y.tg_contabil.ToString() == TipoCaptura.PORTAL).Count();
                            var totalMobile = queryX.Where(y => y.tg_contabil.ToString() == TipoCaptura.MOBILE).Count();

                            d = new
                            {
                                totTransacoesSITEF = totalSitef,
                                totTransacoesPORTAL = totalPortal,
                                totTransacoesMOBILE = totalMobile,
                            };
                        }
                        #endregion

                        var queryPrincipal = db.LOG_Transacoes.
                                    Where(y => y.dt_transacao > dtIni && y.dt_transacao < dtFim).
                                    Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Confirmada ||
                                              y.tg_confirmada.ToString() == TipoConfirmacao.Pendente);

                        if (Convert.ToInt32(userLoggedParceiroId) > 1)
                        {
                            var lst = ObtemDBAListaEmpresasParceiroId();

                            queryPrincipal = queryPrincipal.Where(y => lst.Contains((long)y.fk_empresa));
                        }

                        var final_queryPrincipal = queryPrincipal.ToList();

                        var queryConf = final_queryPrincipal.
                                Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Confirmada).
                                ToList();

                        var queryPendentes = final_queryPrincipal.
                                    Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Pendente).
                                    ToList();

                        var queryCanceladas = final_queryPrincipal.
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

                        #region - H - 
                        {
                            var emps = db.T_Empresa.Where(y => y.nu_diaFech == DateTime.Now.Day).ToList();
                            var mon = new money();

                            foreach (var item in emps)
                            {
                                item.sfechFinalizado = db.T_JobFechamento.Any(y => y.fk_empresa == item.i_unique &&
                                                                                   y.st_ano == DateTime.Now.Year.ToString() &&
                                                                                   y.st_mes == DateTime.Now.Month.ToString().PadLeft(2, '0') &&
                                                                                   y.dt_fim != null) ? "Sim" : "Não";

                                if (item.sfechFinalizado == "Sim")
                                    item.sDtFech = db.T_JobFechamento.FirstOrDefault(y => y.fk_empresa == item.i_unique &&
                                                                                      y.st_ano == DateTime.Now.Year.ToString() &&
                                                                                      y.st_mes == DateTime.Now.Month.ToString().PadLeft(2, '0') &&
                                                                                      y.dt_fim != null).dt_inicio?.ToString("dd/MM/yyyy HH:mm");

                                long atu = 0, ult = 0;

                                var dtUltimo = DateTime.Now.AddMonths(-1);


                                ult = db.LOG_Fechamento.Where(y => y.fk_empresa == item.i_unique &&
                                                                                   y.st_ano == dtUltimo.Year.ToString() &&
                                                                                   y.st_mes == dtUltimo.Month.ToString().PadLeft(2, '0')).
                                                                                     Sum(y => (long)y.vr_valor);

                                item.sultimo = "R$ " + mon.setMoneyFormat(ult);
                                

                                if (item.sfechFinalizado == "Sim")
                                {
                                    item.sfechCartoes = db.LOG_Fechamento.Where(y => y.fk_empresa == item.i_unique &&
                                                                                     y.st_ano == DateTime.Now.Year.ToString() &&
                                                                                     y.st_mes == DateTime.Now.Month.ToString().PadLeft(2, '0')).
                                                                                     Select(y => y.fk_cartao).
                                                                                     Distinct().
                                                                                     Count().
                                                                                     ToString();

                                    atu = db.LOG_Fechamento.Where(y => y.fk_empresa == item.i_unique &&
                                                                                   y.st_ano == DateTime.Now.Year.ToString() &&
                                                                                   y.st_mes == DateTime.Now.Month.ToString().PadLeft(2, '0')).
                                                                                     Sum(y => (long) y.vr_valor);

                                    item.sfechValorTotal = "R$ " + mon.setMoneyFormat(atu);

                                    if (ult > 0)
                                    {
                                        if (atu > ult)
                                        {
                                            float x1 = atu - ult;
                                            float x2 = x1 / ult;

                                            item.svariacao = mon.setMoneyFormat( Convert.ToInt64(x2*10000)) + " %";
                                        }
                                        else
                                        {
                                            float x1 = ult - atu;
                                            float x2 = x1 / atu;

                                            item.svariacao = "-" + mon.setMoneyFormat(Convert.ToInt64(x2 * 10000)) + " %";
                                        }                                        
                                    }
                                }
                            }

                            h = new 
                            {
                                fechamentos = emps
                            };
                        }
                        #endregion

                        #region - I - 

                        {
                            int totMonths = 3;

                            var strMonths = new List<string> { "", "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro" };

                            var qAtual = db.DashboardGrafico.Where(y => y.dtDia > DateTime.Now.AddMonths(-totMonths)).ToList();
                            var qPassado = db.DashboardGrafico.Where(y => y.dtDia > DateTime.Now.AddYears(-1).AddMonths(-totMonths) && y.dtDia < DateTime.Now.AddYears(-1)).ToList();

                            var res = new List<PointConvey>();
                            var resOld = new List<PointConvey>();
                            var ticks = new List<PointConveyLabel>();

                            var dt = DateTime.Now.AddMonths(-totMonths);
                            var dtFinal = DateTime.Now.AddDays(-1);

                            int index = 1;

                            while (dt < dtFinal )
                            {
                                var qa = qAtual.FirstOrDefault(y => y.nuAno == dt.Year && y.nuMes == dt.Month && y.nuDia == dt.Day);

                                if (qa == null)
                                {
                                    var lst = db.LOG_Transacoes.
                                                Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Confirmada).
                                                Where(y => y.dt_transacao > dt && y.dt_transacao < dt.AddDays(1)).
                                                ToList();

                                    qa = new DashboardGrafico
                                    {
                                        nuAno = dt.Year,
                                        nuMes = dt.Month,
                                        nuDia = dt.Day,
                                        totalTransacoes = lst.Count(),
                                        totalCartoes = lst.Select(y => y.fk_cartao).Distinct().Count(),
                                        totalFinanc = lst.Sum(y => (int)y.vr_total),
                                        totalLojas = lst.Select(y => y.fk_loja).Distinct().Count(),
                                        dtDia = dt
                                    };

                                    db.Insert(qa);
                                }

                                res.Add(new PointConvey
                                {
                                    x = index,
                                    y = qa != null ? qa.totalTransacoes : 0,
                                });
                                
                                var qp = qPassado.FirstOrDefault(y => y.nuAno == dt.Year - 1 && y.nuMes == dt.Month && y.nuDia == dt.Day);

                                resOld.Add(new PointConvey
                                {
                                    x = index,
                                    y = qp != null ? qp.totalTransacoes : 0,
                                });

                                if (dt.Day == 15 || dt.Day == 1)
                                {
                                    ticks.Add(new PointConveyLabel
                                    {
                                        label = dt.Day + "/" + strMonths[dt.Month],
                                        x = index,
                                    });
                                }

                                index++;
                                dt = dt.AddDays(1);
                            }

                            i = new
                            {
                                list = res,
                                listOld = resOld,
                                ticks,
                                label_a = DateTime.Now.Year.ToString(),
                                label_b = DateTime.Now.AddYears(-1).Year.ToString(),
                            };
                        }

                        #endregion

                        #region - J - 

                        {
                            int totMonths = 3;

                            var strMonths = new List<string> { "", "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro" };

                            var qAtual = db.DashboardGrafico.Where(y => y.dtDia > DateTime.Now.AddMonths(-totMonths)).ToList();
                            var qPassado = db.DashboardGrafico.Where(y => y.dtDia > DateTime.Now.AddYears(-1).AddMonths(-totMonths) && y.dtDia < DateTime.Now.AddYears(-1)).ToList();

                            var res = new List<PointConvey>();
                            var resOld = new List<PointConvey>();
                            var ticks = new List<PointConveyLabel>();

                            var dt = DateTime.Now.AddMonths(-totMonths);
                            var dtFinal = DateTime.Now.AddDays(-1);

                            int index = 1;

                            while (dt < dtFinal)
                            {
                                var qa = qAtual.FirstOrDefault(y => y.nuAno == dt.Year && y.nuMes == dt.Month && y.nuDia == dt.Day);

                                if (qa == null)
                                {
                                    var lst = db.LOG_Transacoes.
                                                Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Confirmada).
                                                Where(y => y.dt_transacao > dt && y.dt_transacao < dt.AddDays(1)).
                                                ToList();

                                    qa = new DashboardGrafico
                                    {
                                        nuAno = dt.Year,
                                        nuMes = dt.Month,
                                        nuDia = dt.Day,
                                        totalTransacoes = lst.Count(),
                                        totalCartoes = lst.Select(y => y.fk_cartao).Distinct().Count(),
                                        totalFinanc = lst.Sum(y => (int)y.vr_total),
                                        totalLojas = lst.Select(y => y.fk_loja).Distinct().Count(),
                                        dtDia = dt
                                    };

                                    db.Insert(qa);
                                }

                                res.Add(new PointConvey
                                {
                                    x = index,
                                    y = qa != null ? qa.totalFinanc / 100 : 0,
                                });

                                var qp = qPassado.FirstOrDefault(y => y.nuAno == dt.Year - 1 && y.nuMes == dt.Month && y.nuDia == dt.Day);

                                resOld.Add(new PointConvey
                                {
                                    x = index,
                                    y = qp != null ? qp.totalFinanc / 100 : 0,
                                });

                                if (dt.Day == 15 || dt.Day == 1)
                                {
                                    ticks.Add(new PointConveyLabel
                                    {
                                        label = dt.Day + "/" + strMonths[dt.Month],
                                        x = index,
                                    });
                                }

                                index++;
                                dt = dt.AddDays(1);
                            }

                            j = new
                            {
                                list = res,
                                listOld = resOld,
                                ticks,
                                label_a = DateTime.Now.Year.ToString(),
                                label_b = DateTime.Now.AddYears(-1).Year.ToString(),
                            };
                        }

                        #endregion

                        return Ok( new
                            {
                                a,b,c,d,e,f,g,h,i,j
                            });

                        #endregion
                    }

                case "101": //forçar fech
                    {
                        #region -  code - 

                        var mat = Request.GetQueryStringValue("emp").PadLeft(6, '0');

                        var dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 10, 0);

                        using (var db = new AutorizadorCNDB())
                        {
                            string currentEmpresa = "";

                            try
                            {
                                var diaFechamento = dt.Day;
                                var horaAtual = dt.ToString("HHmm");
                                var ano = dt.ToString("yyyy");
                                var mes = dt.ToString("MM").PadLeft(2, '0');

                                var lstEmpresas = db.T_Empresa.Where(y => y.st_empresa == mat).ToList();

                                foreach (var empresa in lstEmpresas)
                                {
                                    // ------------------------------
                                    // só fecha uma vez no mes
                                    // ------------------------------

                                    if (db.LOG_Fechamento.Any(y => y.st_ano == ano &&
                                                                    y.st_mes == mes &&
                                                                    y.fk_empresa == empresa.i_unique))
                                    {
                                        // ------------------------------
                                        // desfaz fechamento
                                        // ------------------------------

                                        var lstDelFech = new List<long>();

                                        var lstOld = db.LOG_Fechamento.Where(y => y.fk_empresa == empresa.i_unique && y.st_mes == mes && y.st_ano == ano).ToList();

                                        int counterOld = 0;

                                        foreach (var itemFech in lstOld)
                                        {
                                            ++counterOld;

                                            lstDelFech.Add((long)itemFech.i_unique);

                                            var parc = db.T_Parcelas.FirstOrDefault(y => y.i_unique == itemFech.fk_parcela);
                                            var logTrans = parc.fk_log_transacoes.ToString();
                                            var cart = db.T_Cartao.FirstOrDefault(y => y.i_unique == itemFech.fk_cartao);

                                            var lstParcs = db.T_Parcelas.Where(y => y.fk_log_transacoes.ToString() == logTrans).OrderBy(y => y.nu_indice).ToList();

                                            foreach (var itemParc in lstParcs)
                                            {
                                                if (itemParc.i_unique >= itemFech.fk_parcela)
                                                {
                                                    var parcUpd = db.T_Parcelas.FirstOrDefault(y => y.i_unique == itemParc.i_unique);
                                                    parcUpd.nu_parcela++;
                                                    db.Update(parcUpd);
                                                }
                                            }
                                        }

                                        foreach (var item in lstDelFech)
                                        {
                                            var itemF = db.LOG_Fechamento.FirstOrDefault(y => y.i_unique == item);
                                            db.Delete(itemF);
                                        }

                                        // limpar T_JobFechamento

                                        var it_job = db.T_JobFechamento.FirstOrDefault(y => y.fk_empresa == empresa.i_unique && y.st_ano == ano && y.st_mes == mes);

                                        if (it_job!= null)
                                            db.Delete(it_job);
                                    }

                                    currentEmpresa = empresa.st_empresa;

                                    db.Insert(new LOG_Audit
                                    {
                                        dt_operacao = DateTime.Now,
                                        fk_usuario = null,
                                        st_oper = "Fechamento [INICIO]",
                                        st_empresa = currentEmpresa,
                                        st_log = "Ano " + ano + " Mes " + mes
                                    });

                                    var g_job = new T_JobFechamento
                                    {
                                        dt_inicio = DateTime.Now,
                                        dt_fim = null,
                                        fk_empresa = (int)empresa.i_unique,
                                        st_ano = ano,
                                        st_mes = mes
                                    };

                                    // ----------------------------
                                    // registra job
                                    // ----------------------------

                                    g_job.i_unique = Convert.ToInt32(db.InsertWithIdentity(g_job));

                                    // ----------------------------
                                    // busca parcelas
                                    // ----------------------------

                                    long totValor = 0, ind_parc = 1, tot_parcs_sel;

                                    var lst = db.T_Parcelas.Where(y => y.fk_empresa == empresa.i_unique && y.nu_parcela > 0).ToList();

                                    tot_parcs_sel = lst.Count();

                                    foreach (var parc in lst)
                                    {
                                        // ----------------------------
                                        // somente confirmadas
                                        // ----------------------------

                                        var logTrans = db.LOG_Transacoes.FirstOrDefault(y => y.i_unique == parc.fk_log_transacoes);

                                        if (logTrans == null)
                                            continue;
                                        else
                                        {
                                            if (logTrans.tg_confirmada == null)
                                                continue;

                                            if (logTrans.tg_confirmada.ToString() != TipoConfirmacao.Confirmada)
                                                continue;

                                            if (logTrans.dt_transacao > dt)
                                                continue;
                                        }

                                        // ----------------------------
                                        // decrementa parcela
                                        // ----------------------------

                                        var parcUpd = db.T_Parcelas.FirstOrDefault(y => y.i_unique == parc.i_unique);
                                        parcUpd.nu_parcela--;
                                        db.Update(parcUpd);

                                        // -------------------------------------------
                                        // insere fechamento quando parcela zerar 
                                        // -------------------------------------------

                                        if (parcUpd.nu_parcela == 0)
                                        {
                                            totValor += (int)parc.vr_valor;

                                            db.Insert(new LOG_Fechamento
                                            {
                                                dt_compra = logTrans.dt_transacao,
                                                dt_fechamento = DateTime.Now,
                                                fk_cartao = parc.fk_cartao,
                                                fk_empresa = parc.fk_empresa,
                                                fk_loja = parc.fk_loja,
                                                fk_parcela = (int)parc.i_unique,
                                                nu_parcela = parc.nu_parcela,
                                                st_afiliada = "",
                                                st_ano = ano,
                                                st_mes = mes,
                                                vr_valor = parc.vr_valor
                                            });
                                        }
                                    }

                                    // ----------------------------
                                    // registra job / finalizado!
                                    // ----------------------------

                                    g_job.dt_fim = DateTime.Now;

                                    db.Update(g_job);

                                    db.Insert(new LOG_Audit
                                    {
                                        dt_operacao = DateTime.Now,
                                        fk_usuario = null,
                                        st_oper = "Fechamento [OK]",
                                        st_empresa = currentEmpresa,
                                        st_log = "Ano " + ano + " Mes " + mes + " Valor => " + totValor
                                    });
                                }

                                return Ok();
                            }
                            catch (SystemException ex)
                            {
                                db.Insert(new LOG_Audit
                                {
                                    dt_operacao = DateTime.Now,
                                    fk_usuario = null,
                                    st_oper = "Fechamento [ERRO]",
                                    st_empresa = currentEmpresa,
                                    st_log = ex.ToString()
                                });
                            }
                        }

                        #endregion

                        break;
                    }

                case "200": //gráficos
                    {
                        #region - code - 

                        var dtInicial = Request.GetQueryStringValue("dtInicial");
                        var dtFinal = Request.GetQueryStringValue("dtFinal");

                        DateTime? dt_inicial = ObtemData(dtInicial),
                                  dt_final = ObtemData(dtFinal)?.AddDays(1);

                        var strMonths = new List<string> { "", "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho",
                                                               "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro" };

                        var qAtual = db.DashboardGrafico.Where(y => y.dtDia > dt_inicial && y.dtDia < dt_final).ToList();

                        var res = new List<PointConvey>();
                        var resOld = new List<PointConvey>();
                        var ticks = new List<PointConveyLabel>();

                        int index = 1;
                        var dt = Convert.ToDateTime(dt_inicial);

                        while (dt < dt_final)
                        {
                            var qa = qAtual.FirstOrDefault(y => y.nuAno == dt.Year && y.nuMes == dt.Month && y.nuDia == dt.Day);

                            if (qa == null)
                            {
                                var lst = db.LOG_Transacoes.
                                            Where(y => y.tg_confirmada.ToString() == TipoConfirmacao.Confirmada).
                                            Where(y => y.dt_transacao > dt && y.dt_transacao < dt.AddDays(1)).
                                            ToList();

                                qa = new DashboardGrafico
                                {
                                    nuAno = dt.Year,
                                    nuMes = dt.Month,
                                    nuDia = dt.Day,
                                    totalTransacoes = lst.Count(),
                                    totalCartoes = lst.Select(y => y.fk_cartao).Distinct().Count(),
                                    totalFinanc = lst.Sum(y => (int)y.vr_total),
                                    totalLojas = lst.Select(y => y.fk_loja).Distinct().Count(),
                                    dtDia = dt
                                };

                                db.Insert(qa);
                            }

                            res.Add(new PointConvey
                            {
                                x = index,
                                y = qa != null ? qa.totalTransacoes : 0,
                            });

                            if (dt.Day == 15 || dt.Day == 1)
                            {
                                ticks.Add(new PointConveyLabel
                                {
                                    label = dt.Day + "/" + strMonths[dt.Month],
                                    x = index,
                                });
                            }

                            index++;
                            dt = dt.AddDays(1);
                        }

                        return Ok(new
                        {
                            list = res,
                            listOld = resOld,
                            ticks,
                            label_a = DateTime.Now.Year.ToString(),
                            label_b = DateTime.Now.AddYears(-1).Year.ToString(),
                        });
                        
                        #endregion
                    }

                case "300":
                    {
                        #region - code - 

                        var t = db.ConfigPlasticoEnvio.FirstOrDefault(y => y.id == 1);

                        if (t != null)
                        {
                            return Ok(t);
                        }
                        else
                        {
                            t = new ConfigPlasticoEnvio
                            {
                                bAtivo = false,
                                ter = true,
                                qui = true,
                                stHorario = "06:00",
                                stEmails = "printserv@printserv.com.br,Atendimento@conveynet.com.br",
                                stEmailSmtp = "conveynet@zohomail.com",
                                stSenhaSmtp = "Gustavo@2012",
                                stHostSmtp = "smtp.zoho.com",
                                nuPortSmtp = 587,
                            };

                            db.Insert(t);

                            return Ok(t);
                        }

                        #endregion
                    }

                case "301":
                    {
                        #region - code - 

                        var bAtivo = Request.GetQueryStringValue<bool?>("bAtivo");
                        var dom = Request.GetQueryStringValue<bool?>("dom");
                        var seg = Request.GetQueryStringValue<bool?>("seg");
                        var ter = Request.GetQueryStringValue<bool?>("ter");
                        var qua = Request.GetQueryStringValue<bool?>("qua");
                        var qui = Request.GetQueryStringValue<bool?>("qui");
                        var sex = Request.GetQueryStringValue<bool?>("sex");
                        var sab = Request.GetQueryStringValue<bool?>("sab");
                        var stHorario = Request.GetQueryStringValue("stHorario");
                        var stEmails = Request.GetQueryStringValue("stEmails");

                        var stEmailSmtp = Request.GetQueryStringValue("stEmailSmtp");
                        var stSenhaSmtp = Request.GetQueryStringValue("stSenhaSmtp");
                        var stHostSmtp = Request.GetQueryStringValue("stHostSmtp");
                        var nuPortSmtp = Request.GetQueryStringValue<int?>("nuPortSmtp");

                        var t = new ConfigPlasticoEnvio
                        {
                            id = 1,
                            bAtivo = bAtivo,
                            dom = dom,
                            seg = seg,
                            ter = ter,
                            qua = qua,
                            qui = qui,
                            sex= sex,
                            sab = sab,
                            stHorario = stHorario,
                            stEmails = stEmails,
                            stEmailSmtp = stEmailSmtp,
                            stSenhaSmtp = stSenhaSmtp,
                            stHostSmtp = stHostSmtp,
                            nuPortSmtp = nuPortSmtp,
                        };

                        db.Update(t);

                        return Ok();

                        #endregion
                    }

                case "400": // re-envio de arquivo de lote via email
                    {
                        #region - code - 

                        var idLote = Convert.ToInt32(Request.GetQueryStringValue("lote"));

                        var tEmp = db.T_Empresa.FirstOrDefault(y => y.i_unique == db.T_LoteCartaoDetalhe.Where(a => a.fk_lote == idLote).Select(b => b.fk_empresa).First());

                        var configPla = db.ConfigPlasticoEnvio.FirstOrDefault(y => y.id == 1);

                        var nomeArq = idLote + "_PEDIDO_PRODUCAO.txt";

                        var myPath = System.Web.Hosting.HostingEnvironment.MapPath("/") + "img\\" + nomeArq;

                        var cartList = db.T_LoteCartaoDetalhe.Where(y => y.fk_lote == idLote).ToList();

                        if (!File.Exists(myPath))
                        {
                            #region - refaz arquivo no servidor - 
                            
                            if (File.Exists(myPath))
                                File.Delete(myPath);

                            try
                            {
                                string total_file = "";

                                var nome = "";

                                foreach (var cart in cartList)
                                {
                                    var line = "";

                                    var _c = db.T_Cartao.FirstOrDefault(y => y.i_unique == cart.fk_cartao);

                                    if (cart.nu_titularidade.ToString().PadLeft(2,'0') == "01")
                                    {
                                        nome = db.T_Proprietario.FirstOrDefault(y => y.i_unique == _c.fk_dadosProprietario).st_nome;
                                    }
                                    else
                                    {
                                        nome = db.T_Dependente.FirstOrDefault(y => y.fk_proprietario == _c.fk_dadosProprietario &&
                                                                                    y.nu_titularidade == cart.nu_titularidade).st_nome;
                                    }

                                    line += nome.PadRight(30, ' ').Substring(0, 30).TrimEnd(' ') + ",";
                                    line += _c.st_empresa + ",";
                                    line += _c.st_matricula.ToString().PadLeft(6, '0') + ",";

                                    line += _c.st_venctoCartao.Substring(0, 2) + "/" +
                                            _c.st_venctoCartao.Substring(2, 2) + ",";

                                    line += calculaCodigoAcesso(_c.st_empresa,
                                                                    _c.st_matricula,
                                                                    _c.st_titularidade,
                                                                    cart.nu_via_original.ToString(),
                                                                    cart.nu_cpf) + ",";


                                    line += nome + ",|";

                                    line += "826766" + _c.st_empresa +
                                                            _c.st_matricula +
                                                            _c.st_titularidade +
                                                            cart.nu_via_original.ToString() +
                                                    "65" + _c.st_venctoCartao;

                                    line += "|";
                                    total_file += line;
                                }

                                configPla.stStatus = "3.9 - " + myPath + " - ";
                                db.Update(configPla);

                                File.WriteAllText(myPath, total_file);

                                configPla.stStatus = "3.91 - " + myPath + " - ";
                                db.Update(configPla);
                            }
                            catch (Exception ex1)
                            {
                                configPla.stStatus = "3.x " + ex1.ToString();
                                db.Update(configPla);
                                return Ok();
                            }

                            #endregion
                        }

                        // envia email

                        var textoEmail = "<p>Este é um arquivo gerado automaticamente - não responder.</p>" +
                                                     "<p>Arquivos para impressão de plástico:</p><p>";
                        
                        textoEmail += "<a href='https://meuconvey.conveynet.com.br/img/" + nomeArq +
                                        "' download target='_blank'>" + tEmp.st_fantasia.Trim() + 
                                        "_" + tEmp.st_empresa + "_" + 
                                        DateTime.Now.Day.ToString().PadLeft(2, '0') + "_" +
                                        DateTime.Now.Month.ToString().PadLeft(2, '0') + "_" + 
                                        DateTime.Now.Year.ToString() + 
                                        "_PEDIDO_PRODUCAO.txt</a>";

                        {
                            var myRelatName = "relat_envio_d" + idLote + ".txt";

                            myPath = System.Web.Hosting.HostingEnvironment.MapPath("/") + "img\\" + myRelatName;

                            if (File.Exists(myPath))
                                File.Delete(myPath);

                            using (var sw = new StreamWriter(myPath, false, Encoding.UTF8))
                            {
                                sw.WriteLine("CONVEY BENEFICIOS");
                                sw.WriteLine("RELATORIO DE ENVIO AUTOMATICO DE CARTÕES");
                                sw.WriteLine("DATA:" + DateTime.Now.Day.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Year.ToString() + " " + DateTime.Now.Hour.ToString().PadLeft(2, '0') + ":" + DateTime.Now.Minute.ToString().PadLeft(2, '0'));
                                sw.WriteLine("");

                                int iNumber = 1, tot_carts = 0;
                                 
                                sw.WriteLine("Empresa: " + tEmp.st_empresa + " - " + tEmp.st_fantasia);

                                foreach (var c in cartList)
                                {
                                    sw.WriteLine(iNumber++ + ") " + c.st_nome_cartao + " - " + tEmp.st_empresa + "." + c.nu_matricula);
                                    tot_carts++;
                                }

                                sw.WriteLine("Cartões na empresa: " + cartList.Count());
                                sw.WriteLine("");

                                sw.WriteLine("");
                                sw.WriteLine("Total de cartões enviados: " + tot_carts);
                                sw.WriteLine("");
                            }

                            textoEmail += "</p><p><a href='https://meuconvey.conveynet.com.br/img/" + myRelatName + 
                                                "' download target='_blank'>https://meuconvey.conveynet.com.br/img/" + myRelatName + "</a></p>";
                        }

                        textoEmail += "<p>&nbsp;</p><p>&nbsp;</p><p>CONVEYNET - " + DateTime.Now.Year + "</p>";

                        configPla.stStatus = "5";
                        db.Update(configPla);

                        if (SendEmail("Produção de Cartões", textoEmail, configPla.stEmails))
                        {
                            configPla.stStatus = "6";
                            db.Update(configPla);
                        }

                        return Ok();

                        #endregion
                    }
            }

            return BadRequest();            
        }
    }
}
