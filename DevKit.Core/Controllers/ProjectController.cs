using DataModel;
using Microsoft.AspNetCore.Mvc;

namespace DevKit.Core.Controllers
{
    [Route("api/[controller]")]
    public class ProjectController : ApiBaseController
    {
        public IActionResult Get()
        {
            var login = GetLoginInfo();

            using (var db = new DevKitDB())
            {
                var count = 0; var mdl = new Project();

                

                var results = mdl.ComposedFilters(db, ref count, new ProjectFilter
                {
                    skip = Request.GetQueryStringValue("skip", 0),
                    take = Request.GetQueryStringValue("take", 15),
                    busca = Request.GetQueryStringValue("busca")?.ToUpper(),
                    fkUser = Request.GetQueryStringValue<long?>("fkUser", null),
                });

                return Ok(new { count = count, results = results });
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
