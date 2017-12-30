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

    public class RelExtratoFutResumido
    {
        public string mesAno,
                        compras,
                        total,
                        limite,
                        comprometido,
                        disponivel;
    }
        
    public class EmissoraRelExtratosController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var tipo = Request.GetQueryStringValue("tipo");            
            var mat = Request.GetQueryStringValue("mat").PadLeft(6,'0');

            var meses = ",Janeiro,Fevereiro,Março,Abril,Maio,Junho,Julho,Agosto,Setembro,Outubro,Novembro,Dezembro".Split(',');

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

                        var associado = (from e in db.T_Proprietario
                                         where cartao.fk_dadosProprietario == e.i_unique
                                         select e).
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
                        // -------------------------
                        // fatura atual
                        // -------------------------

                        var cartao = (from e in db.T_Cartao
                                      where e.st_empresa == db.currentEmpresa.st_empresa
                                      where e.st_matricula == mat
                                      select e).
                                      FirstOrDefault();
                        
                        var diaFech = (from e in db.I_Scheduler
                                          where e.st_job.StartsWith("schedule_fech_mensal;empresa;" + db.currentEmpresa.st_empresa)
                                          select e).
                                          FirstOrDefault().
                                          nu_monthly_day;

                        long tot = 0, dispMensal = 0, dispTot = 0;

                        new SaldoDisponivel().Obter(db, cartao, ref dispMensal, ref dispTot);

                        var associado = (from e in db.T_Proprietario
                                         where cartao.fk_dadosProprietario == e.i_unique
                                         select e).
                                         FirstOrDefault();

                        var lstParcelasAtuais = (from e in db.T_Parcelas
                                                  where e.fk_cartao == cartao.i_unique
                                                  where e.nu_parcela == 1
                                                  orderby e.nu_parcela, e.dt_inclusao
                                                  select e).
                                                  ToList();
                        
                        var lst = new List<RelExtratoEncerrado>();                        

                        var lstIdLojista = lstParcelasAtuais.Select(y => y.fk_loja).Distinct().ToList();
                        var lstLojistas = (from e in db.T_Loja
                                            where lstIdLojista.Contains((int)e.i_unique)
                                            select e).
                                            ToList();

                        var dtNow = DateTime.Now;

                        if (dtNow.Day >= diaFech)
                            dtNow = dtNow.AddMonths(1);

                        foreach (var item in lstParcelasAtuais)
                        {
                            lst.Add(new RelExtratoEncerrado
                            {
                                dtVenda = ObtemData(item.dt_inclusao),
                                fornecedor = lstLojistas.Where(y => y.i_unique == item.fk_loja).FirstOrDefault().st_nome,
                                nsu = item.nu_nsu.ToString(),
                                parcela = item.nu_indice + "/" + item.nu_tot_parcelas.ToString(),
                                valor = mon.setMoneyFormat((long)item.vr_valor),
                            });

                            tot += (long)item.vr_valor;
                        }

                        return Ok(new
                        {
                            count = lst.Count(),
                            results = lst,
                            mesAtual = meses[dtNow.Month] + " / "+ dtNow.Year,
                            total = mon.setMoneyFormat(tot),
                            saldo = mon.setMoneyFormat(dispMensal),
                            associado = associado.st_nome,
                            cartao = cartao.st_empresa + "." + cartao.st_matricula,
                            cpf = associado.st_cpf,
                            dtEmissao = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                        });
                    }

                case "3":
                    {
                        var tipoFut = Request.GetQueryStringValue("tipoFut");
                        
                        var cartao = (from e in db.T_Cartao
                                      where e.st_empresa == db.currentEmpresa.st_empresa
                                      where e.st_matricula == mat
                                      select e).
                                      FirstOrDefault();

                        var associado = (from e in db.T_Proprietario
                                         where cartao.fk_dadosProprietario == e.i_unique
                                         select e).
                                         FirstOrDefault();

                        var lstParcelasFuturas = (from e in db.T_Parcelas
                                                  where e.fk_cartao == cartao.i_unique
                                                  where e.nu_parcela >= 1
                                                  orderby e.nu_parcela, e.dt_inclusao
                                                  select e).
                                                  ToList();

                        var diaFech = (from e in db.I_Scheduler
                                       where e.st_job.StartsWith("schedule_fech_mensal;empresa;" + db.currentEmpresa.st_empresa)
                                       select e).
                                          FirstOrDefault().
                                          nu_monthly_day;

                        if (tipoFut == "1") // resumido
                        {
                            var dtNow = DateTime.Now.AddMonths(1);

                            if (dtNow.Day > diaFech)
                                dtNow = dtNow.AddMonths(1);

                            var lst = new List<RelExtratoFutResumido>();
                            
                            var parcs = lstParcelasFuturas.
                                        Select(y => y.nu_parcela).
                                        ToList().
                                        Distinct().
                                        OrderBy(a => a.Value);
                            
                            foreach (var item in parcs)
                            {
                                var tot = (long)lstParcelasFuturas.
                                            Where(y => y.nu_parcela == item).
                                            Select(y => y.vr_valor).
                                            Sum();

                                lst.Add(new RelExtratoFutResumido
                                {
                                    mesAno = meses[dtNow.Month] + " / " + dtNow.Year,

                                    compras = lstParcelasFuturas.
                                              Where(y => y.nu_parcela == item).
                                              Count().
                                              ToString(),

                                    total = "R$ " + mon.setMoneyFormat(tot),
                                    limite = "R$ " + mon.setMoneyFormat((long)cartao.vr_limiteMensal),
                                    disponivel = "R$ " + mon.setMoneyFormat((long)cartao.vr_limiteMensal - tot),
                                    comprometido = mon.setMoneyFormat( 10000 * tot / (long)cartao.vr_limiteMensal) + "%",
                                });

                                dtNow = dtNow.AddMonths(1);                                
                            }

                            return Ok(new
                            {
                                count = lst.Count(),
                                results = lst,                                
                                associado = associado.st_nome,
                                cartao = cartao.st_empresa + "." + cartao.st_matricula,
                                cpf = associado.st_cpf,
                                dtEmissao = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                            });
                        }
                        else                        
                        {
                            // -------------------------
                            // detalhado
                            // -------------------------

                            var mes = Request.GetQueryStringValue("mes");
                            var ano = Request.GetQueryStringValue("ano");

                            if (DateTime.Now > new DateTime(Convert.ToInt32(ano), Convert.ToInt32(mes), 1))
                                // passado
                                return BadRequest();

                            var dtNow = DateTime.Now;
                            var lst = new List<RelExtratoEncerrado>();

                            var parc = 0;

                            while (true)
                            {
                                dtNow = dtNow.AddMonths(1);
                                parc++;

                                if (dtNow.Month.ToString() == mes && dtNow.Year.ToString() == ano)
                                    break;
                            }

                            var lstParcsSel = lstParcelasFuturas.Where(y => y.nu_parcela == parc).ToList();
                            
                            var lstIdLojista = lstParcsSel.Select(y => y.fk_loja).Distinct().ToList();
                            var lstLojistas = (from e in db.T_Loja
                                               where lstIdLojista.Contains((int)e.i_unique)
                                               select e).
                                               ToList();
                            
                            long tot = 0;

                            foreach (var item in lstParcsSel)
                            {
                                lst.Add(new RelExtratoEncerrado
                                {
                                    dtVenda = ObtemData(item.dt_inclusao),
                                    fornecedor = lstLojistas.Where(y => y.i_unique == item.fk_loja).FirstOrDefault().st_nome,
                                    nsu = item.nu_nsu.ToString(),
                                    parcela = item.nu_indice + "/" + item.nu_tot_parcelas.ToString(),
                                    valor = mon.setMoneyFormat((long)item.vr_valor),
                                });

                                tot += (long)item.vr_valor;
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
                    }
            }

            return BadRequest();
        }
    }
}
