using Entities.Api.Admin;
using Master;
using Master.Repository;
using Master.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;

namespace Api.Master.Controllers
{
    public partial class CtrlBrand : MasterController
    {
        public CtrlBrand(IOptions<LocalNetwork> _network, IMemoryCache _cache) : base(_network, _cache) { }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/brandListing_v1")]
        public ActionResult brandListing([FromBody] DtoBrandListing obj)
        {
            var repo = new DapperAdminRepository();
            var srv = new SrvBrandListingV1(repo, cache);
            var dto = new DtoBrandListingResult();

            if (!srv.Exec(network, obj, ref dto))
                return BadRequest(srv.Error);

            return Ok(dto);
        }

        [HttpGet]
        [Route("api/brand_v1")]
        public ActionResult<DtoBrand> brand([FromQuery] string id)
        {
            var repo = new DapperAdminRepository();
            var srv = new SrvBrandGetV1(repo, cache);
            var dto = new DtoBrand();

            if (!srv.Exec(network, Convert.ToInt32(id), ref dto))
                return BadRequest(srv.Error);

            return Ok(dto);            
        }
    }
}
