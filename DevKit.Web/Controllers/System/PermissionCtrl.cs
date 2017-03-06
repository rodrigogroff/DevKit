using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class TabelaPermissao
	{
		public bool listagem = false,
					visualizar = false,
					edicao = false, 
					novo = false,
					remover = false;
	}

	public class PermissionController : ApiControllerBase
	{
		public IHttpActionResult Get(long id)
		{
			using (var db = new DevKitDB())
			{
				var usr = new Util().GetCurrentUser(db);
				var perf = db.Profiles.Find((long)usr.fkProfile);

				if (perf == null)
					return StatusCode(HttpStatusCode.NotFound);

				var tblPerm = new TabelaPermissao();

				tblPerm.listagem = perf.stPermissions.Contains("|" + id + "1|");
				tblPerm.visualizar = perf.stPermissions.Contains("|" + id + "2|");
				tblPerm.edicao = perf.stPermissions.Contains("|" + id + "3|");
				tblPerm.novo = perf.stPermissions.Contains("|" + id + "4|");
				tblPerm.remover = perf.stPermissions.Contains("|" + id + "5|");

				return Ok(tblPerm);								
			}
		}
	}
}
