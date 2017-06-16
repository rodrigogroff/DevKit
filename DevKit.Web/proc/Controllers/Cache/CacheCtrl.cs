using System.Collections.Generic;
using System.Web.Http;
using System.Linq;

namespace DevKit.Web.Controllers
{
    public class CacheDTO
    {
        public string key, hits, last;
    }
    
    public class CacheController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();
            
            var hsh = GetCacheHitRecord();
            var ret = new List<CacheDTO>();

            foreach (var key in GetCacheTags())
            {
                if (hsh[key] is CacheHitRecord hr)
                {
                    if (hr.hits > 1)
                    {
                        ret.Add(new CacheDTO()
                        {
                            key = key,
                            hits = hr.hits.ToString(),
                            last = hr.dt_last.ToString()
                        });
                    }
                }
            }

            var retOrdered = ret.OrderByDescending(y => y.hits);

            return Ok(new
            {
                count = myApplication.AllKeys.Count(),
                results = retOrdered
            });
        }
    }
}
