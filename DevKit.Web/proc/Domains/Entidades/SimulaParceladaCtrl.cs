using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using System;
using SyCrafEngine;
using DataModel;

namespace DevKit.Web.Controllers
{
    public class SimulaParceladaController : ApiControllerBase
    {
        [NonAction]
        public long ObtemValor(string valor)
        {
            var iValor = 0;

            if (!valor.Contains(",")) valor += ",00"; // 10 => 10,00
            valor = valor.Replace(",", "").Replace(".", ""); // 10,00 => 1000
            iValor = Convert.ToInt32(valor);

            return iValor;
        }

        public IHttpActionResult Get()
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();
            
            var idCartao = Request.GetQueryStringValue<int>("cartao");
            var valor = Request.GetQueryStringValue("valor");
            var parcelas = Request.GetQueryStringValue<int>("parcelas");

            string  p1 = Request.GetQueryStringValue("p1"),
                    p2 = Request.GetQueryStringValue("p2"),
                    p3 = Request.GetQueryStringValue("p3"),
                    p4 = Request.GetQueryStringValue("p4"),
                    p5 = Request.GetQueryStringValue("p5"),
                    p6 = Request.GetQueryStringValue("p6"),
                    p7 = Request.GetQueryStringValue("p7"),
                    p8 = Request.GetQueryStringValue("p8"),
                    p9 = Request.GetQueryStringValue("p9"),
                    p10 = Request.GetQueryStringValue("p10"),
                    p11 = Request.GetQueryStringValue("p11"),
                    p12 = Request.GetQueryStringValue("p12");

            var lstParametrosZerados = new List<bool>();

            if (parcelas >= 1) if (p1 == null || p1 == "0,00" || p1 == "0") lstParametrosZerados.Add(true); else lstParametrosZerados.Add(false);
            if (parcelas >= 2) if (p2 == null || p2 == "0,00" || p2 == "0") lstParametrosZerados.Add(true); else lstParametrosZerados.Add(false);
            if (parcelas >= 3) if (p3 == null || p3 == "0,00" || p3 == "0") lstParametrosZerados.Add(true); else lstParametrosZerados.Add(false);
            if (parcelas >= 4) if (p4 == null || p4 == "0,00" || p4 == "0") lstParametrosZerados.Add(true); else lstParametrosZerados.Add(false);
            if (parcelas >= 5) if (p5 == null || p5 == "0,00" || p5 == "0") lstParametrosZerados.Add(true); else lstParametrosZerados.Add(false);
            if (parcelas >= 6) if (p6 == null || p6 == "0,00" || p6 == "0") lstParametrosZerados.Add(true); else lstParametrosZerados.Add(false);
            if (parcelas >= 7) if (p7 == null || p7 == "0,00" || p7 == "0") lstParametrosZerados.Add(true); else lstParametrosZerados.Add(false);
            if (parcelas >= 8) if (p8 == null || p8 == "0,00" || p8 == "0") lstParametrosZerados.Add(true); else lstParametrosZerados.Add(false);
            if (parcelas >= 9) if (p9 == null || p9 == "0,00" || p9 == "0") lstParametrosZerados.Add(true); else lstParametrosZerados.Add(false);
            if (parcelas >= 10) if (p10 == null || p10 == "0,00" || p10 == "0") lstParametrosZerados.Add(true); else lstParametrosZerados.Add(false);
            if (parcelas >= 11) if (p11 == null || p11 == "0,00" || p11 == "0") lstParametrosZerados.Add(true); else lstParametrosZerados.Add(false);
            if (parcelas >= 12) if (p12 == null || p12 == "0,00" || p12 == "0") lstParametrosZerados.Add(true); else lstParametrosZerados.Add(false);

            int totalZerados = lstParametrosZerados.Where (y=> y == true).Count();

            if (totalZerados == parcelas)
            {
                totalZerados = 0;
                for (int i = 0; i < lstParametrosZerados.Count; i++)
                    lstParametrosZerados[i] = false;
            }

