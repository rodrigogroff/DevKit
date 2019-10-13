using Entities.Api.Configuration;
using Entities.Api.Login;
using Master.Repository;
using Master.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Master.Controllers
{
    public partial class MasterController
    {
        [AllowAnonymous]
        [HttpPost]
        [Route("api/v1/portal/authenticate")]
        public ActionResult AuthenticateAssociado([FromBody] AssociadoLoginInformation obj)
        {
            //obj.browserIP = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            
            var auth = new AuthenticatedUser();
            var repo = new DapperRepository();
            
            var srv = new AssociadoAuthenticateV1(repo);

            if (!srv.Exec(network, obj, ref auth))
                return BadRequest(srv.Error);
                        
            var token = ComposeTokenForSession(auth);
            
            return Ok( new 
            {
                token,
                user = auth
            });
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/v1/portal/authenticateLojista")]
        public ActionResult AuthenticateLojista([FromBody] LojistaLoginInformation obj)
        {
            //obj.browserIP = Request.HttpContext.Connection.RemoteIpAddress.ToString();

            var auth = new AuthenticatedUser();
            var repo = new DapperRepository();

            var srv = new LojistaAuthenticateV1(repo);

            if (!srv.Exec(network, obj, ref auth))
                return BadRequest(srv.Error);

            var token = ComposeTokenForSession(auth);

            return Ok(new
            {
                token,
                user = auth
            });
        }
    }
}
