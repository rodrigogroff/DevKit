using System.Collections.Generic;
using System.Web.Http;
using System;
using System.Linq;
using LinqToDB;
using SyCrafEngine;
using DataModel;

namespace DevKit.Web.Controllers
{
    public class ExtratoAssociadoFechado
    {
        public string dataHora = "", 
                      nsu = "", 
                      valor = "", 
                      parcela = "",
                      estab = "";
    }
    
    public class ExtratoAssociadoController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var tipo = Request.GetQueryStringValue<int>("tipo", 0);
            var mes = Request.GetQueryStringValue("extrato_fech_mes").PadLeft(2,'0');
            var ano = Request.GetQueryStringValue("extrato_fech_ano_inicial");

            if (!StartDatabaseAndAuthorize())
                return BadRequest("Não autorizado!");

            var mon = new money();

            switch (tipo)
            {
                case 1: // extrato fechado
                {
                    var lst = new List<ExtratoAssociadoFechado>();
                        
                    foreach (var item in (from e in db.LOG_Fechamento
                                            where e.fk_cartao == db.currentAssociado.i_unique
                                            where e.st_ano == ano
                                            where e.st_mes == mes
                                            orderby e.dt_compra descending
                                            select e).
                                            ToList())
                    {
                        var parcela = (from e in db.T_Parcelas
                                       where e.i_unique == item.fk_parcela
                                       select e).
                                       FirstOrDefault();

                        var loja = (from e in db.T_Loja
                                    where e.i_unique == item.fk_loja
                                    select e).
                                    FirstOrDefault();

                        lst.Add(new ExtratoAssociadoFechado
                        {
                            dataHora = Convert.ToDateTime(item.dt_compra).ToString("dd/MM/yyyy"),
                            nsu = parcela.nu_nsu.ToString(),
                            valor = mon.formatToMoney(parcela.vr_valor.ToString()),
                            parcela = parcela.nu_indice.ToString() + " / " + parcela.nu_tot_parcelas.ToString(),
                            estab = loja.st_nome
                        });
                    }
        
                    return Ok(new
                    {
                        count = 1,
                        results = lst
                    });
                }
            }

            return BadRequest();            
        }
    }
}
