using Entities.Database;
using Master.Repository;
using Master.Service;
using Microsoft.AspNetCore.Mvc;

namespace Api.Master.Controllers
{
    public partial class MasterController
    {
        [HttpGet("api/v1/portal/userDetail")]
        public ActionResult<User> UserDetail(string cpf)
        {
            var au = GetCurrentAuthenticatedUser();
            var repo = new DapperRepository();
            var ret = new User();
            var srv = new UserDetailV1(repo);
            
            if (!srv.Exec(network, au, cpf, ref ret))
                return BadRequest(srv.Error);

            return Ok(ret);
        }
    }
}
