using DataModel;
using LinqToDB;
using SyCrafEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    
    public class EmissoraLancCCExtratoPagarController : ApiControllerBase
    {
        public class DtoExtratoPagar_LancCC
        {
            public string codLojista { get; set; }
            public string razSoc { get; set; }
            public string lojista { get; set; }
            public string cnpj { get; set; }
            public string banco { get; set; }
            public string agencia { get; set; }
            public string conta { get; set; }
            public string vlrTot { get; set; }
            public string vlrComissao { get; set; }
            public string vlrRepasse { get; set; }
        }

        [HttpGet]
        public IHttpActionResult Get()
        {
            var ano = Convert.ToInt32(Request.GetQueryStringValue("ano"));
            var mes = Convert.ToInt32(Request.GetQueryStringValue("mes"));

            var mon = new money();

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            long totCC = 0, totComissao = 0, totRep = 0;

            var tEmp = db.currentEmpresa;
            
            var lstCC = new List<DtoExtratoPagar_LancCC>();

            // ---------------------------------------------------------------

            var queryLancCC = ( from e in db.LOG_Fechamento
                                where e.fk_empresa == tEmp.i_unique
                                where e.st_ano == ano.ToString()
                                where e.st_mes == mes.ToString().PadLeft(2,'0')
                                select e).
                                ToList();

            if (queryLancCC.Count == 0 )
            {
                return Ok(new
                {
                    listPagarCC = lstCC,
                });
            }

            var idsLojistas = queryLancCC.Select(y => (long)y.fk_loja).Distinct().ToList();
            var lstLojistas = db.T_Loja.Where(y => idsLojistas.Contains((long)y.i_unique)).ToList();

            var bancos = new EnumBancos();

            foreach (var item in idsLojistas)
            {
                var t_loja = lstLojistas.FirstOrDefault(y => y.i_unique == item);

                var convenio = (from e in db.T_Loja
                                from eConv in db.LINK_LojaEmpresa
                                where eConv.fk_empresa == tEmp.i_unique
                                where eConv.fk_loja == t_loja.i_unique
                                select eConv).
                                FirstOrDefault();

                var tot = queryLancCC.Where(y => y.fk_loja == item).Sum(y => (long)y.vr_valor);

                var t_vrBonus = tot * (long)convenio.tx_admin / 10000;
                var t_repasse = tot - t_vrBonus;

                var rec = new DtoExtratoPagar_LancCC
                {
                    lojista = t_loja.st_nome,
                    codLojista = t_loja.st_loja,
                    agencia = t_loja.st_agencia,
                    conta = t_loja.st_conta,
                    banco = t_loja.fk_banco == null ? "": bancos.Get((int)t_loja.fk_banco).stName,
                    cnpj = t_loja.nu_CNPJ,
                    razSoc = t_loja.st_social,
                    vlrTot = mon.setMoneyFormat(tot),
                    vlrComissao = mon.setMoneyFormat(t_vrBonus),
                    vlrRepasse = mon.setMoneyFormat(t_repasse),
                };

                lstCC.Add(rec);

                totCC += tot;
                totComissao += t_vrBonus;
                totRep += t_repasse;
            }

            lstCC = lstCC.OrderBy(y => y.lojista).ToList();

            return Ok(new 
            {
                vlrTotal = "R$ " + mon.setMoneyFormat(totCC),
                vlrTotComissao = "R$ " + mon.setMoneyFormat(totComissao),
                vlrTotRep = "R$ " + mon.setMoneyFormat(totRep),
                listPagarCC = lstCC,
            }); 
        }        
    }
}
