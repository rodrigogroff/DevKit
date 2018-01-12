using DataModel;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class CidadeComboController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            //var parameters = Request.GetQueryStringValue("busca","").ToUpper();

            var filter = new CidadeFilter
            {
                busca = Request.GetQueryStringValue("busca", "").ToUpper(),
                fkEstado = Request.GetQueryStringValue<long?>("fkEstado", null),                
            };

            var parameters = filter.busca;

            if (filter.fkEstado != null)
                parameters += "," + filter.fkEstado;
            else
                parameters += ",";

            var hshReport = SetupCacheReport(CacheTags.CidadeComboReport);
            if (hshReport[parameters] is ComboReport report)
                return Ok(report);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var ret = new Cidade().ComboFilters(db, filter);
                        
            hshReport[parameters] = ret;

            return Ok(ret);
        }

        public IHttpActionResult Get(long id)
        {
            if (RestoreCache(CacheTags.CidadeCombo, id) is BaseComboResponse obj)
                return Ok(obj);
                
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = (from e in db.Cidade where e.id == id
                       select new BaseComboResponse
                       {
                           id = e.id,
                           stName = e.stNome
                       }).
                       FirstOrDefault();

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            BackupCache(mdl);

            return Ok(mdl);
        }
    }
}
