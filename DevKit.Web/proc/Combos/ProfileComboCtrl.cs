using DataModel;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class ProfileComboController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            var filter = new ProfileFilter
            {
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
            };

            var parameters = filter.busca;

            var hshReport = SetupCacheReport(CacheTags.ProfileComboReport);
            if (hshReport[parameters] is ComboReport report)
                return Ok(report);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var ret = new Profile().ComboFilters(db, filter.busca);

            hshReport[parameters] = ret;

            return Ok(ret);
        }

        public IHttpActionResult Get(long id)
		{
            if (RestoreCache(CacheTags.ProfileCombo, id) is BaseComboResponse obj)
                return Ok(obj);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = (from e in db.Profile
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
