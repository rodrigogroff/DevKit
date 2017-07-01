using DataModel;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class TaskFlowComboController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            var filter = new TaskFlowFilter
            {
                busca = Request.GetQueryStringValue("busca","").ToUpper(),
                fkTaskType = Request.GetQueryStringValue<long?>("fkTaskType", null),
                fkTaskCategory = Request.GetQueryStringValue<long?>("fkTaskCategory", null),
            };

            var parameters = filter.busca;

            if (filter.fkTaskType != null)
                parameters += "," + filter.fkTaskType;
            else
                parameters += ",";

            if (filter.fkTaskType != null)
                parameters += "," + filter.fkTaskType;

            var hshReport = SetupCacheReport(CacheTags.TaskFlowComboReport);
            if (hshReport[parameters] is ComboReport report)
                return Ok(report);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var ret = new TaskFlow().ComboFilters(db, filter);

            hshReport[parameters] = ret;

            return Ok(ret);            
		}
		
		public IHttpActionResult Get(long id)
		{
            if (RestoreCache(CacheTags.TaskFlowCombo, id) is BaseComboResponse obj)
                return Ok(obj);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = (from e in db.TaskFlow
                       where e.id == id
                       select new BaseComboResponse
                       {
                           id = e.id,
                           stName = e.stName
                       }).
                       FirstOrDefault();

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            BackupCache(mdl);

            return Ok(mdl);
        }
	}
}
