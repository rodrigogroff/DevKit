using DataModel;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class ProjectTemplateController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            string busca = Request.GetQueryStringValue("busca", "").ToUpper();

            var hshReport = SetupCacheReport(CacheTags.EnumPriorityReport);
            if (hshReport[busca] is ProjectTemplateReport report)
                return Ok(report);

            var _enum = new EnumProjectTemplate();

            var query = (from e in _enum.lst select e);

			if (busca != "")
				query = from e in query where e.stName.ToUpper().Contains(busca) select e;

            var ret = new ProjectTemplateReport
            {
                count = query.Count(),
                results = query.ToList()
            };

            hshReport[busca] = ret;

            return Ok(ret);
        }

        public IHttpActionResult Get(long id)
        {
            var obj = RestoreCache(CacheTags.EnumProjectTemplate, id);
            if (obj != null)
                return Ok(obj);

            var mdl = new EnumProjectTemplate().Get(id);

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            BackupCache(mdl);

            return Ok(mdl);
        }
    }
}
