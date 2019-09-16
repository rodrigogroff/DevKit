using Entities.Api.Configuration;
using Entities.Api.Login;
using Entities.Database;
using Master.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Master.Controllers
{
    public partial class MasterController
    {
        [AllowAnonymous]
        [HttpPost("api/v1/portal/authenticate")]
        public ActionResult Authenticate([FromBody] LoginInformation obj)
        {
            //obj.browserIP = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            
            var auth = new AuthenticatedUser();
            var userInfo = new User();

            var srv = new UserAuthenticateV1();

            if (!srv.Exec(network, obj, ref userInfo, ref auth))
                return BadRequest(srv.Error);
                        
            var token = ComposeTokenForSession(auth);
            
            return Ok( new 
            {
                token,
                user = userInfo 
            });
        }
    }
}
