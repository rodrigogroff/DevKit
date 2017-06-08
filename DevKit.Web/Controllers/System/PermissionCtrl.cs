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
            var login = GetLoginInfo();
            
            using (var db = new DevKitDB())
			{
				var usr = db.GetCurrentUser(login.idUser);
				var perf = db.GetProfile(usr.fkProfile);

				if (perf == null)
					return StatusCode(HttpStatusCode.NotFound);

                return Ok(new TabelaPermissao()
                {
                    idUser = usr.id,
                    listagem = perf.stPermissions.Contains("|" + id + "1|"),
                    visualizar = perf.stPermissions.Contains("|" + id + "2|"),
                    edicao = perf.stPermissions.Contains("|" + id + "3|"),
                    novo = perf.stPermissions.Contains("|" + id + "4|"),
                    remover = perf.stPermissions.Contains("|" + id + "5|")
                });								
			}
		}
	}
}
