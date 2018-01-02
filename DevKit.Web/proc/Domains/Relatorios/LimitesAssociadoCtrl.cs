using System.Collections.Generic;
using System.Web.Http;
using System;
using System.Linq;
using LinqToDB;
using SyCrafEngine;
using DataModel;

namespace DevKit.Web.Controllers
{
    public class LimiteAssociadoDTO
    {
        public string nome = "", valor = "";
    }

    public class LimiteAssociadoController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest("Não autorizado!");

            var mon = new money();
            var sd = new SaldoDisponivel();
            var list = new List<LimiteAssociadoDTO>();

            list.Add(new LimiteAssociadoDTO
            {
                nome = "Limite cartão",
                valor = mon.setMoneyFormat((long)db.currentAssociado.vr_limiteMensal +
                                             (long)db.currentAssociado.vr_extraCota)
            });

            list.Add(new LimiteAssociadoDTO
            {
                nome = "Parcelamento",
                valor = db.currentAssociadoEmpresa.nu_parcelas.ToString().PadLeft(2, '0')
            });

            long dispMensal = 0, dispTot = 0;

            sd.Obter(db, db.currentAssociado, ref dispMensal, ref dispTot);

            list.Add(new LimiteAssociadoDTO
            {
                nome = "Limite mensal disponível",
                valor = "R$ " + mon.setMoneyFormat(dispMensal)
            });

            list.Add(new LimiteAssociadoDTO
            {
                nome = "Cota Extra",
                valor = "R$ " + mon.setMoneyFormat((long)db.currentAssociado.vr_extraCota)
            });

            var scheduler = RestoreTimerCache(CacheTags.I_Scheduler, db.currentAssociado.st_empresa, 100) as I_Scheduler;

            if (scheduler == null)
            {
                scheduler = (from e in db.I_Scheduler
                             where e.st_job.Contains(db.currentAssociado.st_empresa)
                             select e).
                             FirstOrDefault();

                BackupCache(scheduler);
            }

            list.Add(new LimiteAssociadoDTO
            {
                nome = "Melhor dia de compra",
                valor = scheduler.nu_monthly_day.ToString().PadLeft(2, '0')
            });

            list.Add(new LimiteAssociadoDTO
            {
                nome = "Parcela mensal utilizada",
                valor = "R$ " + mon.setMoneyFormat(sd.vrUtilizadoAtual)
            });

            var dt = DateTime.Now;

            if (DateTime.Now.Day >= scheduler.nu_monthly_day)
            {
                dt = dt.AddMonths(1);

                list.Add(new LimiteAssociadoDTO
                {
                    nome = "Mês em vigência",
                    valor = new EnumMonth().Get(dt.Month).stName + " / " + dt.Year
                });
            }
            else
            {
                list.Add(new LimiteAssociadoDTO
                {
                    nome = "Mês em vigência",
                    valor = new EnumMonth().Get(dt.Month).stName + " / " + dt.Year
                });
            }

            return Ok(new
            {
                count = 1,
                results = list
            });
        }
    }
}
