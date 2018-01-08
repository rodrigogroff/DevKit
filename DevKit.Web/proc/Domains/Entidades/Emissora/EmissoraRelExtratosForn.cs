using System.Linq;
using System.Web.Http;
using SyCrafEngine;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Collections;

namespace DevKit.Web.Controllers
{
    public class EmissoraRelExtratosFornDTO
    {
        public string nome, empresa, total, totalRep;

        public List<ItensForn> itens = new List<ItensForn>();
    }

    public class ItensForn
    {
        public string serial, dtVenda, nsu, valor, parcela, vlrParcela;
    }

    public class EmissoraRelExtratosFornController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var tipo = Request.GetQueryStringValue("tipo");
            var codigo = Request.GetQueryStringValue("codigo");
            var mes = Request.GetQueryStringValue("mes",0);
            var ano = Request.GetQueryStringValue("ano", 0);

            var meses = ",Janeiro,Fevereiro,Março,Abril,Maio,Junho,Julho,Agosto,Setembro,Outubro,Novembro,Dezembro".Split(',');

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mon = new money();

            long vrTotalMax = 0, vrBonusMax = 0, vrRepasseMax = 0;

            switch (tipo)
            {
                // ------------------------
                // em aberto
                // ------------------------

                case "1": 
            
                    break;
                    
                // ------------------------
                // encerrado
                // ------------------------

                case "2":
                    
                    break;
            }

            return Ok(new
            {
                referencia = meses[mes] + " / " + ano,
                empresa = db.currentEmpresa.st_fantasia + " (" + db.currentEmpresa.st_empresa + ")",
                total = mon.setMoneyFormat(vrTotalMax),
                totalBonus = mon.setMoneyFormat(vrBonusMax),
                totalRep = mon.setMoneyFormat(vrRepasseMax),
                txAdmin = "0",
                //  results = lst,
                dtEmissao = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
            });
        }
    }
}
