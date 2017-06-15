using System.Collections.Generic;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
    public class CacheDTO
    {
        public string key;
    }
    
    public class CacheController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            var lst = GetCacheTags();

            var ret = new List<CacheDTO>();

            foreach (var key in lst)
            {
                ret.Add(new CacheDTO()
                {
                    key = key
                });
            }

            return Ok(new
            {
                count = ret.Count,
                results = ret
            });
        }
    }
}
