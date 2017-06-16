using DataModel;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class HomeViewController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();
            
            var mdl = new HomeView();
				
			return Ok(mdl.ComposedFilters(db));			
		}
	}
}
