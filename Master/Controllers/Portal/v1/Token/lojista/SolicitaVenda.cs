using Entities.Api.Login;
using Master.Repository;
using Master.Service;
using Microsoft.AspNetCore.Mvc;

namespace Api.Master.Controllers
{
    public partial class MasterController
    {
        [HttpPost]
        [Route("api/v1/portal/solicitaVenda")]
        public ActionResult SolicitaVenda([FromBody] SolicitacaoVenda obj)
        {
            var repo = new DapperRepository();

            var srv = new SolicitaVendaV1(repo);

            if (!srv.Exec(network, obj))
                return BadRequest(srv.Error);

            return Ok(new
            {
            
            });
        }
    }
}