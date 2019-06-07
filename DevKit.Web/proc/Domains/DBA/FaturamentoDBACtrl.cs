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
            var codigo = Request.GetQueryStringValue<string>("codigo");
            var tipoDemonstrativo = Request.GetQueryStringValue<int>("tipoDemonstrativo");
            var isentoFat = Request.GetQueryStringValue<int>("isentoFat");
            var semFat = Request.GetQueryStringValue<int>("semFat");
            var ano = Request.GetQueryStringValue<int>("ano");
            var mes = Request.GetQueryStringValue<int>("mes");

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mon = new money();
            var lst = new List<FaturamentoDTO>();

            long tot = 0, totFat = 0;

            DateTime dt_ini = new DateTime(ano, mes, 1).AddMonths(-1).AddDays(19);
            DateTime dt_fim = new DateTime(ano, mes, 19).AddHours(23).AddMinutes(59).AddSeconds(59);

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

                var lst_lojas = (from e in db.T_Loja
                                 where e.tg_blocked.ToString() == "0"
                                 where string.IsNullOrEmpty(codigo) || e.st_loja.Contains (codigo)
                                 orderby e.st_social 
                                 select e).
                                 ToList();

                if (isentoFat == 1)
                {
                    lst_lojas = (from e in lst_lojas
                                 where e.tg_isentoFat == 1
                                orderby e.st_social
                                select e).
                                 ToList();
                }

                tot = lst_lojas.Count();

                var lstTemp = lst_lojas.Select(y => y.i_unique).ToList();

                var transList = (from e in db.LOG_Transacoes
                                 where e.dt_transacao > dt_ini && e.dt_transacao < dt_fim
                                 where lstTemp.Contains((int)e.fk_loja)
                                 select e).
                                 ToList();

                foreach (var item in lst_lojas)
                {
                    // -================================-
                    //     calculo de faturamento
                    // -================================-

                    int totalTransacoes = transList.Where (y=> y.fk_loja == item.i_unique).Count();

                    int totTransFranq = totalTransacoes - (int) item.nu_franquia;

                    if (totTransFranq < 0)
                        totTransFranq = 0;
                                       
                    long vrTransacoes = (from e in transList
                                         where e.dt_transacao > dt_ini && e.dt_transacao < dt_fim
                                         where e.fk_loja == item.i_unique
                                         where e.tg_confirmada.ToString() == TipoConfirmacao.Confirmada
                                         select (long)e.vr_total).
                                         Sum();

                    long totCom = (vrTransacoes * (long)item.nu_pctValor) / 10000;

                    long totX = (int)item.vr_transacao * totTransFranq + 
                                (int)item.vr_mensalidade +
                                totCom;

                    if (semFat == 1 && totX > 0)
                        continue;

                    if (semFat == 2 && totX == 0)
                        continue;

                    if (item.tg_isentoFat == 1)
                        totX = 0;

                    if (isentoFat == 2 && item.tg_isentoFat == 1)
                        continue;

                    if (isentoFat == 1 && totX > 0)
                        continue;

                    totFat += totX;

                    lst.Add(new FaturamentoDTO
                    {
                        codigo = item.st_loja,
                        nome = item.st_nome,
                        social = item.st_social,
                        dia_venc = new DateTime(ano, mes, 5).AddMonths(1).ToString("dd/MM/yyyy"),
                        total = "R$ " + mon.setMoneyFormat(totX),
                        _total = (int)totX,
                        detalhes = new List<FaturamentoDTOItem>
                        {
                            new FaturamentoDTOItem { item = "Isento? [" + (item.tg_isentoFat == 1 ? "Sim" : "Não") + "]" },
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
                dtEmissao = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                dtVencimento = new DateTime(ano, mes, 5).AddMonths(1).ToString("dd/MM/yyyy"),
                perFatIni = dt_ini.ToString("dd/MM/yyyy"),
                perFatFim = dt_fim.ToString("dd/MM/yyyy"),
                totalFat = mon.setMoneyFormat(totFat),
                results = lst
            });
        }
    }
}
