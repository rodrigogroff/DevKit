using System.Linq;
using System.Net;
using System.Web.Http;
using DataModel;

namespace DevKit.Web.Controllers
{
	public class AccumulatorTypeController : ApiControllerBase
	{
		EnumAccumulatorType AccType = new EnumAccumulatorType();		

		public IHttpActionResult Get()
		{
			{
				string busca = Request.GetQueryStringValue("busca")?.ToUpper();
				
				var query = (from e in AccType.lst select e);

				if (busca != null)
					query = from e in query where e.stName.ToUpper().Contains(busca) select e;
				
				query = query.OrderBy(y => y.stName);

				return Ok(new
				{
					count = query.Count(),
					results = query.ToList()
				});
			}
		}

		public IHttpActionResult Get(long id)
		{
			var model = (from ne in AccType.lst select ne).
				Where(t => t.id == id).
				FirstOrDefault();

			if (model != null)
				return Ok(model);

			return StatusCode(HttpStatusCode.NotFound);			
		}
	}
}
