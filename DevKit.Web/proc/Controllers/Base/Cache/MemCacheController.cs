using DataModel;
using LinqToDB;
using System.Collections;
using System.Collections.Generic;
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
            myApplication.Lock();

            StoreTag(currentCacheTag);
            myApplication[currentCacheTag] = obj;

            myApplication.UnLock();
        }

        [NonAction]
        public List<string> GetCacheTags()
        {
            return myApplication["%lstTags"] as List<string>;
        }

        [NonAction]
        public void StoreTag(string tag)
        {
            var lstTags = myApplication["%lstTags"] as List<string>;

            if (lstTags == null)
            {
                lstTags = new List<string>();
                myApplication["%lstTags"] = lstTags;
            }

            if (!lstTags.Contains(tag))
                lstTags.Add(tag);
        }

        [NonAction]
        public void RemoveTag(string tag)
        {
            var lstTags = myApplication["%lstTags"] as List<string>;

            if (lstTags != null)
                lstTags.Remove(tag);
        }

        [NonAction]
        public void StoreCache(string tag, long? id, object obj)
        {
            myApplication.Lock();

            tag = tag + id;

            StoreTag(tag);
            myApplication[tag] = obj;

            myApplication.UnLock();
        }

        [NonAction]
        public object RestoreCache(string tag, long id)
        {
            currentCacheTag = tag + id;

            myApplication.Lock();

            var ret = myApplication[currentCacheTag] as object;

            myApplication.UnLock();

            return ret;
        }

        [NonAction]
        public Hashtable SetupCacheReport(string tag)
        {
            myApplication.Lock();

            var hsh = myApplication[tag] as Hashtable;

            if (hsh == null)
            {
                hsh = new Hashtable();

                StoreTag(tag);
                myApplication[tag] = hsh;
            }

            myApplication.UnLock();

            return hsh;
        }

        [NonAction]
        public void CleanCacheReport(string tag)
        {
            myApplication.Lock();

            var hsh = myApplication[tag] as Hashtable;

            if (hsh != null)
            {
                hsh.Clear();

                RemoveTag(tag);
                myApplication[tag] = null;                
            }

            myApplication.UnLock();
        }

        [NonAction]
        public void CleanCache(DevKitDB db, string tag, long? id)
        {
            if (id != null)
                StoreCache(tag, id, null);

            db.Insert(new CacheControl
            {
                fkTarget = id,
                stEntity = tag
            });
        }
    }
}
