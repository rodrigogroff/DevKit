using DevKit.Web.Services;
using System.Web;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class StartupController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var myApp = HttpContext.Current.Application;

            if (myApp["start"] == null)
            {
                System.Threading.Tasks.Task.Run(() => { new StartupPreCacheService().Run(myApp); });
           //     System.Threading.Tasks.Task.Run(() => { new CacheControlService().Run(myApp); });  

                myApp["start"] = true;
            }           

            return Ok();
        }
    }
}
