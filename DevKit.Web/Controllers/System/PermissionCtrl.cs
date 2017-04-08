using DataModel;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class TabelaPermissao
	{
		public long idUser = 0;

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
				var usr = db.GetCurrentUser();
				var perf = db.Profile(usr.fkProfile);

				if (perf == null)
					return StatusCode(HttpStatusCode.NotFound);

				var tblPerm = new TabelaPermissao();

				tblPerm.idUser = usr.id;
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
