using DataModel;

using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class ClientController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            var login = GetLoginInfo();

            using (var db = new DevKitDB())
			{
				var count = 0; var mdl = new Client();

				var results = mdl.ComposedFilters(db, ref count, new ClientFilter
				{
					skip = Request.GetQueryStringValue("skip", 0),
					take = Request.GetQueryStringValue("take", 15),
                    fkCurrentUser = login.idUser,
                    busca = Request.GetQueryStringValue("busca")?.ToUpper(),
				});

				return Ok(new { count = count, results = results });
			}
		}

		public IHttpActionResult Get(long id)
		{
			using (var db = new DevKitDB())
			{
				var model = db.GetClient(id);

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

		public IHttpActionResult Post(Client mdl)
		{
            using (var db = new DevKitDB())
			{
				var resp = "";

				if (!mdl.Create(db, mdl.login.idUser, ref resp))
					return BadRequest(resp);

				return Ok(mdl);
			}
		}

		public IHttpActionResult Put(long id, Client mdl)
		{
            using (var db = new DevKitDB())
			{
				var resp = "";

				if (!mdl.Update(db, mdl.login.idUser, ref resp))
					return BadRequest(resp);

				return Ok(mdl);				
			}
		}

		public IHttpActionResult Delete(long id)
		{
            var login = GetLoginInfo();

            using (var db = new DevKitDB())
			{
				var model = db.GetClient(id);

				if (model == null)
					return StatusCode(HttpStatusCode.NotFound);

				var resp = "";

				if (!model.CanDelete(db, ref resp))
					return BadRequest(resp);

				model.Delete(db, login.idUser);
								
				return Ok();
			}
		}
	}
}
