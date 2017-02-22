using System.Web.Http;

namespace App.Web.Controllers
{
	[Authorize]
	public abstract class ApiControllerBase : ApiController
	{
	}
}