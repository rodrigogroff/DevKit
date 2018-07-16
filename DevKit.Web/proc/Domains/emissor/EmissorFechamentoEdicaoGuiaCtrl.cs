using DataModel;
using System;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class EmissorFechamentoEdicaoGuiaController : ApiControllerBase
	{
        public IHttpActionResult Get()
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var filter = new 
            {                
                idGuia = Request.GetQueryStringValue("idGuia", 0),
                vlr = ObtemValor( Request.GetQueryStringValue("vlr")),
                vlrCoPart = ObtemValor(Request.GetQueryStringValue("vlrCoPart")),
            };

            if (new Empresa().EdicaoGuia (db, filter.idGuia, filter.vlr, filter.vlrCoPart))
                return Ok();
            else
                return BadRequest("Falha na atualização");
        }
	}
}
