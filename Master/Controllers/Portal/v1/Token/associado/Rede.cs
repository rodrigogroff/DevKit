using Entities.Api.Associado;
using Master.Repository;
using Master.Service;
using Microsoft.AspNetCore.Mvc;

namespace Api.Master.Controllers
{
    public partial class MasterController
    {
        [HttpGet]
        [Route("api/v1/portal/associadoRede")]
        public ActionResult<AssociadoRede> associadoRede()
        {
            var auth = GetCurrentAuthenticatedUser();

            var repo = new DapperRepository();
            var srv = new AssociadoRedeV1(repo);
            var dto = new AssociadoRede();

            if (!srv.Exec(network, auth, ref dto))
                return BadRequest(srv.Error);

            return Ok(dto);
        }
    }
}
