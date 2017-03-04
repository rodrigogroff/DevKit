using System.Web.Http;

namespace DevKit.Web.Controllers
{
	[Authorize]
	public abstract class ApiControllerBase : ApiController
	{
	}
}