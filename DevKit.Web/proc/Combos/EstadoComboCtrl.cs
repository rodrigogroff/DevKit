using DataModel;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class EstadoComboController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var parameters = Request.GetQueryStringValue("busca","").ToUpper();
            
            var hshReport = SetupCacheReport(CacheTags.EstadoComboReport);
            if (hshReport[parameters] is ComboReport report)
                return Ok(report);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var ret = new Estado().ComboFilters(db, parameters);
                        
            hshReport[parameters] = ret;

            return Ok(ret);
        }

        public IHttpActionResult Get(long id)
        {
            if (RestoreCache(CacheTags.EstadoCombo, id) is BaseComboResponse obj)
                return Ok(obj);
                
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = (from e in db.Estado where e.id == id
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
