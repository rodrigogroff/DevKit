using System.Collections.Generic;
using System.Web.Http;
using System;
using System.Linq;
using LinqToDB;
using SyCrafEngine;

namespace DevKit.Web.Controllers
{
    public class CancelaVendaController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var nsu = Request.GetQueryStringValue<int>("nsu");

            var cupom = new List<string>();



            return Ok(new
            {
                count = 1,
                results = cupom
            });
        }
    }
}