            long iValor = ObtemValor(valor);

            var associado = RestoreTimerCache("associado", idCartao.ToString(), 5) as T_Cartao;

            if (associado == null)
            {
                associado = (from e in db.T_Cartao
                             where e.i_unique == idCartao
                             select e).
                             FirstOrDefault();

                BackupCache(associado);
            }                

            if (iValor > (int)associado.vr_limiteTotal)
                return BadRequest("Limite excedido");

            var empresa = RestoreTimerCache("empresa", associado.st_empresa, 5) as T_Empresa;

            if (empresa == null)
            {
                empresa = (from e in db.T_Empresa
                           where e.st_empresa == associado.st_empresa
                           select e).
                           FirstOrDefault();

                BackupCache(empresa);
            }
            
            if (parcelas > empresa.nu_parcelas)
                return BadRequest("Excedeu número de parcelas da empresa");
            
            var iParcIdeal = iValor / (parcelas - totalZerados);

            long iUltimaParc = iParcIdeal;

            if (iParcIdeal * parcelas < iValor)
                iUltimaParc += iValor - (iParcIdeal * parcelas);

            if (iParcIdeal > associado.vr_limiteMensal + associado.vr_extraCota)
                return BadRequest("Limite mensal excedido");

            var mon = new money();
            var lst = new List<SimulacaoParcela>();

            var tagEmpMat = associado.st_empresa + associado.st_matricula;

            var lstCarts = RestoreTimerCache("lstCarts", tagEmpMat, 5) as List<int>;

            if (lstCarts == null)
            {
                lstCarts = (from e in db.T_Cartao
                            where e.st_empresa == associado.st_empresa
                            where e.st_matricula == associado.st_matricula
                            select (int)e.i_unique).
                           ToList();

                BackupCache(lstCarts);
            }

            long vrSum = 0;

            for (int t=0; t < parcelas; ++t)
            {
                var tagParcelaAtual = tagEmpMat + t.ToString();

                var maxParcAtual = RestoreTimerCache("parcelasCartao", tagParcelaAtual, 5) as string;

                if (maxParcAtual == null)
                {
                    var lstParcs = (from e in db.T_Parcelas
                                    where lstCarts.Contains((int)e.fk_cartao)
                                    where e.nu_parcela == t + 1
                                    select e).
                                    ToList();

                    int mTot = 0;

                    foreach (var item in lstParcs)
                    {
                        var ltr = (from e in db.LOG_Transacoes
                                   where e.i_unique == item.fk_log_transacoes
                                   select e).
                                   FirstOrDefault();
                        
                        if (ltr.tg_confirmada == TipoConfirmacao.Confirmada)
                        {
                            mTot += (int) item.vr_valor;
                        }
                    }

                    maxParcAtual = mTot.ToString();
                                       
                    BackupCache(maxParcAtual);
                }                

                string vr = "0,00";

                if (lstParametrosZerados[t] != true)
                {
                    vrSum += iParcIdeal;
                    vr = mon.setMoneyFormat(iParcIdeal);
                }
                
                lst.Add(new SimulacaoParcela
                {
                    valor = vr,
                    valorMax = mon.setMoneyFormat ( ((int)associado.vr_limiteMensal + (int)associado.vr_extraCota) - Convert.ToInt32(maxParcAtual)),
                });
            }

            if (vrSum != iValor)
            {
                long vrUltParc = Convert.ToInt64(lst[parcelas - 1].valor.Replace(".", "").Replace(",", ""));

                vrUltParc += iValor - vrSum;

                lst[parcelas - 1].valor = mon.setMoneyFormat(vrUltParc);
            }

            lst.Add(new SimulacaoParcela
            {
                valor = mon.setMoneyFormat(iValor)                
            });

            return Ok(new
            {
                count = lst.Count(),
                results = lst
            });
        }
    }
}
