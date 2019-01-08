using System;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class DataServerController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            return Ok(new { dt = DateTime.Now.ToString("dd/MM/yyyy") });
		}
	}
}
