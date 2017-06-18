using System.Collections.Generic;
using System.Web.Http;
using System.Linq;
using System;

namespace DevKit.Web.Controllers
{
    public class CacheDTO
    {
        public string key;
        public int hits;
        public DateTime last;
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

            foreach (var key in lstTags)
            {
                if (hsh[key] is CacheHitRecord hr)
                {
                    if (hr.hits > 1)
                    {
                        ret.Add(new CacheDTO
                        {
                            key = key,
                            hits = hr.hits,
                            last = hr.dt_last
                        });
                    }
                }
            }

            var retOrdered = ret.OrderByDescending(y => y.hits);

            return Ok(new
            {
                count = lstTags.Count(),
                results = retOrdered
            });
        }
    }
}
