using System.Linq;
using System.Collections.Generic;
using System;

namespace DataModel
{
	public class TimesheetFilter
	{
		public long? nuYear, nuMonth;
	}

	public class TimesheetDTO
	{
		public bool fail = false;

		public List<TimeTable> days = new List<TimeTable>();
	}

	public class TimeTable
	{
		public string	day,
						hhmm;

		public List<long> tasks = new List<long>();
	}
		
	public class Timesheet
	{
		public TimesheetDTO ComposedFilters(DevKitDB db, TimesheetFilter filter, User user)
		{
			var dto = new TimesheetDTO();

			var dtIni = new DateTime((int)filter.nuYear, (int)filter.nuMonth, 1);
			var dtFim = dtIni.AddMonths(1).AddSeconds(-1);

			var lstHours = (from e in db.TaskAccumulatorValues
							where e.fkUser == user.id
							where e.dtLog > dtIni && e.dtLog < dtFim
							select e).
							ToList();

			var idsTasks = lstHours.Select(y => y.fkTask).Distinct().ToList();

			var lstTasks = (from e in db.Tasks
						   where idsTasks.Contains(e.id)
						   select e).
						   ToList();

			for (int i = dtIni.Day; i < dtFim.Day; i++)
			{
				var dtCurrent = new DateTime((int)filter.nuYear, (int)filter.nuMonth, i);
				var dtCurrentFim = dtCurrent.AddDays(1).AddSeconds(-1);

				var hh = (from e in lstHours
						  where e.dtLog > dtCurrent && e.dtLog < dtCurrentFim
						  select e).
						  Sum(y => y.nuHourValue);

				var mm = (from e in lstHours
						  where e.dtLog > dtCurrent && e.dtLog < dtCurrentFim
						  select e).
						  Sum(y => y.nuMinValue);

				if (hh == null) hh = 0;
				if (mm == null) mm = 0;

				if (mm > 59)
				{
					var hours = (long) mm / 60;
					hh += hours;
					mm -= hours * 60;
				}

				var _hours = hh + ":" + mm.ToString().PadLeft(2, '0');

				dto.days.Add(new TimeTable
				{
					day = dtCurrent.Day.ToString(),
					hhmm = _hours,
					tasks = (from e in lstHours
							 where e.dtLog > dtCurrent && e.dtLog < dtCurrentFim
							 select (long)e.fkTask).
							 Distinct().
							 ToList()
				});
			}
			
			return dto;
		}
	}
}
