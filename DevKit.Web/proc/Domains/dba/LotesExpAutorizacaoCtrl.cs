using DataModel;
using LinqToDB;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class LotesExpAutorizacaoController : ApiControllerBase
	{
        public IHttpActionResult Get()
	    {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            return Ok(new LoteExpAutorizacao().ComposedFilters(db, new LoteExpAutorizacaoFilter
            {
                fkEmpresa = Request.GetQueryStringValue<long?>("fkEmpresa", null),
                nuMes = Request.GetQueryStringValue<long?>("nuMes", null),
                nuAno = Request.GetQueryStringValue<long?>("nuAno", null),
            }));                
        }
        
        [HttpGet]
        [AllowAnonymous]
        [Route("api/lotesexpautorizacao/exportar", Name = "LotesAutExportar")]
        public IHttpActionResult Exportar()
        {
            var fkEmpresa = Request.GetQueryStringValue<long?>("fkEmpresa");
            var nuMes = Request.GetQueryStringValue<long?>("nuMes");
            var nuAno = Request.GetQueryStringValue<long?>("nuAno");

            db = new DevKitDB();
            var lg = new LoteExpAutorizacao();

            return ResponseMessage ( 
                        TransferirConteudo ( 
                            lg.Exportar (db, fkEmpresa, nuMes, nuAno) ) );
        }
	}
}
