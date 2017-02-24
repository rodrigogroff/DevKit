using DataModel;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace App.Web.Controllers
{
	public class PerfilController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
			using (var db = new SuporteCITDB())
			{
				var skip = Request.GetQueryStringValue<int?>("skip", null);
				var take = Request.GetQueryStringValue<int?>("take", null);

				var busca = Request.GetQueryStringValue("busca")?.ToUpper();

				var query = from e in db.Perfils select e;

				if (busca != null)
					query = from e in query where e.StNome.ToUpper().Contains(busca) select e;

				query = query.OrderBy(y => y.StNome);

				if (!skip.HasValue || !take.HasValue)
					return Ok(query);

				return Ok(new
				{
					count = query.Count(),
					results = Output(query.Skip(() => skip.Value).Take(() => take.Value), db)
				});
			}
		}

		List<Perfil> Output(IQueryable<Perfil> query, SuporteCITDB db)
		{
			var lst = query.ToList();

			lst.ForEach(mdl => 
			{
				mdl = mdl.Load(db, new PerfilLoad_Params { bQtdUsuarios = true });
			});

			return lst;
		}

		public IHttpActionResult Get(long id)
		{
			using (var db = new SuporteCITDB())
			{
				var model = db.Perfils.Find(id);

				if (model != null)
					return Ok(model.Load(db));

				return StatusCode(HttpStatusCode.NotFound);
			}
		}

		private bool VerificaDuplicado(Perfil item, SuporteCITDB db)
		{
			var query = from e in db.Perfils select e;

			if (item.StNome != null)
				query = from e in query where e.StNome.ToUpper().Contains(item.StNome.ToUpper()) select e;

			if (item.Id > 0)
				query = from e in query where e.Id != item.Id select e;

			return query.Any();
		}

		public IHttpActionResult Post(Perfil mdl)
		{
			using (var db = new SuporteCITDB())
			{
				if (VerificaDuplicado(mdl, db))
					return BadRequest("O perfil informado já existe.");

				mdl.Id = Convert.ToInt64(db.InsertWithIdentity(mdl));

				return Ok(mdl);
			}
		}

		public IHttpActionResult Put(long id, Perfil mdl)
		{
			using (var db = new SuporteCITDB())
			{
				mdl.Id = id;

				if (VerificaDuplicado(mdl, db))
					return BadRequest("O perfil informado já existe.");

				db.Update(mdl);

				return Ok();
			}
		}

		public IHttpActionResult Delete(long id)
		{
			using (var db = new SuporteCITDB())
			{
				var model = db.Perfils.Find(id);

				if (model != null)
				{
					if ((from ne in db.Usuarios where ne.FkPerfil == model.Id select ne).Count() > 0)
						return BadRequest("O perfil informado possui usuários associados.");
					
					db.Delete(model);
				}

				return Ok();
			}
		}
	}
}
