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

		public string total_hhmm;
	}

	public class TimeTable
	{
		public string	day,
						hhmm;

		public List<TaskTimeSheet> tasks = new List<TaskTimeSheet>();
	}

	public class TaskTimeSheet
	{
		public string protocol,
						id,
						hhmm,
						nuPriority;
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

			long totHH = 0, totMM = 0;

			for (int i = dtIni.Day; i <= dtFim.Day; i++)
			{
				var dtCurrent = new DateTime((int)filter.nuYear, (int)filter.nuMonth, i);
				var dtCurrentFim = dtCurrent.AddDays(1).AddSeconds(-1);

				var _hours = "";

				{
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
						var hours = (long)mm / 60;
						hh += hours;
						mm -= hours * 60;

						totHH += (long) hh;
						totMM += (long) mm;
					}

					_hours = hh + ":" + mm.ToString().PadLeft(2, '0');
				}
				
				var lstTaskTimesheet = new List<TaskTimeSheet>();

				foreach (var item in (from e in lstHours
									  where e.dtLog > dtCurrent && e.dtLog < dtCurrentFim
									  select e).
									  Distinct().
									  ToList())
				{
					var task = lstTasks.Where(y => y.id == item.fkTask).
						FirstOrDefault();

					var hh = item.nuHourValue;
					var mm = item.nuMinValue;

					if (hh == null) hh = 0;
					if (mm == null) mm = 0;

					if (mm > 59)
					{
						var hours = (long)mm / 60;
						hh += hours;
						mm -= hours * 60;
					}

					lstTaskTimesheet.Add(new TaskTimeSheet
					{
						id = task.id.ToString(),
						protocol = task.stProtocol,
						hhmm = hh + ":" + mm.ToString().PadLeft(2, '0'),
						nuPriority = task.nuPriority.ToString()
					});
				}

				dto.days.Add(new TimeTable
				{
					day = dtCurrent.Day.ToString(),
					hhmm = _hours,
					tasks = lstTaskTimesheet
				});
			}

			if (totMM > 59)
			{
				var hours = (long)totMM / 60;
				totHH += hours;
				totMM -= hours * 60;
			}

			dto.total_hhmm = totHH + ":" + totMM.ToString().PadLeft(2,'0');

			return dto;
		}
	}
}
