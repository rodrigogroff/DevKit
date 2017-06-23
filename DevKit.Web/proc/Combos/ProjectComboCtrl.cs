using DataModel;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class ProjectComboController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            var filter = new ProjectFilter
            {
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
                fkUser = Request.GetQueryStringValue<long?>("fkUser", null),
            };

            var parameters = filter.Parameters();

            var hshReport = SetupCacheReport(CacheTags.ProjectComboReport);
            if (hshReport[parameters] is ComboReport report)
                return Ok(report);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = new Project();

            var results = mdl.ComboFilters(db, filter);

            var ret = new ComboReport
            {
                count = results.Count,
                results = new List<BaseComboResponse>()
            };

            foreach (var item in results)
                ret.results.Add(new BaseComboResponse { id = item.id, stName = item.stName });

            hshReport[parameters] = ret;

            return Ok(ret);
        }

        public IHttpActionResult Get(long id)
		{
            if (RestoreCache(CacheTags.ProjectCombo, id) is BaseComboResponse obj)
                return Ok(obj);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetProject(id);

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            var ret = new BaseComboResponse
            {
                id = mdl.id,
                stName = mdl.stName
            };

            BackupCache(ret);

            return Ok(ret);
        }
	}
}
