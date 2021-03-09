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
    public partial class CtrlPasswordRecovery : MasterController
    {
        public CtrlPasswordRecovery(IOptions<LocalNetwork> _network, IMemoryCache _cache) : base(_network, _cache) { }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/passrecovery_v1")]
        public ActionResult Post([FromBody] DtoPasswordRecovery obj)
        {
            var repo = new DapperUserRepository();
            var srv = new SrvPasswordRecoveryV1(repo) {
                _sendEmail = this._sendEmail
            };

            if (!srv.Exec(network, obj))
                return BadRequest(srv.Error);

            return Ok(new { });
        }
    }
}
