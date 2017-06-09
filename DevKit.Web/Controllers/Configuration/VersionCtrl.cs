﻿using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class VersionController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var mdl = new ProjectSprintVersion();

			var results = mdl.ComposedFilters(db, ref count, new ProjectSprintVersionFilter
			{
				skip = Request.GetQueryStringValue("skip", 0),
				take = Request.GetQueryStringValue("take", 15),
				busca = Request.GetQueryStringValue("busca")?.ToUpper(),
				fkSprint = Request.GetQueryStringValue<int?>("fkSprint", null),
			});

			return Ok(new { count = count, results = results });
		}

		public IHttpActionResult Get(long id)
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var model = db.GetProjectSprintVersion(id);

			if (model != null)
				return Ok(model);

			return StatusCode(HttpStatusCode.NotFound);
		}
	}
}
