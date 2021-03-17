using Master.Data.Domains.User;
using Master.Infra;
using Master.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Master.Controllers
{
    public partial class CtrlCheckToken : MasterController
    {
        public CtrlCheckToken(IOptions<LocalNetwork> _network) : base(_network) { }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/CheckToken_v1")]
        public ActionResult Post([FromBody] DtoCheckToken obj)
        {
            //var repo = new DapperRepository();
            var srv = new SrvCheckTokenV1();

            if (!srv.Exec(network, obj))
                return BadRequest(srv.Error);

            return Ok(new { });
        }
    }
}
