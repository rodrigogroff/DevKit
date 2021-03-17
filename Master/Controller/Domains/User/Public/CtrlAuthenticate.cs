using Master.Data.Domains.User;
using Master.Infra;
using Master.Repository;
using Master.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Master.Controllers
{
    public partial class CtrlAuthenticate : MasterController
    {
        public CtrlAuthenticate(IOptions<LocalNetwork> _network) : base(_network) { }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/authenticate_v1")]
        public ActionResult Post([FromBody] DtoLoginInformation obj)
        {
            var auth = new DtoAuthenticatedUser();
            
            var srv = new SrvAuthenticateV1
            {
                cartaoRepository = new CartaoDapperRepository(),
                empresaRepository = new EmpresaDapperRepository()
            };

            if (!srv.Exec(network, obj, ref auth))
                return BadRequest(srv.Error);

            var token = ComposeTokenForSession(auth);
            
            return Ok( new 
            {
                token,
                user = auth,                
            });
        }
    }
}
