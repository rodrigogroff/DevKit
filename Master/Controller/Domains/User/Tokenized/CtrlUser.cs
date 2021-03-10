using Master;
using Master.Infra.Entity.Database;
using Master.Repository;
using Master.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Api.Master.Controllers
{
    /*
    public partial class CtrlUserListing : MasterController
    {        
        public CtrlUserListing(IOptions<LocalNetwork> _network, IMemoryCache _cache) : base(_network, _cache) { }

        [HttpGet]
        [Route("api/user")]
        public ActionResult<User> user(long id)
        {
            var au = GetCurrentAuthenticatedUser();

            var repo = new DapperRepository();
            var srv = new SrvUserGetV1(repo);
            var dto = new User();

            if (!srv.Exec(network, au, id, ref dto))
                return BadRequest(srv.Error);

            return Ok(dto);
        }        
    }
    */
}
