using DataModel;
using LinqToDB;
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

        // save id list from tag

        [NonAction]
        public void StoreCache(string tag, long? id, object obj)
        {
            myApplication[tag + id] = obj;
        }

        [NonAction]
        public object RestoreCache(string tag, long id)
        {
            currentCacheTag = tag + id;

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

        [NonAction]
        public void CleanCacheReport(string tag)
        {
            var hsh = myApplication[tag] as Hashtable;

            if (hsh != null)
            {
                hsh.Clear();
                myApplication[tag] = null;                
            }           
        }

        [NonAction]
        public void CleanCache(DevKitDB db, string tag, long? id)
        {
            db.Insert(new CacheControl
            {
                fkTarget = id,
                stEntity = tag
            });
        }
    }
}
