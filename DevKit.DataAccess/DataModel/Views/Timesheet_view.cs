using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public class TimesheetFilter
	{
		public long? nuYear, nuMonth;
	}

	public class TimesheetDTO
	{
		public bool fail = false;

		public List<ManagementDTO_Version> versions = new List<ManagementDTO_Version>();
	}
	
	public class Timesheet
	{
		public TimesheetDTO ComposedFilters(DevKitDB db, TimesheetFilter filter, List<long?> lstUserProjects)
		{
			var dto = new TimesheetDTO();
			
			return dto;
		}
	}
}
