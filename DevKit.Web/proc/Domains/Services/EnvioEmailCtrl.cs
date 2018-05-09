using System.Collections.Generic;
using System.Web.Http;
using System;
using System.Linq;
using LinqToDB;
using SyCrafEngine;

namespace DevKit.Web.Controllers
{
    public class EnvioEmailController : ApiControllerBase
    {
        public string nsu;

        public IHttpActionResult Get()
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest("Não autorizado!");

            var nsu = Request.GetQueryStringValue("nsu");
            
            return Ok(new
            {
                count = 1,
                
            });
        }
    }
}
