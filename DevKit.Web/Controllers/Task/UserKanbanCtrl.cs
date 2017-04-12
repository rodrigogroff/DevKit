﻿using DataModel;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class UserKanbanController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
			using (var db = new DevKitDB())
			{
				var mdl = new UserKanban();
				
				return Ok(mdl.ComposedFilters(db, new UserKanbanFilter()
				{
					busca = Request.GetQueryStringValue("busca")?.ToUpper(),
					complete = Request.GetQueryStringValue<bool?>("complete", null),
					nuPriority = Request.GetQueryStringValue<long?>("nuPriority", null),
					fkProject = Request.GetQueryStringValue<long?>("fkProject", null),
					fkPhase = Request.GetQueryStringValue<long?>("fkPhase", null),
					fkSprint = Request.GetQueryStringValue<long?>("fkSprint", null),
					fkTaskType = Request.GetQueryStringValue<long?>("fkTaskType", null),
					fkTaskCategory = Request.GetQueryStringValue<long?>("fkTaskCategory", null),
					fkTaskFlowCurrent = Request.GetQueryStringValue<long?>("fkTaskFlowCurrent", null),
					fkUserStart = Request.GetQueryStringValue<long?>("fkUserStart", null),
					fkUserAssigned = Request.GetQueryStringValue<long?>("fkUserAssigned", null)
				}));				
			}
		}
	}
}
