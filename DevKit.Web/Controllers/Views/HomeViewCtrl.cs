using DataModel;
using Newtonsoft.Json;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class HomeViewController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            AuthorizeAndStartDatabase();

            var mdl = new HomeView();
				
			return Ok(mdl.ComposedFilters(db, login.idUser));			
		}
	}
}
