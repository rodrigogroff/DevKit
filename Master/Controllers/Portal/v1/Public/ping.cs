using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Master.Controllers
{
    public partial class MasterController
    {
        [AllowAnonymous]
        [HttpGet("api/v1/portal/ping")]
        public ActionResult Ping()
        {
            return Ok("pong");
        }
    }
}
