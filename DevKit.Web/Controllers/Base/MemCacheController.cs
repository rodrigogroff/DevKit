using System.Collections;
using System.Web;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public static class CachedObject
    {
        public const string User = "User";
        public const string TaskReports = "TaskReports";
    }

	[Authorize]
	public class MemCacheController : ApiController
	{
        public HttpApplicationState myApplication;

        [NonAction]
        public void BackupCache(string tag, object obj)
        {
            myApplication[tag] = obj;
        }

        [NonAction]
        public object RestoreCache(string tag)
        {
            return myApplication[tag] as object;
        }

        [NonAction]
        public Hashtable SetupCacheReport(string tag)
        {
            var hsh = myApplication[tag] as Hashtable;

            if (hsh == null)
            {
                hsh = new Hashtable();
                myApplication[tag] = hsh;
            }               

            return hsh;
        }
    }
}
