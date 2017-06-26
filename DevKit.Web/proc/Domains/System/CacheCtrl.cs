using System.Collections.Generic;
using System.Web.Http;
using System.Linq;

namespace DevKit.Web.Controllers
{
    public class CacheDTO
    {
        public string key, last;
        public int hits;
    }
    
    public class CacheController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var lstTags = GetCacheTags();
            var hsh = GetCacheHitRecord();

            var ret = new List<CacheDTO>();

            int totalHits = 0;

            foreach (var key in lstTags)
            {
                if (hsh[key] is CacheHitRecord hr)
                {                    
                    ret.Add(new CacheDTO
                    {
                        key = key,
                        hits = hr.hits,
                        last = hr.dt_last.ToString()
                    });                        
                    
                    totalHits += hr.hits;
                }
            }

            var retOrdered = ret.OrderByDescending(y => y.hits);

            return Ok(new
            {
                count = totalHits,
                results = retOrdered
            });
        }
    }
}
