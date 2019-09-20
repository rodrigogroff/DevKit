using Entities.Api.Associado;
using Master.Repository;
using Master.Service;
using Microsoft.AspNetCore.Mvc;

namespace Api.Master.Controllers
{
    public partial class MasterController
    {
        [HttpGet("api/v1/portal/associadoLimites")]
        public ActionResult<AssociadoLimites> AssociadoLimites()
        {
            var auth = GetCurrentAuthenticatedUser();

            var repo = new DapperRepository();
            var srv = new AssociadoLimitesV1(repo);
            var dto = new AssociadoLimites();

            if (!srv.Exec(network, auth, ref dto))
                return BadRequest(srv.Error);

            return Ok(dto);
        }
    }
}
