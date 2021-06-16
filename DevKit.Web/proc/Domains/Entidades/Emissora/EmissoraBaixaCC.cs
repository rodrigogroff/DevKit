using DataModel;
using LinqToDB;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class BaixaManualDto
    {
        public long idCartao { get; set; }
        public string valor { get; set; }
        public long ano { get; set; }
        public long mes { get; set; }
    }

    public class EmissoraBaixaCCController : ApiControllerBase
    {
        [HttpPost]
        public IHttpActionResult Post(BaixaManualDto desp)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var tEmp = db.currentEmpresa;

            db.Insert(new LancamentosCC
            {
                bRecorrente = false,
                dtLanc = System.DateTime.Now,
                fkInicial = null,
                fkBaixa = null,
                fkCartao = desp.idCartao,
                fkEmpresa = (long) tEmp.i_unique,
                fkTipo = db.EmpresaDespesa.FirstOrDefault ( y=> y.stCodigo == "10" && y.fkEmpresa == (long)tEmp.i_unique).id,
                nuAno = desp.ano,
                nuMes = desp.mes,
                nuParcela = 1,
                nuTotParcelas = 1,
                vrValor = ObtemValor(desp.valor),                
            });
            
            return Ok(desp);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("baixacc")]
        public HttpResponseMessage Xdad()
        {
            var httpRequest = HttpContext.Current.Request;

            if (HttpContext.Current.Request.Files.Count < 1)
            {
                return Request.CreateResponse(HttpStatusCode.Created);
            }
            else
            {
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    var binReader = new BinaryReader(postedFile.InputStream);
                    byte[] byteArray = binReader.ReadBytes(postedFile.ContentLength);

                    string result = System.Text.Encoding.UTF8.GetString(byteArray);

                    foreach (var line in result.Split ('\n'))
                    {

                    }
                }

                return Request.CreateResponse(HttpStatusCode.Created);
            }
        }
    }
}
