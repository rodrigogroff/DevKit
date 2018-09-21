using System.Linq;
using System.Collections.Generic;
using System.Web.Http;

using SyCrafEngine;
using LinqToDB;
using System;

namespace DevKit.Web.Controllers
{
    // dto

    #region - cartão - 

    public class FechamentoVendaCartao
    {
        public string id,
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
                      repasse;

        public List<FechamentoVendaLoja> vendas = new List<FechamentoVendaLoja>();
    }

    #endregion

    public class EmissoraFechamentoController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var tipoFech = Request.GetQueryStringValue("tipoFech");
            var mes = Request.GetQueryStringValue("mes").PadLeft(2, '0');
            var ano = Request.GetQueryStringValue("ano");
            var idEmpresa = Request.GetQueryStringValue<long?>("idEmpresa",null);

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

            if (tipoFech == "1")
            {
                #region - code - 

                var fech = (from e in db.LOG_Fechamento
                            where e.fk_empresa == tEmp.i_unique && e.st_mes == mes && e.st_ano == ano
                            select new  { e.fk_loja, e.fk_parcela, e.fk_cartao }).
                            ToList();

                var ids_lojas = fech.Select(a => (long)a.fk_loja).Distinct().ToList();
                var ids_parcelas = fech.Select(a => (long)a.fk_parcela).Distinct().ToList();
                var ids_cartoes = fech.Select(a => (long)a.fk_cartao).Distinct().ToList();

                var parcelas = (from parc in db.T_Parcelas
                                       where ids_parcelas.Contains((long)parc.i_unique)
                                       select new FechamentoVendaCartao
                                       {
                                           dtCompra = Convert.ToDateTime(parc.dt_inclusao).ToString("dd/MM/yyyy HH:mm"),
                                           nsu = parc.nu_nsu.ToString(),
                                           parcela = parc.nu_indice + " / " + parc.nu_tot_parcelas,
                                           id = parc.fk_cartao.ToString(),
                                           valor = m.setMoneyFormat((long) parc.vr_valor),
                                           _valor = (long)parc.vr_valor,
                                           _loja = (long)parc.fk_loja
                                       }).
                                       ToList();

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

                    item.total = m.setMoneyFormat(item.vendas.Sum ( y=> y._valor));
                }

                return Ok(new
                {
                    count = cartoes.Count(),
                    totalFechamento = m.formatToMoney(parcelas.Sum(y=> y._valor).ToString()),
                    totalCartoes = cartoes.Count(),
                    convenio = tEmp.st_fantasia + " (" + tEmp.st_empresa.TrimStart('0') + ")",
                    results = cartoes
                });

                #endregion
            }
            else if (tipoFech == "2")
            {
                #region - code - 

                var itensFechamento = (from e in db.LOG_Fechamento
                                       where e.fk_empresa == tEmp.i_unique
                                       where e.st_mes == mes
                                       where e.st_ano == ano
                                       select e).
                                      ToList();

                #region - indexamento em memoria -

                #region - cartoes -

                var lstIDsCarts = (from e in itensFechamento
                                   join cart in db.T_Cartao on e.fk_cartao equals (int)cart.i_unique
                                   select cart.i_unique).
                                   ToList();

                var lstCartoes = (from e in db.T_Cartao
                                  where lstIDsCarts.Contains(e.i_unique)
                                  select e).
                                  ToList();

                #endregion

                #region - proprietarios -

                var lstIDsProps = (from e in itensFechamento
                                   join cart in db.T_Cartao on e.fk_cartao equals (int)cart.i_unique
                                   join prop in db.T_Proprietario on cart.fk_dadosProprietario equals (int)prop.i_unique
                                   select prop.i_unique).
                                   ToList();

                var lstProprietarios = (from e in db.T_Proprietario
                                        where lstIDsProps.Contains(e.i_unique)
                                        select e).
                                        ToList();

                #endregion

                #region - parcelas & terminais -

                var lstIDsParcelas = (from e in itensFechamento
                                      join parc in db.T_Parcelas on e.fk_parcela equals (int)parc.i_unique
                                      select parc.i_unique).
                                       ToList();

                var lstParcelas = (from e in db.T_Parcelas
                                   where lstIDsParcelas.Contains(e.i_unique)
                                   select e).
                                  ToList();

                var lstIdsTerminais = lstParcelas.Select(y => y.fk_terminal).ToList();

                var lstTerminais = (from e in db.T_Terminal
                                   where lstIdsTerminais.Contains((int)e.i_unique)
                                   select e).
                                  ToList();

                #endregion

                #region - lojistas & convenios -

                var lstIDsLojistas = (from e in itensFechamento
                                      join loja in db.T_Loja on e.fk_loja equals (int)loja.i_unique
                                      select loja.i_unique).
                                      ToList();

                var lstLojistas = (from e in db.T_Loja
                                   where lstIDsLojistas.Contains(e.i_unique)
                                   orderby e.st_nome
                                   select e).
                                  ToList();

                var lstConvenios = (from e in db.LINK_LojaEmpresa
                                    where e.fk_empresa == tEmp.i_unique
                                    select e).
                                    ToList();

                #endregion

                #endregion

                var lst = new List<FechamentoListagemLoja>();

                // ---------------------------------------
                // ## iterar por todos os lojistas
                // ---------------------------------------

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
                        _taxa = tr
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

                        var prop = lstProprietarios.
                                   Where(y => y.i_unique == cart.fk_dadosProprietario).
                                   FirstOrDefault();

                        var term = lstTerminais.
                                   Where(y => y.i_unique == parc.fk_terminal).
                                   FirstOrDefault();

                        vrTotal += (long)vendas.vr_valor;

                        var rep   = (long) vendas.vr_valor - 
                                    (long)(vendas.vr_valor * item._taxa / 10000);

                        vrRepasse += rep;

                        vrBonus += (long)vendas.vr_valor - rep;

                        item.vendas.Add(new FechamentoVendaLoja
                        {
                            id = (++_id).ToString(),
                            nsu = parc.nu_nsu.ToString(),
                            associado = prop.st_nome,
                            matricula = cart.st_matricula + "." + cart.st_titularidade.PadLeft(2,'0'),
                            dtCompra = ObtemData(parc.dt_inclusao),
                            parcela = parc.nu_indice + " / " + parc.nu_tot_parcelas,
                            valor = m.setMoneyFormat((int)parc.vr_valor),
                            //repasse = m.setMoneyFormat((int)rep),
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

                return Ok(new
                {
                    count = lst.Count(),
                    totalFechamento = m.setMoneyFormat((int)vrTotalFechamento),
                    totalRepasse = m.setMoneyFormat((int)vrTotalRepasse),
                    totalBonus = m.setMoneyFormat((int)vrTotalBonus),
                    totalLojistas = lstLojistas.Count(),
                    convenio = tEmp.st_fantasia + " (" + tEmp.st_empresa.TrimStart('0') + ")",
                    results = lst
                });

                #endregion
            }

            return BadRequest();
        }
    }
}
