using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class ClientGroupController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var filter = new ClientGroupFilter
            {
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
            };

            var parameters = filter.Parameters();

            var hshReport = SetupCacheReport(CacheTags.ClientGroupReport);
            if (hshReport[parameters] is ClientGroupReport report)
                return Ok(report);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = new ClientGroup();

            var results = mdl.ComposedFilters ( db, 
                                                ref reportCount, 
                                                filter, 
                                                bSaveAuditLog:true );

            var ret = new ClientGroupReport
            {
                count = reportCount,
                results = results
            };

            hshReport[parameters] = ret;

            return Ok(ret);
        }

        public IHttpActionResult Get(long id)
        {
            var combo = Request.GetQueryStringValue("combo", false);

            if (RestoreCache(CacheTags.ClientGroup, id) is ClientGroup obj)
                if (combo)
                    return Ok(obj.ClearAssociations());
                else
                    return Ok(obj);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

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
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Create(db, ref apiError))
                return BadRequest(apiError);

            mdl.LoadAssociations(db);

            CleanCache(db, CacheTags.ClientGroup, null);
            StoreCache(CacheTags.ClientGroup, mdl.id, mdl);

            return Ok();
        }

        public IHttpActionResult Put(long id, ClientGroup mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Update(db, ref apiError))
                return BadRequest(apiError);

            mdl.LoadAssociations(db);

            CleanCache(db, CacheTags.ClientGroup, null);
            StoreCache(CacheTags.ClientGroup, mdl.id, mdl);

            return Ok();
        }

        public IHttpActionResult Delete(long id)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetClientGroup(id);

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            if (!mdl.CanDelete(db, ref apiError))
                return BadRequest(apiError);

            mdl.Delete(db);

            CleanCache(db, CacheTags.ClientGroup, null);

            return Ok();
        }
    }
}
