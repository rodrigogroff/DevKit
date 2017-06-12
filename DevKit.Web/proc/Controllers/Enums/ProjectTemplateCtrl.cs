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
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            string busca = Request.GetQueryStringValue("busca", "").ToUpper();

            var hshReport = SetupCacheReport(CacheObject.EnumPriorityReport);
            if (hshReport[busca] is TaskReport report)
                return Ok(report);

            var _enum = new EnumProjectTemplate();

            var query = (from e in _enum.lst select e);

			if (busca != "")
				query = from e in query where e.stName.ToUpper().Contains(busca) select e;

            var ret = new
            {
                count = query.Count(),
                results = query.ToList()
            };

            hshReport[busca] = ret;

            return Ok(ret);
        }

        public IHttpActionResult Get(long id)
        {
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var obj = RestoreCache(CacheObject.EnumProjectTemplate, id);
            if (obj != null)
                return Ok(obj);

            var model = new EnumProjectTemplate().Get(id);

            if (model == null)
                return StatusCode(HttpStatusCode.NotFound);

            BackupCache(model);

            return Ok(model);
        }
    }
}
