using Entities.Api.Associado;
using Entities.Api.Lojista;
using Master.Repository;
using Master.Service;
using Microsoft.AspNetCore.Mvc;

namespace Api.Master.Controllers
{
    public partial class MasterController
    {
        [HttpGet]
        [Route("api/v1/portal/lojistaCancelamentos")]
        public ActionResult<LojistaCancelamentosDTO> lojistaCancelamentos()
        {
            var auth = GetCurrentAuthenticatedUser();

            var repo = new DapperRepository();
            var srv = new LojistaCancelamentosV1(repo);
            var dto = new LojistaCancelamentosDTO();

            if (!srv.Exec(network, auth, ref dto))
                return BadRequest(srv.Error);

            return Ok(dto);
        }

        [HttpPost]
        [Route("api/v1/portal/solicitaVendaCancelamento")]
        public ActionResult SolicitaVendaCancelamento([FromBody] ReqSolicitacaoVendaCancelamento obj)
        {
            var repo = new DapperRepository();
            var srv = new SolicitaVendaPosCancelamentoV1(repo);

            var au = GetCurrentAuthenticatedUser();

            if (!srv.Exec(network, au, obj))
                return BadRequest(srv.Error);

            return Ok(new { });
        }
    }
}