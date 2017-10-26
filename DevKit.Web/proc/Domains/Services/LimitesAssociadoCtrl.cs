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

            var sd = new SaldoDisponivel();

            var list = new List<LimiteAssociadoDTO>();

            var mon = new money();

            list.Add(new LimiteAssociadoDTO
            {
                nome = "Limite cartão",
                valor = mon.setMoneyFormat((long)db.currentAssociado.vr_limiteMensal)
            });

            list.Add(new LimiteAssociadoDTO
            {
                nome = "Parcelamento",
                valor = db.currentAssociadoEmpresa.nu_parcelas.ToString().PadLeft(2,'0')
            });

            long dispMensal = (long)db.currentAssociado.vr_limiteMensal,
                 dispTot = (long)db.currentAssociado.vr_limiteMensal;
            
            sd.Obter(db, db.currentAssociado, ref dispMensal, ref dispTot);

            list.Add(new LimiteAssociadoDTO
            {
                nome = "Limite Disponível",
                valor = "R$ " + mon.setMoneyFormat(dispMensal)
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

            if (DateTime.Now.Day >= scheduler.nu_monthly_day )
            {
                var dt = DateTime.Now;

                list.Add(new LimiteAssociadoDTO
                {
                    nome = "Mês em vigência",
                    valor = new EnumMonth().Get(dt.Month).stName + " / " + dt.Year
                });
            }
            else
            {
                var dt = DateTime.Now.AddMonths(-1);

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
