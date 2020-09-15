using System.Linq;
using System.Collections.Generic;
using System.Web.Http;

using SyCrafEngine;
using LinqToDB;
using System;
using DataModel;
using System.Net.Http;
using App.Web;
using WebGrease;

namespace DevKit.Web.Controllers
{
    // dto

    #region - cartão - 

    public class FechamentoVendaCartao
    {
        public string id,
                        idParcela,
                      nsu,
                      terminal,
                      lojista,
                      dtCompra,
                      parcela,
                      valor;

        public long _valor, _loja;
    }

    public class FechamentoListagemCartao
    {
        public int fkCartao;

        public string associado, 
                      matricula,
                      total;

        public List<FechamentoVendaCartao> vendas = new List<FechamentoVendaCartao>();
    }

    #endregion

    #region - loja -

    public class FechamentoVendaLoja
    {
        public string id,
                      nsu,
                      terminal,
                      associado,
                      matricula,
                      repasse,
                      dtCompra,
                      parcela,
                      valor;
    }

    public class FechamentoListagemLoja
    {
        public int fkLoja, _taxa;

        public string lojista,
                      bonus,
                      endereco,
                      sigla,
                      taxa,
                      total,

                      banco, agencia, conta, cnpj,

                      repasse;

        public List<FechamentoVendaLoja> vendas = new List<FechamentoVendaLoja>();
    }

    #endregion

    public class EmissoraFechamentoController : ApiControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        [Route("api/EmissoraFechamento/exportar", Name = "ExportarEmissoraFechamento")]
        public HttpResponseMessage ExportarEmpresaDBA()
        {
            Get(export: true);

            var x = new ExportMultiSheetWrapper("fechamento.xlsx");

            if (_tipoFech == "1") // associado
            {
                {
                    var abaString = "Simplificado";

                    x.NovaAba_Header(abaString, (new List<string>
                    {
                        "#",
                        "Associado",
                        "Período",
                        "Cartão",
                        "Valor Desconto Mensal R$",
                    }).
                    ToArray());

                    int t = 1;

                    foreach (var mdl in lstFechamentoCartoes)
                    {
                        x.AdicionarConteudo(abaString, (new List<string>
                        {
                            t.ToString(),
                            mdl.associado,
                            periodo,
                            mdl.matricula,
                            mdl.total,
                        }).
                        ToArray());

                        t++;
                    }

                    x.AdicionarConteudo(abaString, (new List<string>
                    {
                        "",
                        "TOTAL",
                        "",
                        "",
                        totalFechamento,
                    }).
                    ToArray());
                }
                {
                    var abaString = "Detalhado";

                    x.NovaAba_Header(abaString, (new List<string>
                    {
                        "","","","","","","","","","","","","","","","","",
                    }).
                    ToArray());

                    int t = 1;

                    foreach (var mdl in lstFechamentoCartoes)
                    {
                        x.AdicionarConteudo(abaString, (new List<string>
                        {
                            "Associado",
                            "Matricula",                            
                        }).
                        ToArray());

                        x.AdicionarConteudo(abaString, (new List<string>
                        {
                            mdl.associado,
                            mdl.matricula,
                        }).
                        ToArray());

                        x.AdicionarConteudo(abaString, (new List<string>
                        {
                            "#",
                            "Estabelecimento",
                            "NSU",
                            "Data compra",
                            "Valor parcela",
                            "Parcela",
                        }).
                        ToArray());

                        foreach (var item in mdl.vendas)
                        {
                            x.AdicionarConteudo(abaString, (new List<string>
                            {
                                t.ToString(),
                                item.lojista,
                                item.nsu,
                                item.dtCompra,
                                item.valor,
                                item.parcela,
                            }).
                            ToArray());

                            t++;
                        }

                        x.AdicionarConteudo(abaString, (new List<string>
                        {
                            "Total Cartão",
                            mdl.total,
                        }).
                        ToArray());
                    }
                    
                    x.AdicionarConteudo(abaString, (new List<string>
                    {
                        "",
                        "",
                        "",
                        "",
                        "",
                    }).
                    ToArray());

                    x.AdicionarConteudo(abaString, (new List<string>
                    {
                        "",
                        "TOTAL FINAL",
                        "",
                        "",
                        totalFechamento,
                    }).
                    ToArray());
                }
            }
            else if (_tipoFech == "2") // lojista
            {
                {
                    var abaString = "Simplificado";

                    x.NovaAba_Header(abaString, (new List<string>
                    {
                        "#",
                        "Lojista",
                        "Período",
                        "Valor Fechamento",
                        "Valor Bonificação",
                        "Valor Pagar",
                    }).
                    ToArray());

                    int t = 1;

                    foreach (var mdl in lstFechamentoLoja)
                    {
                        x.AdicionarConteudo(abaString, (new List<string>
                        {
                            t.ToString(),
                            mdl.lojista,
                            periodo,
                            mdl.total,
                            mdl.bonus,
                            mdl.repasse,
                        }).
                        ToArray());

                        t++;
                    }

                    x.AdicionarConteudo(abaString, (new List<string>
                    {
                        "",
                        "TOTAL",
                        "",
                        totalFechamento,
                        total_loja_bonus,
                        total_loja_rep,
                    }).
                    ToArray());
                }

                {
                    var abaString = "Detalhado";

                    x.NovaAba_Header(abaString, (new List<string>
                    {
                        "","","","","","","","","","","","","","","",                        

                    }).
                    ToArray());

                    int t = 1;

                    foreach (var mdl in lstFechamentoLoja)
                    {
                        x.AdicionarConteudo(abaString, (new List<string>
                        {
                            "LOJISTA",
                            "ENDERECO",
                        }).
                        ToArray());

                        x.AdicionarConteudo(abaString, (new List<string>
                        {
                            mdl.lojista + " - " + mdl.sigla,
                            mdl.endereco,
                        }).
                        ToArray());

                        x.AdicionarConteudo(abaString, (new List<string>
                        {
                            "#",
                            "ID",
                            "NSU",
                            "Associado",
                            "Cartão",
                            "Data Compra",
                            "Valor",
                            "Parcela",
                            "Terminal",
                        }).
                        ToArray());

                        foreach (var item in mdl.vendas)
                        {
                            x.AdicionarConteudo(abaString, (new List<string>
                            {
                                t.ToString(),
                                item.id,
                                item.nsu,
                                item.associado,
                                item.matricula,
                                item.dtCompra,
                                item.valor,
                                item.parcela,
                                item.terminal,
                            }).
                            ToArray());

                            t++;
                        }

                        x.AdicionarConteudo(abaString, (new List<string>
                        {
                            "Bonificação",
                            "Valor Total",
                            "Valor Bonificação",
                            "Valor Repasse",
                        }).
                        ToArray());

                        x.AdicionarConteudo(abaString, (new List<string>
                        {
                            mdl.taxa,
                            mdl.total,
                            mdl.bonus,
                            mdl.repasse,
                        }).
                        ToArray());
                    }

                    x.AdicionarConteudo(abaString, (new List<string>
                    {
                        "","","","","","","",
                    }).
                    ToArray());

                    x.AdicionarConteudo(abaString, (new List<string>
                    {
                        "",
                        "TOTAL",
                        "",
                        totalFechamento,
                        total_loja_bonus,
                        total_loja_rep,
                    }).
                    ToArray());
                }
            }

            return x.GeraXLS();
        }

