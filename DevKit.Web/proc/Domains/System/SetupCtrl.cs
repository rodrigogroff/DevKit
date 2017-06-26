using DataModel;

using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class SetupController : ApiControllerBase
	{
		public IHttpActionResult Get(long id)
		{
            var obj = RestoreCache(CacheTags.Setup, id);
            if (obj != null)
                return Ok(obj);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.GetSetup();
				
			if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            mdl.LoadAssociations(db);

            BackupCache(mdl);

            return Ok(mdl);			
		}

		public IHttpActionResult Put(long id, Setup mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Update(db, ref apiError))
			    return BadRequest(apiError);

            mdl.LoadAssociations(db);

            StoreCache(CacheTags.Setup, id, mdl);

            return Ok();			
		}
	}
}
