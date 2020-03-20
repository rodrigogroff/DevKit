using Entities.Api.Associado;
using Entities.Api.Lojista;
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
        [Route("api/v1/portal/solicitaVendaCartao")]
        public ActionResult SolicitaVendaCartao([FromBody] ReqSolicitacaoVendaCartao obj)
        {
            var repo = new DapperRepository();
            var srv = new SolicitaVendaCartaoV1(repo);

            var nomeCartao = "";

            if (!srv.Exec(network, obj, ref nomeCartao))
                return BadRequest(srv.Error);

            return Ok(new { nomeCartao });
        }

        [HttpPost]
        [Route("api/v1/portal/solicitaVendaPos")]
        public ActionResult SolicitaVendaPos([FromBody] ReqSolicitacaoVendaPOS obj)
        {
            var repo = new DapperRepository();
            var srv = new SolicitaVendaPosV1(repo);

            var au = GetCurrentAuthenticatedUser();

            if (!srv.Exec(network, Convert.ToInt64(au._id), obj))
                return BadRequest(srv.Error);

            return Ok(new { });
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

            return Ok(new { });
        }

        [HttpPost]
        [Route("api/v1/portal/solicitaVendaCancelamento")]
        public ActionResult SolicitaVendaCancelamento([FromBody] ReqSolicitacaoVenda obj)
        {
            var repo = new DapperRepository();
            var srv = new SolicitaVendaCancelamentoV1(repo);

            var au = GetCurrentAuthenticatedUser();

            if (!srv.Exec(network, Convert.ToInt64(au._id), obj))
                return BadRequest(srv.Error);

            return Ok(new { });
        }

        [HttpGet]
        [Route("api/v1/portal/lojistaAutorizacoes")]
        public ActionResult<LojistaAutorizacoesDTO> lojistaAutorizacoes()
        {
            var auth = GetCurrentAuthenticatedUser();

            var repo = new DapperRepository();
            var srv = new LojistaAutorizacoesV1(repo);
            var dto = new LojistaAutorizacoesDTO();

            if (!srv.Exec(network, auth, ref dto))
                return BadRequest(srv.Error);

            return Ok(dto);
        }
    }
}