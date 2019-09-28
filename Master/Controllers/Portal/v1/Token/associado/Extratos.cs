using Entities.Api.Associado;
using Master.Repository;
using Master.Service;
using Microsoft.AspNetCore.Mvc;

namespace Api.Master.Controllers
{
    public partial class MasterController
    {
        [HttpGet]
        [Route("api/v1/portal/associadoExtratoAtual")]
        public ActionResult<AssociadoExtratoAtual> AssociadoExtratoAtual()
        {
            var auth = GetCurrentAuthenticatedUser();

            var repo = new DapperRepository();
            var srv = new AssociadoExtratoAtualV1(repo);
            var dto = new AssociadoExtratoAtual();

            if (!srv.Exec(network, auth, ref dto))
                return BadRequest(srv.Error);

            return Ok(dto);
        }

        [HttpGet]
        [Route("api/v1/portal/associadoExtratoFuturo")]
        public ActionResult<AssociadoExtratoFuturo> AssociadoExtratoFuturo()
        {
            var auth = GetCurrentAuthenticatedUser();

            var repo = new DapperRepository();
            var srv = new AssociadoExtratoFuturoV1(repo);
            var dto = new AssociadoExtratoFuturo();

            if (!srv.Exec(network, auth, ref dto))
                return BadRequest(srv.Error);

            return Ok(dto);
        }

        [HttpGet]
        [Route("api/v1/portal/associadoExtratoFechada")]
        public ActionResult<AssociadoExtratoFechada> AssociadoExtratoFechada()
        {
            var auth = GetCurrentAuthenticatedUser();

            var repo = new DapperRepository();
            var srv = new AssociadoExtratoFechadaV1(repo);
            var dto = new AssociadoExtratoFechada();

            if (!srv.Exec(network, auth, ref dto))
                return BadRequest(srv.Error);

            return Ok(dto);
        }
    }
}
