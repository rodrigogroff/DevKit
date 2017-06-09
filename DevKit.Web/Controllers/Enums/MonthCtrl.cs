using DataModel;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class MonthController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            string busca = Request.GetQueryStringValue("busca")?.ToUpper();

            var hshReport = SetupCacheReport(CachedObject.EnumMonthReport);

            if (hshReport[busca] is TaskReport report)
                return Ok(report);

            var _enum = new EnumMonth();

            var query = (from e in _enum.lst select e);

			if (busca != null)
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

            var cacheTag = CachedObject.EnumMonth + id;

            var obj = RestoreCache(cacheTag);

            if (obj != null)
                return Ok(obj);

            var _enum = new EnumMonth();

            var model = _enum.Get(id);

			if (model != null)
            {
                BackupCache(cacheTag, model);

                return Ok(model);
            }				

			return StatusCode(HttpStatusCode.NotFound);			
		}
	}
}
