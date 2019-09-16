using Entities.Database;
using Master.Service;
using Microsoft.AspNetCore.Mvc;
//using Portal.Ensemble.Services;

namespace Api.Master.Controllers
{
    public partial class MasterController
    {
        [HttpGet("api/v1/portal/userDetail")]
        public ActionResult<User> UserDetail(string cpf)
        {
            var au = GetCurrentAuthenticatedUser();
            var ret = new User();
                        
            var srv = new UserDetailV1();
            
            if (!srv.Exec(network, au, cpf, ref ret))
                return BadRequest(srv.Error);

            return Ok(ret);
        }
    }
}
