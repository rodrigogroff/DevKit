using DataModel;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class OrdemRelLojistaTransController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            string busca = Request.GetQueryStringValue("busca", "").ToUpper();

            var hshReport = SetupCacheReport(CacheTags.EnumOrdemRelLojistaTransReport);
            if (hshReport[busca] is OrdemRelLojistaTransReport report)
                return Ok(report);
            
            var query = (from e in new EnumOrdemRelLojistaTrans().lst select e);

			if (busca != "")
				query = from e in query where e.stName.ToUpper().Contains(busca) select e;

            var ret = new OrdemRelLojistaTransReport
            {
                count = query.Count(),
                results = query.ToList()
            };

            hshReport[busca] = ret;

            return Ok(ret);
		}

		public IHttpActionResult Get(long id)
		{
         //   var obj = RestoreCache(CacheTags.EnumOrdemRelLojistaTrans, id);
           // if (obj != null)
             //   return Ok(obj);

            var mdl = new EnumOrdemRelLojistaTrans().Get(id);

			if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            //BackupCache(mdl);

            return Ok(mdl);
		}
	}
}
