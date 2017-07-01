using DataModel;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class PhaseComboController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            var filter = new ProjectPhaseFilter
            {
                busca = Request.GetQueryStringValue("busca","").ToUpper(),
                fkProject = Request.GetQueryStringValue<int?>("fkProject", null),
            };

            var parameters = filter.busca;
            
            if (filter.fkProject != null)
                parameters += "," + filter.fkProject;

            var hshReport = SetupCacheReport(CacheTags.ProjectPhaseComboReport);
            if (hshReport[parameters] is ComboReport report)
                return Ok(report);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var ret = new ProjectPhase().ComboFilters(db, filter.busca, filter.fkProject);

            hshReport[parameters] = ret;

            return Ok(ret);
        }

        public IHttpActionResult Get(long id)
		{
            if (RestoreCache(CacheTags.ProjectPhaseCombo, id) is BaseComboResponse obj)
                return Ok(obj);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = (from e in db.ProjectPhase
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
