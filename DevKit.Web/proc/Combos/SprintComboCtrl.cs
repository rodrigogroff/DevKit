using DataModel;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class SprintComboController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            var filter = new ProjectSprintFilter
            {
                busca = Request.GetQueryStringValue("busca","").ToUpper(),
                fkProject = Request.GetQueryStringValue<long?>("fkProject", null),
                fkPhase = Request.GetQueryStringValue<long?>("fkPhase", null),
            };

            var parameters = filter.busca;

            if (filter.fkProject != null)
                parameters += "," + filter.fkProject;
            else
                parameters += ",";

            if (filter.fkPhase != null)
                parameters += "," + filter.fkPhase;

            var hshReport = SetupCacheReport(CacheTags.SprintComboReport);
            if (hshReport[parameters] is ComboReport report)
                return Ok(report);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var ret = new ProjectSprint().ComboFilters(db, filter);

            hshReport[parameters] = ret;

            return Ok(ret);
        }

        public IHttpActionResult Get(long id)
        {
            if (RestoreCache(CacheTags.SprintCombo, id) is BaseComboResponse obj)
                return Ok(obj);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = (from e in db.ProjectSprint
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