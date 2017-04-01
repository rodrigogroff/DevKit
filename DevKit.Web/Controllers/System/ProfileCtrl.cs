using DataModel;
using LinqToDB;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
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

				var query = mdl.ComposedFilters(db, filter).OrderBy(y => y.stName);

				var results = (query.Skip(() => filter.skip).Take(() => filter.take)).ToList();

				results.ForEach(y => { y = y.LoadAssociations(db); });

				return Ok(new { count = query.Count(), results = results });
			}
		}
		
		public IHttpActionResult Get(long id)
		{
			using (var db = new DevKitDB())
			{
				var model = (from ne in db.Profiles select ne).
					Where(t => t.id == id).
					FirstOrDefault();

				if (model != null)
					return Ok(model.LoadAssociations(db));

				return StatusCode(HttpStatusCode.NotFound);
			}
		}
		
		public IHttpActionResult Post(Profile mdl)
		{
			using (var db = new DevKitDB())
			{
				var resp = "";

				if (!mdl.Create(db, ref resp))
					return BadRequest(resp);

				return Ok(mdl);
			}
		}

		public IHttpActionResult Put(long id, Profile mdl)
		{
			using (var db = new DevKitDB())
			{
				var resp = "";

				if (!mdl.Update(db, ref resp))
					return BadRequest(resp);

				return Ok(mdl);
			}
		}

		public IHttpActionResult Delete(long id)
		{
			using (var db = new DevKitDB())
			{
				var model = (from ne in db.Profiles select ne).
					Where(t => t.id == id).
					FirstOrDefault();
				
				if (model == null)
					return StatusCode(HttpStatusCode.NotFound);

				var resp = "";

				if (!model.CanDelete(db, ref resp))
					return BadRequest(resp);

				model.Delete(db);
				
				return Ok();
			}
		}
	}
}
