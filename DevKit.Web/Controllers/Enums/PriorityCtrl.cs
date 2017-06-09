using System.Linq;
using System.Net;
using System.Web.Http;
using DataModel;

namespace DevKit.Web.Controllers
{
	public class PriorityController : ApiControllerBase
	{
		EnumPriority _enum = new EnumPriority();		

		public IHttpActionResult Get()
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();

            string busca = Request.GetQueryStringValue("busca")?.ToUpper();
				
			var query = (from e in _enum.lst select e);

			if (busca != null)
				query = from e in query where e.stName.ToUpper().Contains(busca) select e;
				
			return Ok(new
			{
				count = query.Count(),
				results = query.ToList()
			});
		}

		public IHttpActionResult Get(long id)
		{
            if (!AuthorizeAndStartDatabase())
                return BadRequest();
            
            var model = _enum.Get(id);

			if (model != null)
				return Ok(model);

			return StatusCode(HttpStatusCode.NotFound);			
		}
	}
}
