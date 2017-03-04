using DataModel;
using LinqToDB;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace App.Web.Controllers
{
	public class ProfileController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
			using (var db = new DevKitDB())
			{
				var filter = new ProfileFilter()
				{
					skip = Request.GetQueryStringValue("skip", 0),
					take = Request.GetQueryStringValue("take", 15),
					busca = Request.GetQueryStringValue("busca")?.ToUpper()
				};

				var mdl = new Profile();

				var query = mdl.ComposedFilters(db, filter).
					OrderBy(y => y.stName);

				return Ok(new
				{
					count = query.Count(),
					results = Output(query.Skip(() => filter.skip).Take(() => filter.take), db)
				});
			}
		}

		List<Profile> Output(IQueryable<Profile> query, DevKitDB db)
		{
			var lst = query.ToList();

			lst.ForEach(mdl => { mdl = mdl.Load(db, new ProfileLoad_Params { bQttyUsers = true }); });

			return lst;
		}

		public IHttpActionResult Get(long id)
		{
			using (var db = new DevKitDB())
			{
				var model = db.Profiles.Find(id);

				if (model != null)
					return Ok(model.Load(db));

				return StatusCode(HttpStatusCode.NotFound);
			}
		}
		
		public IHttpActionResult Post(Profile mdl)
		{
			using (var db = new DevKitDB())
			{
				var resp = ""; if (!mdl.Create(db, ref resp))
					return BadRequest(resp);

				return Ok(mdl);
			}
		}

		public IHttpActionResult Put(long id, Profile mdl)
		{
			using (var db = new DevKitDB())
			{
				var resp = ""; if (!mdl.Update(db, ref resp))
					return BadRequest(resp);

				return Ok(mdl);
			}
		}

		public IHttpActionResult Delete(long id)
		{
			using (var db = new DevKitDB())
			{
				var model = db.Profiles.Find(id);

				if (model == null)
					return StatusCode(HttpStatusCode.NotFound);

				var resp = ""; if (!model.CanDelete(db, ref resp))
					return BadRequest(resp);

				db.Delete(model);

				return Ok();
			}
		}
	}
}
