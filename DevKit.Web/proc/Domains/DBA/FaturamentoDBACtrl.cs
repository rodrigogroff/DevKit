using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using System;
using SyCrafEngine;
using App.Web;

namespace DevKit.Web.Controllers
{
    public class FaturamentoDTOItem
    {
        public string item { get; set; }
    }

    public class FaturamentoDTO
    {
        public string codigo { get; set; }
        public string nome { get; set; }
        public string social { get; set; }
        public string total { get; set; }
        public string dia_venc { get; set; }

        public int _total { get; set; }

        public List<FaturamentoDTOItem> detalhes { get; set; }
    }

    public class FaturamentoDBAController : ApiControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        [Route("api/FaturamentoDBA/exportar", Name = "ExportarFaturamentoLojas")]
        public IHttpActionResult Exportar()
        {
            return Get(exportar: true);
        }

        [NonAction]
        private IHttpActionResult Export(List<FaturamentoDTO> query, int ano, int mes)
        {
            var myXLSWrapper = new ExportWrapper("Export_Faturamento_Lojas_ " + mes.ToString().PadLeft(2,'0') + "_" + ano + ".xlsx",
                                                   "Faturamento",
                                                   new string[] {   "INTEGRACAO",
                                                                    "Cliente-ou-Fornecedor",
                                                                    "Categoria",
                                                                    "Forma-de-Pagamento",
                                                                    "Vencimento",
                                                                    "Valor-Vencimento",
                                                                    "Tipo",
                                                                    "Descrição",
                                                                    "Marca", });

            //INTEGRACAO	Cliente-ou-Fornecedor	Categoria	Forma-de-Pagamento	Vencimento	Valor-Vencimento	Tipo	Descricao	Marca
            //4060	DROGARIA FARMANELLI LTDA 3	EM COBRANCA	BOLETO BANCARIO	16/05/2019	412,96	RECEITA	EM ATRASO	SICREDI

            foreach (var item in query)
            {
                myXLSWrapper.AddContents(new string[]
                {
                    item.codigo,
                    item.social,
                    "EM COBRANCA",
                    "BOLETO BANCARIO",
                    item.dia_venc,
                    item.total.Substring (3),
                    "RECEITA",
                    "A FATURAR",
                    "SICREDI",
                });
            };

            return ResponseMessage(myXLSWrapper.GetSingleSheetHttpResponse());
        }

        public IHttpActionResult Get(bool? exportar = false)
        {            
            var skip = Request.GetQueryStringValue<int>("skip");
            var take = Request.GetQueryStringValue<int>("take");

            var codigo = Request.GetQueryStringValue<string>("codigo");
            var tipoDemonstrativo = Request.GetQueryStringValue<int>("tipoDemonstrativo");
            var ano = Request.GetQueryStringValue<int>("ano");
            var mes = Request.GetQueryStringValue<int>("mes");

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mon = new money();

            var lst = new List<FaturamentoDTO>();

            int tot = 0;

            if (tipoDemonstrativo == 1)
            {
                #region - empresa -

                #endregion
            }
            else 
            {
                #region - lojista -

                // -==================-
                //     lista
                // -==================-

                DateTime dt_ini = new DateTime(ano, mes, 1).AddMonths(-1).AddDays(19);
                DateTime dt_fim = new DateTime(ano, mes, 19).AddHours(23).AddMinutes(59).AddSeconds(59);

                var lst_fk_lojas = (from e in db.LOG_Transacoes
                                    join loj in db.T_Loja on e.fk_loja equals (int) loj.i_unique
                                    where e.dt_transacao > dt_ini && e.dt_transacao < dt_fim
                                    where e.tg_confirmada.ToString() == TipoConfirmacao.Confirmada
                                    where codigo == null || loj.st_loja == codigo
                                    select e.fk_loja).
                                    Distinct();

                tot = lst_fk_lojas.Count();

                // -==================-
                //     paginação
                // -==================-

                if (exportar == false)
                    lst_fk_lojas = lst_fk_lojas.Skip(skip).Take(take);
                
                foreach (var item in (from e in db.T_Loja where lst_fk_lojas.Contains((int)e.i_unique) select e).ToList())
                {
                    // -================================-
                    //     calculo de faturamento
                    // -================================-

                    int totalTransacoes = (from e in db.LOG_Transacoes
                                           where e.dt_transacao > dt_ini && e.dt_transacao < dt_fim
                                           where e.fk_loja == item.i_unique
                                           select e).
                                           Count();

                    int totTransFranq = totalTransacoes - (int) item.nu_franquia;

                    if (totTransFranq < 0)
                        totTransFranq = 0;
                                       
                    long vrTransacoes = (from e in db.LOG_Transacoes
                                         where e.dt_transacao > dt_ini && e.dt_transacao < dt_fim
                                         where e.fk_loja == item.i_unique
                                         where e.tg_confirmada.ToString() == TipoConfirmacao.Confirmada
                                         select (long)e.vr_total).
                                         Sum();

                    long totCom = (vrTransacoes * (long)item.nu_pctValor) / 10000;

                    long totX = (int)item.vr_transacao * totTransFranq + 
                                (int)item.vr_mensalidade +
                                totCom;

                    lst.Add(new FaturamentoDTO
                    {
                        codigo = item.st_loja,
                        nome = item.st_nome,
                        social = item.st_social,
                        dia_venc = new DateTime(ano, mes, (int) item.nu_periodoFat).AddMonths(1).ToString("dd/MM/yyyy"),
                        total = "R$ " + mon.setMoneyFormat(totX),
                        _total = (int)totX,
                        detalhes = new List<FaturamentoDTOItem>
                        {
                            new FaturamentoDTOItem { item = "Periodo [" + dt_ini.ToString("dd/MM/yyyy") + "] a [" + dt_fim.ToString("dd/MM/yyyy") + "]" },
                            new FaturamentoDTOItem { item = "Total de transações [" + totalTransacoes + "], Custo por transação [R$ " + mon.setMoneyFormat((long)item.vr_transacao) + "], Franquia [" + item.nu_franquia + "]"},
                            new FaturamentoDTOItem { item = "Valor vendas [R$ " + mon.setMoneyFormat(vrTransacoes) + "], Pct Valor % [ " + mon.setMoneyFormat((long)item.nu_pctValor) + "]" },
                            new FaturamentoDTOItem { item = "Valor transação (1) = R$" + mon.setMoneyFormat((long)item.vr_transacao * totTransFranq) + " Final [" + totTransFranq + "]" },
                            new FaturamentoDTOItem { item = "Valor Mensalidade (2) = R$" + mon.setMoneyFormat((long)item.vr_mensalidade ) },
                            new FaturamentoDTOItem { item = "Valor Comissão (3) = R$" + mon.setMoneyFormat(totCom ) },
                            new FaturamentoDTOItem { item = "Total = R$" + mon.setMoneyFormat(totX) },}
                        }
                    );
                }

                if (exportar == true)
                    return Export(lst, ano, mes);

                #endregion
            }

            return Ok(new
            {
                count = tot,
                results = lst
            });
        }
    }
}
