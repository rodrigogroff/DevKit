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
	public class LotesGraficaController : ApiControllerBase
	{
        public IHttpActionResult Get()
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var pesqInicialNovoLote = Request.GetQueryStringValue<bool?>("novoLote", null);
            var criarLote = Request.GetQueryStringValue<bool?>("criarLote", null);
            var ativarLote = Request.GetQueryStringValue<bool?>("ativarLote", null);

            var lg = new LoteGrafica();

            if (pesqInicialNovoLote == true)
            {
                return Ok(lg.NovoLoteQuery(db));
            }
            else if (criarLote == true)
            {
                var empresas = Request.GetQueryStringValue("empresas");

                return Ok(new { codigo = lg.Create(db, empresas) });
            }
            else if (ativarLote == true)
            {
                var lotes = Request.GetQueryStringValue("lotes");

                lg.Ativar(db, lotes);

                CleanCache(db, CacheTags.Associado, null);

                return Ok();
            }
            else
            {
                return Ok(lg.ComposedFilters(db, new LoteGraficaFilter
                {
                    skip = Request.GetQueryStringValue("skip", 0),
                    take = Request.GetQueryStringValue("take", 15),
                    nuCodigo = Request.GetQueryStringValue<long?>("codigo", null),
                }));
            }            
        }
        
        [HttpGet]
        [AllowAnonymous]
        [Route("api/lotesgrafica/exportar", Name = "LotesExportar")]
        public IHttpActionResult Exportar()
        {
            var idLote = Request.GetQueryStringValue("idLote");
            var dep = Request.GetQueryStringValue("dep");

            db = new DevKitDB();
            var lg = new LoteGrafica();

            return ResponseMessage ( 
                        TransferirConteudo ( 
                            lg.Exportar (db,idLote,dep) ) );
        }
            
        public IHttpActionResult Get(long id)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.LoteGrafica.Where(y => y.id == id).FirstOrDefault();

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);
            
            mdl.LoadAssociations(db);

            return Ok(mdl);
        }

		public IHttpActionResult Put(long id, LoteGrafica mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Update(db, ref apiError))
                return BadRequest(apiError);

            mdl.LoadAssociations(db);

            return Ok();			
		}
        
	}
}
