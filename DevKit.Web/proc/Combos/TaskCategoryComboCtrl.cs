using DataModel;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class TaskCategoryComboController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var filter = new TaskCategoryFilter
            {
                fkEmpresa = db.currentUser.fkEmpresa,
                busca = Request.GetQueryStringValue("busca","").ToUpper(),
                fkTaskType = Request.GetQueryStringValue<long?>("fkTaskType", null)
            };

            var parameters = filter.Parameters();

            var hshReport = SetupCacheReport(CacheTags.TaskCategoryComboReport);
            if (hshReport[parameters] is ComboReport report)
                return Ok(report);
            
            var ret = new TaskCategory().ComboFilters(db, filter);

            hshReport[parameters] = ret;

            return Ok(ret);
        }

        public IHttpActionResult Get(long id)
        {
            if (RestoreCache(CacheTags.TaskCategoryCombo, id) is BaseComboResponse obj)
                return Ok(obj);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = (from e in db.TaskCategory
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