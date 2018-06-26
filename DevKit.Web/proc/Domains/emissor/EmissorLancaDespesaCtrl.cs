using DataModel;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class EmissorLancaDespesaController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var ret = new Empresa().LancaDespesa(db, new LancaDespesa_PARAMS
            {
                matricula = Request.GetQueryStringValue<long?>("matricula", null),
                credenciado = Request.GetQueryStringValue<long?>("credenciado", null),
                vrValor = ObtemValor(Request.GetQueryStringValue("vrValor")),
                dataLanc = ObtemData(Request.GetQueryStringValue("dataLanc")),
                nuTipo = Request.GetQueryStringValue<long?>("nuTipo", null),
                fkPrecificacao = Request.GetQueryStringValue<long?>("fkPrecificacao", null),
                nuParcelas = Request.GetQueryStringValue<long?>("nuParcelas", null),
            });

            if (!string.IsNullOrEmpty(ret))
                return BadRequest(ret);                
            else
                return Ok();
        }
    }
}
