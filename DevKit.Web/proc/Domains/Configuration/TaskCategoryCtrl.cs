﻿using DataModel;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class TaskCategoryController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            var filter = new TaskCategoryFilter
            {
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
                busca = Request.GetQueryStringValue("busca")?.ToUpper(),
                fkTaskType = Request.GetQueryStringValue<long?>("fkTaskType", null)
            };

            var parameters = filter.busca;

            if (filter.fkTaskType != null)
                parameters += "," + filter.fkTaskType;
            
            var hshReport = SetupCacheReport(CacheTags.TaskCategoryComboReport);
            if (hshReport[parameters] is TaskCategoryReport report)
                return Ok(report);

            if (!StartDatabaseAndAuthorize())
                return BadRequest(); 

            var ret = new TaskCategory().ComposedFilters(db, filter);

            hshReport[parameters] = ret;

            return Ok(ret);
        }

        public IHttpActionResult Get(long id)
        {
            if (RestoreCache(CacheTags.TaskCategoryCombo, id) is BaseComboResponse obj)
                return Ok(obj);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = (from e in db.TaskCategory
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
