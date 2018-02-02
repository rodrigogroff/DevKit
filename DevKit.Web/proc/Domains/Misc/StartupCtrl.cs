using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class StartupController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            // atualiza report de data de login...
            CleanCache(db, CacheTags.User, null);

            return Ok();
        }
    }
}
