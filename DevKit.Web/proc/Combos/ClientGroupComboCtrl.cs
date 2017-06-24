﻿using DataModel;
using System.Linq;
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
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
            };

            var parameters = filter.Parameters();

            var hshReport = SetupCacheReport(CacheTags.ClientGroupComboReport);
            if (hshReport[parameters] is ComboReport report)
                return Ok(report);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var ret = new ClientGroup().ComboFilters ( db, filter.busca );

            hshReport[parameters] = ret;

            return Ok(ret);
        }

        public IHttpActionResult Get(long id)
        {
            if (RestoreCache(CacheTags.ClientGroupCombo, id) is BaseComboResponse obj)
                return Ok(obj);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = (from e in db.ClientGroup
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
