using DataModel;
using LinqToDB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class CacheHitRecord
    {
        public DateTime dt_last;
        public int hits = 0;
    }

	[Authorize]
	public class MemCacheController : ApiController
	{
        public HttpApplicationState myApplication = HttpContext.Current.Application;

        public string currentCacheTag = "";

        // ---------------
        // initialization
        // ---------------

        [NonAction]
        public void StartCache()
        {
            var lstTags = new List<string>();
            var hshHits = new Hashtable();

            myApplication["%lstTags"] = lstTags;
            myApplication["%hshHits"] = hshHits;
        }

        // ---------------
        // retrieve first
        // ---------------

        [NonAction]
        public object RestoreCache(string tag, long id)
        {
            currentCacheTag = tag + id;

            var ret = myApplication[currentCacheTag] as object;

            if (ret != null)
                SaveHit(tag);

            return ret;
        }               

        [NonAction]
        public Hashtable SetupCacheReport(string tag)
        {
            var hsh = myApplication[tag] as Hashtable;

            if (hsh == null)
            {
                hsh = new Hashtable();

                StoreTag(tag);
                myApplication[tag] = hsh;
            }

            SaveHit(tag);

            return hsh;
        }

        // ---------------
        // save results
        // ---------------

        [NonAction]
        public void BackupCache(object obj)
        {
            StoreTag(currentCacheTag);
            myApplication[currentCacheTag] = obj;
        }

        [NonAction]
        public void StoreCache(string tag, long? id, object obj)
        {
            tag = tag + id;

            if (obj != null)
                StoreTag(tag);
            else
                RemoveTag(tag);
            
            myApplication[tag] = obj;
        }
        
        // ---------------
        // cleanup
        // ---------------

        [NonAction]
        public void CleanCacheReport(string tag)
        {
            if (myApplication[tag] is Hashtable hsh)
            {
                hsh.Clear();

                RemoveTag(tag);
                myApplication[tag] = null;
            }            
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

        // ---------------
        // control
        // ---------------

        [NonAction]
        public List<string> GetCacheTags()
        {
            return myApplication["%lstTags"] as List<string>;
        }

        [NonAction]
        public Hashtable GetCacheHitRecord()
        {
            return myApplication["%hshHits"] as Hashtable;
        }

        [NonAction]
        public void StoreTag(string tag)
        {
            var lstTags = myApplication["%lstTags"] as List<string>;

            if (!lstTags.Contains(tag))
                lstTags.Add(tag);
        }

        [NonAction]
        public void RemoveTag(string tag)
        {
            (myApplication["%lstTags"] as List<string>).Remove(tag);
            (myApplication["%hshHits"] as Hashtable)[tag] = null;
        }

        [NonAction]
        public void SaveHit(string tag)
        {
            var hshHits = GetCacheHitRecord();

            var obj = hshHits[tag] as CacheHitRecord;

            if (obj == null)
            {
                hshHits[tag] = new CacheHitRecord { dt_last = DateTime.Now, hits = 1 };
            }
            else
            {
                obj.hits++;
                obj.dt_last = DateTime.Now;
            }
        }
    }
}
