using DataModel;
using LinqToDB;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Collections.Generic;
using System;

namespace App.Web.Controllers
{
	public class UsuarioController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
			using (var db = new SuporteCITDB())
			{
				var skip = Request.GetQueryStringValue<int?>("skip", null);
				var take = Request.GetQueryStringValue<int?>("take", null);
				var fkPerfil = Request.GetQueryStringValue<int?>("fkPerfil", null);

				var ativo = Request.GetQueryStringValue<bool?>("ativo", null);

				var busca = Request.GetQueryStringValue("busca")?.ToUpper();

				var query = from e in db.Usuarios select e;

				if (ativo != null)
					query = from e in query where e.bAtivo == ativo select e;

				if (busca != null)
					query = from e in query where e.StLogin.ToUpper().Contains(busca) select e;

				if (fkPerfil != null)
					query = from e in query where e.FkPerfil == fkPerfil select e;

				query = query.OrderBy(y => y.StLogin);

				if (!skip.HasValue || !take.HasValue)
					return Ok(query);

				return Ok(new
				{
					count = query.Count(),
					results = Output (query.Skip(() => skip.Value).Take(() => take.Value), db)
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

		private bool VerificaDuplicado(Usuario item, SuporteCITDB db)
		{
			var query = from e in db.Usuarios select e;

			if (item.bAtivo != null)
				query = from e in query where e.bAtivo == item.bAtivo select e;

			if (item.StLogin != null)
				query = from e in query where e.StLogin.ToUpper().Contains(item.StLogin.ToUpper()) select e;

			if (item.Id > 0)
				query = from e in query where e.Id != item.Id select e;

			return query.Any();
		}

		public IHttpActionResult Post(Usuario mdl)
		{
			using (var db = new SuporteCITDB())
			{
				if (VerificaDuplicado(mdl, db))
					return BadRequest("O login informado já existe.");

				mdl.Id = Convert.ToInt64(db.InsertWithIdentity(mdl));

				return Ok(mdl);
			}
		}

		public IHttpActionResult Put(long id, Usuario mdl)
		{
			using (var db = new SuporteCITDB())
			{
				mdl.Id = id;

				if (VerificaDuplicado(mdl, db))
					return BadRequest("O login informado já existe.");

				var resp = "";

				if (!mdl.Update(db, ref resp))
					return BadRequest(resp);

				return Ok();
			}
		}

		public void Delete(long id)
		{
			using (var db = new SuporteCITDB())
			{
				var model = db.Usuarios.Find(id);

				if (model != null)
					db.Delete(model);
			}
		}
	}
}
