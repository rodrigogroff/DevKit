using System.Web;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public static class CachedObject
    {
        public const string User = "User";
    }

	[Authorize]
	public class MemCacheController : ApiController
	{
        public HttpApplicationState myApplication;

        [NonAction]
        public object RestoreCache(string tag)
        {
            return myApplication[tag] as object;
        }

        [NonAction]
        public void BackupCache(string tag, object obj)
        {
            myApplication[tag] = obj;
        }
    }
}
