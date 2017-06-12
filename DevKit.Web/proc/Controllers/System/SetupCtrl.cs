using DataModel;

using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class SetupController : ApiControllerBase
	{
		public IHttpActionResult Get(long id)
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var obj = RestoreCache(CacheObject.Setup, id);
            if (obj != null)
                return Ok(obj);

            var mdl = db.GetSetup();
				
			if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            mdl.LoadAssociations(db);
            BackupCache(mdl);

            return Ok(mdl);			
		}

		public IHttpActionResult Put(long id, Setup mdl)
		{
            if (!AuthorizeAndStartDatabase(mdl.login))
                return BadRequest();

            if (!mdl.Update(db, ref serviceResponse))
			    return BadRequest(serviceResponse);

            mdl.LoadAssociations(db);

            StoreCache(CacheObject.Setup, id, mdl);

            return Ok();			
		}
	}
}
