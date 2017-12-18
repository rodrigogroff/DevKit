using System.Linq;
using System.Web.Http;

using SyCrafEngine;
using LinqToDB;
using System;
using System.Collections.Generic;

namespace DevKit.Web.Controllers
{
    public class RelExtratoEncerrado
    {
        public string dtVenda,
                        nsu,
                        valor,
                        parcela,
                        fornecedor;
    }
    
    public class EmissoraRelExtratosController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var tipo = Request.GetQueryStringValue("tipo");            
            var mat = Request.GetQueryStringValue("mat").PadLeft(6,'0');

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mon = new money();

            switch (tipo)
            {
                // ------------------------
                // extratos fechados
                // ------------------------

                case "1": 
                    {
                        var mes = Request.GetQueryStringValue("mes").PadLeft(2, '0');
                        var ano = Request.GetQueryStringValue("ano");

                        var cartao = (from e in db.T_Cartao
                                      where e.st_empresa == db.currentEmpresa.st_empresa
                                      where e.st_matricula == mat
                                      select e).
                                      FirstOrDefault();

                        var associado = (from e in db.T_Cartao
                                         join prop in db.T_Proprietario on e.fk_dadosProprietario equals (int) prop.i_unique
                                         where e.st_empresa == db.currentEmpresa.st_empresa
                                         where e.st_matricula == mat
                                         select prop).
                                         FirstOrDefault();

                        var lstFechamento = (from e in db.LOG_Fechamento 
                                             join cart in db.T_Cartao on e.fk_cartao equals (int) cart.i_unique 
                                             where e.fk_empresa == db.currentEmpresa.i_unique 
                                             where cart.st_matricula == mat 
                                             where e.st_mes == mes
                                             where e.st_ano == ano
                                             select e).
                                             ToList();

                        var lstIdLojista = lstFechamento.Select(y => y.fk_loja).Distinct().ToList();
                        var lstLojistas = (from e in db.T_Loja
                                           where lstIdLojista.Contains((int)e.i_unique)
                                           select e).
                                           ToList();

                        var lstIdParcela = lstFechamento.Select(y => y.fk_parcela).Distinct().ToList();
                        var lstParcelas = (from e in db.T_Parcelas
                                           where lstIdParcela.Contains((int)e.i_unique)
                                           select e).
                                           ToList();

                        var lst = new List<RelExtratoEncerrado>();

                        long tot = 0;

                        foreach (var item in lstFechamento)
                        {
                            var parc = lstParcelas.Where(y => y.i_unique == item.fk_parcela).FirstOrDefault();

                            lst.Add(new RelExtratoEncerrado
                            {                                
                                dtVenda = ObtemData(item.dt_compra),
                                fornecedor = lstLojistas.Where(y => y.i_unique == item.fk_loja).FirstOrDefault().st_nome,
                                nsu = parc.nu_nsu.ToString(),
                                parcela = parc.nu_indice + "/" + parc.nu_tot_parcelas.ToString(),
                                valor = mon.setMoneyFormat((long)parc.vr_valor),
                            });

                            tot += (long) parc.vr_valor;
                        }

                        return Ok(new
                        {
                            count = lst.Count(),
                            results = lst,
                            total = mon.setMoneyFormat(tot),
                            associado = associado.st_nome,
                            cartao = cartao.st_empresa + "." + cartao.st_matricula,
                            cpf = associado.st_cpf,                            
                            dtEmissao = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                        });
                    }

                case "2":
                    {
                        break;
                    }

                case "3":
                    {
                        break;
                    }
            }

            return BadRequest();
        }
    }
}
