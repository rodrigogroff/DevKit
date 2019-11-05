using Entities.Api.Associado;
using Master.Repository;
using Master.Service;
using Microsoft.AspNetCore.Mvc;

namespace Api.Master.Controllers
{
    public partial class MasterController
    {
        [HttpGet]
        [Route("api/v1/portal/associadoSolicitacao")]
        public ActionResult<AssociadoSolicitacao> associadoSolicitacao()
        {
            var auth = GetCurrentAuthenticatedUser();

            var repo = new DapperRepository();
            var srv = new AssociadoSolicitacaoV1(repo);
            var dto = new AssociadoSolicitacao();

            if (!srv.Exec(network, auth, ref dto))
                return BadRequest(srv.Error);

            return Ok(dto);
        }

        [HttpPost]
        [Route("api/v1/portal/confirmaSolicitacao")]
        public ActionResult ConfirmaSolicitacao([FromBody] AssociadoSolicitacao obj)        
        {
            var auth = GetCurrentAuthenticatedUser();

            var repo = new DapperRepository();
            var srv = new AssociadoConfSolicitacaoV1(repo);
            
            if (!srv.Exec(network, auth, obj))
                return BadRequest(srv.Error);

            return Ok(new { });
        }
    }
}
