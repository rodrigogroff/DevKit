using System.Linq;
using System.Collections.Generic;
using System.Web.Http;

using SyCrafEngine;
using LinqToDB;

namespace DevKit.Web.Controllers
{
    public class FechamentoVenda
    {
        public string lojista,
                      dtCompra,
                      parcela,
                      valor;
    }
    
    public class FechamentoListagemCartao
    {
        public int fkCartao;

        public string associado, 
                      matricula,
                      total;

        public List<FechamentoVenda> vendas = new List<FechamentoVenda>();
    }

    public class EmissoraFechamentoController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var tipoFech = Request.GetQueryStringValue("tipoFech");
            var mes = Request.GetQueryStringValue("mes").PadLeft(2, '0');
            var ano = Request.GetQueryStringValue("ano"); 

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var m = new money();

            if (tipoFech == "1")
            {
                var itensFechamento = (from e in db.LOG_Fechamento
                                       where e.fk_empresa == db.currentEmpresa.i_unique
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

                #region - parcelas -

                var lstIDsParcelas = (from e in itensFechamento
                                       join parc in db.T_Parcelas on e.fk_parcela equals (int)parc.i_unique
                                       select parc.i_unique).
                                       ToList();

                var lstParcelas = (from e in db.T_Parcelas
                                  where lstIDsParcelas.Contains(e.i_unique)
                                  select e).
                                  ToList();

                #endregion

                #region - lojistas -

                var lstIDsLojistas = (from e in itensFechamento
                                      join loja in db.T_Loja on e.fk_loja equals (int)loja.i_unique
                                      select loja.i_unique).
                                      ToList();

                var lstLojistas = (from e in db.T_Loja
                                   where lstIDsLojistas.Contains(e.i_unique)
                                   select e).
                                  ToList();

                #endregion

                #endregion

                var lst = new List<FechamentoListagemCartao>();

                // ---------------------------------------
                // ## iterar por todos os associados
                // ---------------------------------------

                var dList = itensFechamento.
                            Select(y => y.fk_cartao).
                            Distinct().
                            ToList();

                var finalCartoes = (from e in db.T_Cartao
                                    join prop in db.T_Proprietario on e.fk_dadosProprietario equals (int)prop.i_unique
                                    where dList.Contains((int)e.i_unique)
                                    orderby prop.st_nome
                                    select e.i_unique).
                                    ToList();

                foreach (var fkCartao in finalCartoes)
                {
                    var cart = lstCartoes.
                                Where(y => y.i_unique == fkCartao).
                                FirstOrDefault();

                    var prop = lstProprietarios.
                                Where (y=> y.i_unique == cart.fk_dadosProprietario).
                                FirstOrDefault();

                    lst.Add(new FechamentoListagemCartao
                    {
                        fkCartao = (int)cart.i_unique,
                        matricula = cart.st_matricula,
                        associado = prop.st_nome
                    });
                }

                long vrTotalFechamento = 0;
                
                foreach (var item in lst)
                {
                    // -------------------------------------------
                    // ## iterar por todas as vendas do associado
                    // -------------------------------------------

                    var lstParcsFechamento = itensFechamento.
                                             Where(y => y.fk_cartao == item.fkCartao).
                                             ToList();

                    long vrTotal = 0;

                    foreach (var vendas in lstParcsFechamento)
                    {
                        var parc = lstParcelas.
                                   Where(y => y.i_unique == vendas.fk_parcela).
                                   FirstOrDefault();

                        var loja = lstLojistas.
                                   Where(y => y.i_unique == vendas.fk_loja).
                                   FirstOrDefault();

                        vrTotal += (long) vendas.vr_valor;

                        item.vendas.Add(new FechamentoVenda
                        {
                            lojista = loja.st_nome,
                            dtCompra = ObtemData(parc.dt_inclusao),
                            parcela = parc.nu_indice + " / " + parc.nu_tot_parcelas,
                            valor = m.setMoneyFormat((int)parc.vr_valor)
                        });
                    }

                    vrTotalFechamento += vrTotal;

                    item.total = m.setMoneyFormat((int)vrTotal);
                }

                return Ok(new
                {
                    count = lst.Count(),
                    totalFechamento = m.setMoneyFormat((int)vrTotalFechamento),
                    results = lst
                });
            }
            else if (tipoFech == "2")
            {

            }        

            return BadRequest();
        }
    }
}
