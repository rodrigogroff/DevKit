using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Cache
{
    [AllowAnonymous]
    [ApiController]
    public partial class MasterController : ControllerBase
    {
        private IMemoryCache _memoryCache;
        
        public MasterController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        
        [HttpGet]
        [Route("api/getCache")]
        public ActionResult getCache(string _tag)
        {            
            string data; 
            if (!_memoryCache.TryGetValue(_tag, out data))
                return BadRequest();
            return Ok(data);
        }
        
        [HttpGet]
        [Route("api/updateCache")]
        public ActionResult updateCache(string _tag, string _value)
        {
            _memoryCache.Set(_tag, _value);
            return Ok();
        }
    }
}
