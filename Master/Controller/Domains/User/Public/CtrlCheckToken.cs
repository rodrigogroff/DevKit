using Entities.Api.User;
using Master;
using Master.Repository;
using Master.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Api.Master.Controllers
{
    public partial class CtrlCheckToken : MasterController
    {
        public CtrlCheckToken(IOptions<LocalNetwork> _network, IMemoryCache _cache) : base(_network, _cache) { }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/CheckToken_v1")]
        public ActionResult Post([FromBody] DtoCheckToken obj)
        {
            var repo = new DapperUserRepository();
            var srv = new SrvCheckTokenV1(repo);

            if (!srv.Exec(network, obj))
                return BadRequest(srv.Error);

            return Ok(new { });
        }
    }
}
