using DataModel;
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

            if (RestoreCache(CacheObject.Profile, (long)db.currentUser.fkProfile) is Profile obj)
                return Ok(Parse(obj, id));

            var mdl = db.GetProfile(db.currentUser.fkProfile);

            if (mdl == null)
				return StatusCode(HttpStatusCode.NotFound);

            BackupCache(mdl);

            return Ok(Parse(mdl, id));
		}

        [NonAction]
        public PermTable Parse(Profile mdl, long id)
        {
            return new PermTable()
            {
                listagem = mdl.stPermissions.Contains("|" + id + "1|"),
                visualizar = mdl.stPermissions.Contains("|" + id + "2|"),
                edicao = mdl.stPermissions.Contains("|" + id + "3|"),
                novo = mdl.stPermissions.Contains("|" + id + "4|"),
                remover = mdl.stPermissions.Contains("|" + id + "5|")
            };
        }
	}
}
