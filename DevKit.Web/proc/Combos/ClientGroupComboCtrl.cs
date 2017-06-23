using DataModel;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class ClientGroupComboController : ApiControllerBase
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

            var hshReport = SetupCacheReport(CacheTags.ClientGroupComboReport);
            if (hshReport[parameters] is ComboReport report)
                return Ok(report);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = new ClientGroup();

            var results = mdl.ComposedFilters ( db, ref reportCount, filter, bSaveAuditLog:false );

            var resultsCombo = new List<BaseComboResponse>();

            foreach (var item in results)
            {
                resultsCombo.Add(new BaseComboResponse
                {
                    id = item.id,
                    stName = item.stName
                });
            }

            var ret = new ComboReport
            {
                count = reportCount,
                results = resultsCombo
            };

            hshReport[parameters] = ret;

            return Ok(ret);
        }

        public IHttpActionResult Get(long id)
        {
            if (RestoreCache(CacheTags.ClientGroupCombo, id) is BaseComboResponse obj)
                return Ok(obj);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetClientGroup(id);

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            BackupCache(new BaseComboResponse
            {
                id = mdl.id,
                stName = mdl.stName
            });

            return Ok(mdl);
        }
    }
}
