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

            var myApp = HttpContext.Current.Application;

            if (myApp["start"] == null)
            {
                System.Threading.Tasks.Task.Run(() => { new StartupPreCacheService().Run(myApp, db.currentUser); });
                System.Threading.Tasks.Task.Run(() => { new CacheControlService().Run(myApp); });

                myApp["start"] = true;
            }

            var mdl = new HomeView();
				
			return Ok(mdl.ComposedFilters(db));			
		}
	}
}
