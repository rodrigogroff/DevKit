using DataModel;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class SituacoesController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            string busca = Request.GetQueryStringValue("busca", "").ToUpper();

            var hshReport = SetupCacheReport(CacheTags.EnumSituacoesReport);
            if (hshReport[busca] is SituacaoReport report)
                return Ok(report);
            
            var query = (from e in new EnumSituacao().lst select e);

			if (busca != "")
				query = from e in query where e.stName.ToUpper().Contains(busca) select e;

            var ret = new SituacaoReport
            {
                count = query.Count(),
                results = query.ToList()
            };

            hshReport[busca] = ret;

            return Ok(ret);
		}

		public IHttpActionResult Get(long id)
		{
           // var obj = RestoreCache(CacheTags.EnumMonth, id);
            //if (obj != null)
              //  return Ok(obj);

            var mdl = new EnumSituacao().Get(id);

			if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            //BackupCache(mdl);

            return Ok(mdl);
		}
	}
}
