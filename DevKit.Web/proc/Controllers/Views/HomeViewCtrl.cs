using DataModel;
using DevKit.Web.Services;

using System.Web;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class HomeViewController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            myApplication = HttpContext.Current.Application;

            if (myApplication["start"] == null)
            {
                myApplication["start"] = true;

                StartCache();

                System.Threading.Tasks.Task.Run(() => { new StartupPreCacheService().Run(myApplication, db.currentUser); });
                System.Threading.Tasks.Task.Run(() => { new CacheControlService().Run(myApplication); });
            }
            
            var mdl = new HomeView();
				
			return Ok(mdl.ComposedFilters(db));			
		}
	}
}
