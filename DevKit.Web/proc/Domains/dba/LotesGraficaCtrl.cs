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
                                
                return Ok(new { codigo = lg.Create(db, empresas) } );
            }
            else if (ativarLote == true)
            {
                var lotes = Request.GetQueryStringValue("lotes");

                lg.Ativar(db, lotes);

                return Ok();
            }
            else 
            {
                var filter = new LoteGraficaFilter
                {
                    skip = Request.GetQueryStringValue("skip", 0),
                    take = Request.GetQueryStringValue("take", 15),
                    nuCodigo = Request.GetQueryStringValue<long?>("codigo", null),
                };

                return Ok(lg.ComposedFilters(db, filter));
            }            
        }
        
        [HttpGet]
        [AllowAnonymous]
        [Route("api/lotesgrafica/exportar", Name = "LotesExportar")]
        public IHttpActionResult Exportar()
        {
            db = new DevKitDB();

            var idLote = Request.GetQueryStringValue("idLote");

            string dir = "c:\\lotes_grafica\\";
            string file = dir + "Lote_" + idLote + ".txt";
            string ext = ".txt";

            using (var sw = new StreamWriter(file, false, Encoding.Default))
            {
                foreach (var item in (from e in db.LoteGraficaCartao
                                      where e.fkLoteGrafica.ToString() == idLote
                                      select e).
                                      ToList())
                {
                    var assoc = db.Person.Where(y => y.id == item.fkAssociado).FirstOrDefault();
                    var emp = db.Empresa.Where(y => y.id == item.fkEmpresa).FirstOrDefault();

                    var empresa = emp.nuEmpresa.ToString().PadLeft(6, '0');
                    var mat = assoc.nuMatricula.ToString().PadLeft(6, '0');

                    string line = "+";
                    
                    line += assoc.stName.PadRight(30, ' ').Substring(0,30) + ",";
                    line += empresa + ",";
                    line += mat + ",";

                    if (assoc.stVencCartao == null)
                    {
                        var dt = DateTime.Now.AddYears(5);

                        assoc.stVencCartao = dt.Month.ToString().PadLeft(2, '0') + dt.Year.ToString().Substring(2);

                        db.Update(assoc);
                    }

                    line += assoc.stVencCartao.Substring(0, 2) + "/" + assoc.stVencCartao.Substring(2, 2) + ",";

                    line += calculaCodigoAcesso (empresa,
                                                  mat,
                                                  item.nuTit.ToString(),
                                                  item.nuVia.ToString(),
                                                  assoc.stCPF );

                    line += ",";
                    line += assoc.stName.PadRight(30, ' ').Substring(0, 30) + ",";

                    line += "|";

                    line += "826766" + empresa +
                                        mat +
                                         item.nuTit.ToString() +
                                        item.nuVia.ToString() +
                             "65" + assoc.stVencCartao;

                    line += "|";

                    sw.WriteLine(line);
                }
            }

            return ResponseMessage(TransferirConteudo(file));
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
