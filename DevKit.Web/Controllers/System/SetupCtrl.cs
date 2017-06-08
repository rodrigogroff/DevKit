using DataModel;

using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class SetupController : ApiControllerBase
	{
		public IHttpActionResult Get(long id)
		{
            var login = GetLoginInfo();

            using (var db = new DevKitDB())
			{
                // validate login!

				var model = db.GetSetup();
				
				if (model != null)
					return Ok(model.LoadAssociations(db));

				return StatusCode(HttpStatusCode.NotFound);
			}
		}

		public IHttpActionResult Put(long id, Setup mdl)
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
