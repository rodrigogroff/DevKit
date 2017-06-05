using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using DataModel;

namespace DevKit.Core.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values

        public IActionResult Get()
        {
            using (var db = new DevKitDB())
            {
                var count = 0; var mdl = new Project();

                var results = mdl.ComposedFilters(db, ref count, new ProjectFilter
                {
                    //skip = Request.QueryString["skip", 0),
                    //take = Request.GetQueryStringValue("take", 15),
                    //busca = Request.GetQueryStringValue("busca")?.ToUpper(),
                    //fkUser = Request.GetQueryStringValue<long?>("fkUser", null),
                });

                return Ok(new { count = count, results = results });
            }
        }

        /*
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }

*/


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
