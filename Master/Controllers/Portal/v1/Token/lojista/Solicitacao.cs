using Entities.Api.Associado;
using Entities.Api.Login;
using Master.Repository;
using Master.Service;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Api.Master.Controllers
{
    public partial class MasterController
    {
        [HttpGet]
        [Route("api/v1/portal/lojistaSolicitacao")]
        public ActionResult<AssociadoSolicitacao> lojistaSolicitacao()
        {
            var auth = GetCurrentAuthenticatedUser();

            var repo = new DapperRepository();
            var srv = new LojistaSolicitacaoV1(repo);
            var dto = new AssociadoSolicitacao();

            if (!srv.Exec(network, auth, ref dto))
                return BadRequest(srv.Error);

            return Ok(dto);
        }

        [HttpPost]
        [Route("api/v1/portal/solicitaVenda")]
        public ActionResult SolicitaVenda([FromBody] ReqSolicitacaoVenda obj)
        {
            var repo = new DapperRepository();
            var srv = new SolicitaVendaV1(repo);

            var au = GetCurrentAuthenticatedUser();

            if (!srv.Exec(network, Convert.ToInt64(au._id), obj))
                return BadRequest(srv.Error);

            return Ok(new
            {
                            
            });
        }
    }
}