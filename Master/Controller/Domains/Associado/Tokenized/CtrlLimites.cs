using Master.Data.Domains.Associado;
using Master.Infra;
using Master.Repository;
using Master.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Master.Controllers
{
    public partial class CtrlLimites : MasterController
    {
        public CtrlLimites(IOptions<LocalNetwork> _network) : base(_network) { }

        [HttpGet]        
        [Route("api/v1/portal/associadoLimites")]
        public ActionResult<DtoAssociadoLimites> associadoLimites(long id)
        {
            var au = GetCurrentAuthenticatedUser();

            var srv = new SrvAssociadoLimitesV1()
            {
                cartaoRepository = new CartaoDapperRepository(),
                empresaRepository = new EmpresaDapperRepository(),
                parcelaDapperRepository = new ParcelaDapperRepository(),
                logTransacaoDapperRepository = new LogTransacaoDapperRepository(),
            };

            var dto = new DtoAssociadoLimites();

            if (!srv.Exec(network, au, ref dto))
                return BadRequest(srv.Error);

            return Ok(dto);
        }        
    }
}
