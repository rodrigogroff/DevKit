using System.Linq;
using System.Web.Http;
using SyCrafEngine;
using LinqToDB;
using System;
using System.Collections.Generic;

namespace DevKit.Web.Controllers
{
    public class EmissoraRelRepFinancController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var tipo = Request.GetQueryStringValue("tipo");            

            var meses = ",Janeiro,Fevereiro,Março,Abril,Maio,Junho,Julho,Agosto,Setembro,Outubro,Novembro,Dezembro".Split(',');

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mon = new money();

            switch (tipo)
            {
                // ------------------------
                // movimento resumido
                // ------------------------

                case "1": 
                    {
                        return Ok(new
                        {
                            //count = lst.Count(),
                            //results = lst,
                            dtEmissao = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                        });
                    }
            }

            return BadRequest();
        }
    }
}
