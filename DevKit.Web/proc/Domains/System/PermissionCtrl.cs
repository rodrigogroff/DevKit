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
            // ------------------------------
            // try for pure cache
            // ------------------------------

            var currentUser = RestoreCacheNoHit(CacheTags.User + userLoggedName) as User;

            if (currentUser != null)
                if (RestoreCache(CacheTags.Profile, currentUser.fkProfile) is Profile objProfile)
                    return Ok(Parse(objProfile, id));

            // ------------------------------
            // try for database
            // ------------------------------

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var userProfile = db.currentUser.fkProfile;

            if (RestoreCache(CacheTags.Profile, userProfile) is Profile obj)
                return Ok(Parse(obj, id));
            
            var mdl = db.GetProfile(userProfile);

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
