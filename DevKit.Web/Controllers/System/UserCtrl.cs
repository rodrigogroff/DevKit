using DataModel;
using LinqToDB;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Collections.Generic;

namespace DevKit.Web.Controllers
{
	public class UserController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
			using (var db = new DevKitDB())
			{
				var filter = new UserFilter()
				{
					skip = Request.GetQueryStringValue("skip", 0),
					take = Request.GetQueryStringValue("take", 15),
					fkPerfil = Request.GetQueryStringValue<long?>("fkPerfil", null),
					ativo = Request.GetQueryStringValue<bool?>("ativo", null),
					busca = Request.GetQueryStringValue("busca")?.ToUpper()
				};

				var mdl = new User();

				var query = mdl.ComposedFilters(db, filter).
					OrderBy(y => y.stLogin);

				return Ok(new
				{
					count = query.Count(),
					results = Output (query.Skip(() => filter.skip).Take(() => filter.take), db)
				});
			}
		}

		List<User> Output(IQueryable<User> query, DevKitDB db)
		{
			var lst = query.ToList();

			lst.ForEach(mdl => { mdl = mdl.Load(db); });

			return lst;
		}

		public IHttpActionResult Get(long id)
		{
			using (var db = new DevKitDB())
			{
				var model = db.Users.Find(id);

				if (model != null)
					return Ok(model.Load(db));

				return StatusCode(HttpStatusCode.NotFound);
			}
		}

		public IHttpActionResult Post(User mdl)
		{
			using (var db = new DevKitDB())
			{
				var resp = ""; if (!mdl.Create(db, ref resp))
					return BadRequest(resp);

				return Ok(mdl);
			}
		}

		public IHttpActionResult Put(long id, User mdl)
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
				var model = db.Users.Find(id);

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
