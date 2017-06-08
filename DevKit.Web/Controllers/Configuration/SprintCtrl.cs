using DataModel;

using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class SprintController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            var login = GetLoginInfo();

            using (var db = new DevKitDB())
			{
				var count = 0; var mdl = new ProjectSprint();

				var results = mdl.ComposedFilters(db, ref count, new ProjectSprintFilter()
				{
					skip = Request.GetQueryStringValue("skip", 0),
					take = Request.GetQueryStringValue("take", 15),
					busca = Request.GetQueryStringValue("busca")?.ToUpper(),
                    fkCurrentUser = login.idUser,
                    fkProject = Request.GetQueryStringValue<long?>("fkProject", null),
					fkPhase = Request.GetQueryStringValue<long?>("fkPhase", null),
				});

				return Ok(new { count = count, results = results });
			}
		}

		public IHttpActionResult Get(long id)
		{
			using (var db = new DevKitDB())
			{
				var model = db.GetProjectSprint(id);

                if (model != null)
                {
                    var combo = Request.GetQueryStringValue("combo", false);

                    if (combo)
                        return Ok(model);

                    return Ok(model.LoadAssociations(db));
                }

                return StatusCode(HttpStatusCode.NotFound);
			}
		}

		public IHttpActionResult Put(long id, ProjectSprint mdl)
		{
            using (var db = new DevKitDB())
			{
				var resp = "";

				if (!mdl.Update(db, mdl.login.idUser, ref resp))
					return BadRequest(resp);

				return Ok(mdl);
			}
		}
	}
}
