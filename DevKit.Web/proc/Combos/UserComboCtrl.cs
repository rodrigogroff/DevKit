using DataModel;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class UserComboController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            var filter = new UserFilter
            {
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
            };

            var parameters = filter.busca;

            var hshReport = SetupCacheReport(CacheTags.UserComboReport);
            if (hshReport[parameters] is ComboReport report)
                return Ok(report);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var ret = new User().ComboFilters(db, filter.busca);

            hshReport[parameters] = ret;

            return Ok(ret);
        }

        public IHttpActionResult Get(long id)
		{
            if (RestoreCache(CacheTags.UserCombo, id) is BaseComboResponse obj)
                return Ok(obj);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = (from e in db.User
                       where e.id == id
                       select new BaseComboResponse
                       {
                           id = e.id,
                           stName = e.stLogin
                       }).
                       FirstOrDefault();

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            BackupCache(mdl);

            return Ok(mdl);
        }
    }
}