        public List<FechamentoListagemCartao> lstFechamentoCartoes;
        public List<FechamentoListagemLoja> lstFechamentoLoja;

        public string periodo = "", totalFechamento = "", _tipoFech = "", total_loja_rep = "", total_loja_bonus = "";

        public IHttpActionResult Get(bool? export = false)
        {
            try
            {
                var tipoFech = Request.GetQueryStringValue("tipoFech");
                var detFech = Request.GetQueryStringValue("detFech");
                var mes = Request.GetQueryStringValue("mes").PadLeft(2, '0');
                var ano = Request.GetQueryStringValue("ano");
                var idEmpresa = Request.GetQueryStringValue<long?>("idEmpresa", null);

                _tipoFech = tipoFech;
                periodo = mes + "/" + ano;

                if (!StartDatabaseAndAuthorize())
                    return BadRequest();

                var tEmp = db.currentEmpresa;

                if (idEmpresa != null)
                {
                    tEmp = db.T_Empresa.FirstOrDefault(y => y.i_unique == idEmpresa);

                    if (tEmp == null)
                        return BadRequest();
                }

                var m = new money();

                switch (tipoFech)
                {
                    case "1": // cartao

                        #region - code - 

                        var fech = (from e in db.LOG_Fechamento
                                    where e.fk_empresa == tEmp.i_unique && e.st_mes == mes && e.st_ano == ano
                                    select e).
                                    ToList();

                        var ids_lojas = fech.Select(a => (long)a.fk_loja).Distinct().ToList();
                        var ids_parcelas = fech.Select(a => (long)a.fk_parcela).Distinct().ToList();
                        var ids_cartoes = fech.Select(a => (long)a.fk_cartao).Distinct().ToList();

                        var lstParc = new List<T_Parcela>();

                        {
                            var lst_tmp_ids_parcelas = new List<long>();

                            lst_tmp_ids_parcelas = ids_parcelas.Take(99999999).ToList();

                            while (true)
                            {
                                var lst_ids = lst_tmp_ids_parcelas.Take(500).ToList();

                                lstParc.AddRange((from e in db.T_Parcelas where lst_ids.Contains((long)e.i_unique) select e).ToList());

                                foreach (var item in lst_ids)
                                    lst_tmp_ids_parcelas.Remove(item);

                                lst_ids.Clear();

                                if (lst_tmp_ids_parcelas.Count() == 0)
                                    break;
                            }
                        }

                        var parcelas = new List<FechamentoVendaCartao>();

                        foreach (var _parc in ids_parcelas)
                        {
                            var parc = lstParc.FirstOrDefault(y => y.i_unique == _parc);

                            parcelas.Add(new FechamentoVendaCartao
                            {
                                dtCompra = Convert.ToDateTime(parc.dt_inclusao).ToString("dd/MM/yyyy HH:mm"),
                                nsu = parc.nu_nsu.ToString(),
                                parcela = parc.nu_indice + " / " + parc.nu_tot_parcelas,
                                idParcela = parc.i_unique.ToString(),
                                id = parc.fk_cartao.ToString(),
                                valor = m.setMoneyFormat((long)parc.vr_valor),
                                _valor = (long)parc.vr_valor,
                                _loja = (long)parc.fk_loja
                            });
                        }

                        var lojas = db.T_Loja.Where(y => ids_lojas.Contains((long)y.i_unique)).ToList();

                        var cartoes = (from cart in db.T_Cartao
                                       join prop in db.T_Proprietario on cart.fk_dadosProprietario equals (int)prop.i_unique
                                       where ids_cartoes.Contains((long)cart.i_unique)
                                       select new FechamentoListagemCartao
                                       {
                                           fkCartao = (int)cart.i_unique,
                                           associado = prop.st_nome,
                                           matricula = cart.st_matricula,
                                       }).
                                   OrderBy(u => u.associado).
                                   ToList();
                                                
                        foreach (var item in cartoes)
                        {
                            item.vendas = parcelas.Where(y => y.id == item.fkCartao.ToString()).OrderBy(y => y.dtCompra).ToList();

                            foreach (var i in item.vendas)
                            {
                                var f = lojas.FirstOrDefault(y => y.i_unique == i._loja);
                                i.lojista = f.st_loja + " - " + f.st_nome;
                            }

                            item.total = m.setMoneyFormat(item.vendas.Sum(y => y._valor));
                        }

                        totalFechamento = m.formatToMoney(parcelas.Sum(y => y._valor).ToString());

                        if (export == true)
                        {
                            lstFechamentoCartoes = cartoes.ToList();
                            return Ok();
                        }                        

                        return Ok(new
                        {
                            count = cartoes.Count(),
                            totalFechamento,
                            totalCartoes = cartoes.Count(),
                            convenio = tEmp.st_fantasia + " (" + tEmp.st_empresa.TrimStart('0') + ")",
                            results = cartoes
                        });
                        
                        #endregion

                    case "2": // lojista

                        #region - code - 

                        var itensFechamento = (from e in db.LOG_Fechamento
                                               where e.fk_empresa == tEmp.i_unique
                                               where e.st_mes == mes
                                               where e.st_ano == ano
                                               select e).
                                              ToList();

                        var lstIDsParcelas = (from e in itensFechamento select (long)e.fk_parcela).Distinct().ToList();

                        var lstParcelas = new List<T_Parcela>();

                        {
                            var lst_tmp_ids_parcelas = new List<long>();

                            lst_tmp_ids_parcelas = lstIDsParcelas.Take(99999999).ToList();

                            while (true)
                            {
                                var lst_ids = lst_tmp_ids_parcelas.Take(500).ToList();

                                lstParcelas.AddRange((from e in db.T_Parcelas where lst_ids.Contains((long)e.i_unique) select e).ToList());

                                foreach (var item in lst_ids)
                                    lst_tmp_ids_parcelas.Remove(item);

                                lst_ids.Clear();

                                if (lst_tmp_ids_parcelas.Count() == 0)
                                    break;
                            }
                        }

                        var lstIDsCartoes = (from e in itensFechamento select e.fk_cartao).Distinct().ToList();

                        var lstCartoes = (from e in db.T_Cartao
                                          where lstIDsCartoes.Contains((int)e.i_unique)
                                          select e).
                                          ToList();

                        var lstIdsTerminais = lstParcelas.Select(y => y.fk_terminal).Distinct().ToList();

                        var lstTerminais = (from e in db.T_Terminal
                                            where lstIdsTerminais.Contains((int)e.i_unique)
                                            select e).
                                          ToList();

                        var lstIDsProps = (from e in lstCartoes select e.fk_dadosProprietario).Distinct().ToList();

                        var lstProps = (from e in db.T_Proprietario
                                        where lstIDsProps.Contains((int)e.i_unique)
                                        select e).
                                          ToList();


                        var lstIDsLojistas = (from e in itensFechamento select e.fk_loja).Distinct().ToList();

                        var lstLojistas = (from e in db.T_Loja
                                           where lstIDsLojistas.Contains((int)e.i_unique)
                                           orderby e.st_nome
                                           select e).
                                          ToList();

                        var lstConvenios = (from e in db.LINK_LojaEmpresa
                                            where e.fk_empresa == tEmp.i_unique
                                            select e).
                                            ToList();

                        var lst = new List<FechamentoListagemLoja>();

                        // ---------------------------------------
                        // ## iterar por todos os lojistas
                        // ---------------------------------------

                        var bancoEnum = new EnumBancos();

                        foreach (var tLoj in lstLojistas)
                        {
                            var tr = (from e in lstConvenios
                                      where e.fk_loja == tLoj.i_unique
                                      select (int)e.tx_admin).
                                      FirstOrDefault();

                            lst.Add(new FechamentoListagemLoja
                            {
                                fkLoja = (int)tLoj.i_unique,
                                lojista = tLoj.st_nome,
                                endereco = tLoj.st_endereco,
                                sigla = tLoj.st_loja,
                                _taxa = tr,
                                banco = tLoj.fk_banco != null ? bancoEnum.Get((int)tLoj.fk_banco).stName : "",
                                agencia = tLoj.st_agencia,
                                conta = tLoj.st_conta,
                                cnpj = tLoj.nu_CNPJ,
                            });
                        }

                        long vrTotalFechamento = 0,
                             vrTotalRepasse = 0,
                             vrTotalBonus = 0;

                        foreach (var item in lst)
                        {
                            // -------------------------------------------
                            // ## iterar por todas as vendas da loja
                            // -------------------------------------------

                            var lstParcsFechamento = itensFechamento.
                                                     Where(y => y.fk_loja == item.fkLoja).
                                                     ToList();

                            long vrTotal = 0, vrRepasse = 0, vrBonus = 0, _id = 0;

                            foreach (var vendas in lstParcsFechamento)
                            {
                                var parc = lstParcelas.
                                           Where(y => y.i_unique == vendas.fk_parcela).
                                           FirstOrDefault();

                                var cart = lstCartoes.
                                           Where(y => y.i_unique == vendas.fk_cartao).
                                           FirstOrDefault();

                                var prop = lstProps.
                                           Where(y => y.i_unique == cart.fk_dadosProprietario).
                                           FirstOrDefault();

                                var term = lstTerminais.
                                           Where(y => y.i_unique == parc.fk_terminal).
                                           FirstOrDefault();

                                vrTotal += (long)vendas.vr_valor;

                                var rep = (long)vendas.vr_valor -
                                            (long)(vendas.vr_valor * item._taxa / 10000);

                                vrRepasse += rep;

                                vrBonus += (long)vendas.vr_valor - rep;

                                item.vendas.Add(new FechamentoVendaLoja
                                {
                                    id = (++_id).ToString(),
                                    nsu = parc.nu_nsu.ToString(),
                                    associado = prop.st_nome,
                                    matricula = cart.st_matricula + "." + cart.st_titularidade.PadLeft(2, '0'),
                                    dtCompra = ObtemData(parc.dt_inclusao),
                                    parcela = parc.nu_indice + " / " + parc.nu_tot_parcelas,
                                    valor = m.setMoneyFormat((int)parc.vr_valor),
                                    terminal = term.nu_terminal
                                });
                            }

                            vrTotalFechamento += vrTotal;
                            vrTotalRepasse += vrRepasse;
                            vrTotalBonus += vrBonus;

                            item.taxa = m.setMoneyFormat(item._taxa) + "%";
                            item.total = m.setMoneyFormat((int)vrTotal);
                            item.repasse = m.setMoneyFormat((int)vrRepasse);
                            item.bonus = m.setMoneyFormat((int)vrBonus);
                        }

                        lstFechamentoLoja = lst;

                        total_loja_rep = m.setMoneyFormat((int)vrTotalRepasse);
                        total_loja_bonus = m.setMoneyFormat((int)vrTotalBonus);
                        totalFechamento = m.setMoneyFormat((int)vrTotalFechamento);

                        if (export == true)
                        {
                            return Ok();
                        }

                        return Ok(new
                        {
                            count = lst.Count(),
                            totalFechamento = totalFechamento,
                            totalRepasse = total_loja_rep,
                            totalBonus = total_loja_bonus,
                            totalLojistas = lstLojistas.Count(),
                            convenio = tEmp.st_fantasia + " (" + tEmp.st_empresa.TrimStart('0') + ")",
                            results = lst
                        });

                        #endregion
                }

                return BadRequest();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}
