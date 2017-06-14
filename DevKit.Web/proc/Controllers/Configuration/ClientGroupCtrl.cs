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

            var results = mdl.ComposedFilters(db, ref count, filter, true);

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
            var combo = Request.GetQueryStringValue("combo", false);

            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var obj = RestoreCache(CacheObject.ClientGroup, id) as ClientGroup;
            if (obj != null)
                if (combo)
                    return Ok(obj.ClearAssociations());
                else
                    return Ok(obj);

            var mdl = db.GetClientGroup(id);

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            mdl.LoadAssociations(db);
            BackupCache(mdl);

            if (combo)
                return Ok(mdl.ClearAssociations());
            else
                return Ok(mdl);
        }

        public IHttpActionResult Post(ClientGroup mdl)
        {
            if (!AuthorizeAndStartDatabase(mdl.login))
                return BadRequest();

            if (!mdl.Create(db, ref serviceResponse))
                return BadRequest(serviceResponse);

            mdl.LoadAssociations(db);

            CleanCache(db, CacheObject.ClientGroup, null);
            StoreCache(CacheObject.ClientGroup, mdl.id, mdl);

            return Ok();
        }

        public IHttpActionResult Put(long id, ClientGroup mdl)
        {
            if (!AuthorizeAndStartDatabase(mdl.login))
                return BadRequest();

            if (!mdl.Update(db, ref serviceResponse))
                return BadRequest(serviceResponse);

            mdl.LoadAssociations(db);

            StoreCache(CacheObject.ClientGroup, mdl.id, mdl);

            return Ok();
        }

        public IHttpActionResult Delete(long id)
        {
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var mdl = db.GetClientGroup(id);

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            if (!mdl.CanDelete(db, ref serviceResponse))
                return BadRequest(serviceResponse);

            mdl.Delete(db);

            CleanCache(db, CacheObject.ClientGroup, null);

            return Ok();
        }
    }
}
