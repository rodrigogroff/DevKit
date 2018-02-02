using DataModel;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class TimesheetViewController : ApiControllerBase
	{
		public IHttpActionResult Get()
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

			var mdl = new TimesheetView();

			return Ok(mdl.ComposedFilters(db, new TimesheetViewFilter
            {
                nuYear = Request.GetQueryStringValue<long?>("nuYear", null),
                nuMonth = Request.GetQueryStringValue<long?>("nuMonth", null),
                fkUser = Request.GetQueryStringValue<long?>("fkUser", null),
            }));			
		}
	}
}
