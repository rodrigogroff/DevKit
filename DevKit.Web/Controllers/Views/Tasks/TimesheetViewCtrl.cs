using DataModel;
using Newtonsoft.Json;
using System.Net;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class TimesheetViewController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            AuthorizeAndStartDatabase();

			var mdl = new TimesheetView();

			return Ok(mdl.ComposedFilters(db, new TimesheetViewFilter()
            {
                nuYear = Request.GetQueryStringValue<long?>("nuYear", null),
                nuMonth = Request.GetQueryStringValue<long?>("nuMonth", null),
                fkUser = Request.GetQueryStringValue<long?>("fkUser", null),
            }));			
		}
	}
}
