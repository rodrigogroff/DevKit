using DataModel;
using System.Linq;
using System;
using System.Web.Http;
using System.Threading;
using System.Security.Claims;
using System.Net;

namespace App.Web.Controllers
{
	public class TabelaPermissao
	{
		public bool listagem = false, visualizar = false, edicao = false,  novo = false,  remover = false;
	}

	public class PermissaoController : ApiControllerBase
	{
		public IHttpActionResult Get(long id)
		{
			using (var db = new SuporteCITDB())
			{
				var identity = Thread.CurrentPrincipal as ClaimsPrincipal;				
				var sid = Convert.ToInt64(identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault());
				var perf = db.Perfils.Find(sid);

				if (perf == null)
					return StatusCode(HttpStatusCode.NotFound);

				var tblPerm = new TabelaPermissao();

				tblPerm.listagem = perf.StPermissoes.Contains("|" + id + "1|");
				tblPerm.visualizar = perf.StPermissoes.Contains("|" + id + "2|");
				tblPerm.edicao = perf.StPermissoes.Contains("|" + id + "3|");
				tblPerm.novo = perf.StPermissoes.Contains("|" + id + "4|");
				tblPerm.remover = perf.StPermissoes.Contains("|" + id + "5|");

				return Ok(tblPerm);								
			}
		}
	}
}
