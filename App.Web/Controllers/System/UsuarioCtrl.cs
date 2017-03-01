using DataModel;
using LinqToDB;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Collections.Generic;

namespace App.Web.Controllers
{
	public class UsuarioController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
			using (var db = new SuporteCITDB())
			{
				var filter = new UsuarioFilter()
				{
					skip = Request.GetQueryStringValue("skip", 0),
					take = Request.GetQueryStringValue("take", 15),
					fkPerfil = Request.GetQueryStringValue<long?>("fkPerfil", null),
					ativo = Request.GetQueryStringValue<bool?>("ativo", null),
					busca = Request.GetQueryStringValue("busca")?.ToUpper()
				};

				var mdl = new Usuario();

				var query = mdl.ComposedFilters(db, filter).
					OrderBy(y => y.StLogin);

				return Ok(new
				{
					count = query.Count(),
					results = Output (query.Skip(() => filter.skip).Take(() => filter.take), db)
				});
			}
		}

		List<Usuario> Output(IQueryable<Usuario> query, SuporteCITDB db)
		{
			var lst = query.ToList();

			lst.ForEach(mdl => { mdl = mdl.Load(db); });

			return lst;
		}

		public IHttpActionResult Get(long id)
		{
			using (var db = new SuporteCITDB())
			{
				var model = db.Usuarios.Find(id);

				if (model != null)
					return Ok(model.Load(db));

				return StatusCode(HttpStatusCode.NotFound);
			}
		}

		public IHttpActionResult Post(Usuario mdl)
		{
			using (var db = new SuporteCITDB())
			{
				var resp = ""; if (!mdl.Create(db, ref resp))
					return BadRequest(resp);

				return Ok(mdl);
			}
		}

		public IHttpActionResult Put(long id, Usuario mdl)
		{
			using (var db = new SuporteCITDB())
			{
				var resp = ""; if (!mdl.Update(db, ref resp))
					return BadRequest(resp);

				return Ok(mdl);
			}
		}

		public IHttpActionResult Delete(long id)
		{
			using (var db = new SuporteCITDB())
			{
				var model = db.Usuarios.Find(id);

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
