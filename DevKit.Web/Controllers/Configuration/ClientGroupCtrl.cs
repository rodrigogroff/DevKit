using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class ClientGroupController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var mdl = new ClientGroup();

            var filter = new ClientGroupFilter
            {
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
            };

            var hshReport = SetupCacheReport(CacheObject.ClientGroupReports);
            if (hshReport[filter.Parameters()] is ClientGroupReport report)
                return Ok(report);

            var results = mdl.ComposedFilters(db, ref count, filter);

            var ret = new ClientGroupReport
            {
                count = count,
                results = results
            };

            hshReport[filter.Parameters()] = ret;

            return Ok(ret);
        }

        public IHttpActionResult Get(long id)
        {
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var obj = RestoreCache(CacheObject.ClientGroup + id);
            if (obj != null)
                return Ok(obj);

            var model = db.GetClientGroup(id);

            if (model == null)
                return StatusCode(HttpStatusCode.NotFound);

            var combo = Request.GetQueryStringValue("combo", false);
            if (combo)
                return Ok(model);

            BackupCache(model);

            return Ok(model.LoadAssociations(db));
        }

        public IHttpActionResult Post(ClientGroup mdl)
        {
            if (!AuthorizeAndStartDatabase(mdl.login))
                return BadRequest();

            if (!mdl.Create(db, ref serviceResponse))
                return BadRequest(serviceResponse);

            SetupCacheReport(CacheObject.ClientGroupReports).Clear();
            StoreCache(CacheObject.ClientGroup + mdl.id, mdl);

            return Ok();
        }

        public IHttpActionResult Put(long id, ClientGroup mdl)
        {
            if (!AuthorizeAndStartDatabase(mdl.login))
                return BadRequest();

            if (!mdl.Update(db, ref serviceResponse))
                return BadRequest(serviceResponse);

            SetupCacheReport(CacheObject.ClientGroupReports).Clear();
            StoreCache(CacheObject.ClientGroup + mdl.id, mdl);

            return Ok();
        }

        public IHttpActionResult Delete(long id)
        {
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var model = db.GetClientGroup(id);

            if (model == null)
                return StatusCode(HttpStatusCode.NotFound);

            if (!model.CanDelete(db, ref serviceResponse))
                return BadRequest(serviceResponse);

            model.Delete(db);

            SetupCacheReport(CacheObject.TaskReports).Clear();

            return Ok();
        }
    }
}
