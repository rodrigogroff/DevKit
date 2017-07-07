using System.Linq;
using System.Net;
using System.Web.Http;
using DataModel;

namespace DevKit.Web.Controllers
{
	public class VersionStateController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            string busca = Request.GetQueryStringValue("busca", "").ToUpper();

            var hshReport = SetupCacheReport(CacheTags.EnumVersionStateReport);
            if (hshReport[busca] is VersionStateReport report)
                return Ok(report);

            var _enum = new EnumVersionState();

            var query = (from e in _enum.lst select e);

			if (busca != "")
				query = from e in query where e.stName.ToUpper().Contains(busca) select e;

            var ret = new VersionStateReport
            {
                count = query.Count(),
                results = query.ToList()
            };

            hshReport[busca] = ret;

            return Ok(ret);
        }

        public IHttpActionResult Get(long id)
        {
            var obj = RestoreCache(CacheTags.EnumVersionState, id);
            if (obj != null)
                return Ok(obj);

            var mdl = new EnumVersionState().Get(id);

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            BackupCache(mdl);

            return Ok(mdl);
        }
    }
}
