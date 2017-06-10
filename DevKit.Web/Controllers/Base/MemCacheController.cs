using System.Collections;
using System.Web;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	[Authorize]
	public class MemCacheController : ApiController
	{
        public HttpApplicationState myApplication;

        public string currentCacheTag = "";

        [NonAction]
        public void BackupCache(object obj)
        {
            myApplication[currentCacheTag] = obj;
        }

        [NonAction]
        public void StoreCache(string tag, object obj)
        {
            myApplication[tag] = obj;
        }

        [NonAction]
        public object RestoreCache(string tag)
        {
            currentCacheTag = tag;

            return myApplication[currentCacheTag] as object;
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
