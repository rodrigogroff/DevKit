using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class PermTable
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
            if (!AuthorizeAndStartDatabase())
                return BadRequest();
            
            var usr = db.currentUser;
			var perf = db.GetProfile(usr.fkProfile);

            if (perf == null)
				return StatusCode(HttpStatusCode.NotFound);

            return Ok(new PermTable()
            {
                listagem = perf.stPermissions.Contains("|" + id + "1|"),
                visualizar = perf.stPermissions.Contains("|" + id + "2|"),
                edicao = perf.stPermissions.Contains("|" + id + "3|"),
                novo = perf.stPermissions.Contains("|" + id + "4|"),
                remover = perf.stPermissions.Contains("|" + id + "5|")
            });			
		}
	}
}
