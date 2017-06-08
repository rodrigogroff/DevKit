using DataModel;
using Newtonsoft.Json;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class HomeViewController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            var login = GetLoginInfo();

            if (login == null)
                return BadRequest();

            using (var db = new DevKitDB())
			{
                if (!db.ValidateUser(login.idUser))
                    return BadRequest();

                var mdl = new HomeView();
				
				return Ok(mdl.ComposedFilters(db, login.idUser));
			}
		}
	}
}
