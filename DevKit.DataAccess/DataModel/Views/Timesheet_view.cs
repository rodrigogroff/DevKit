using System;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public class TimesheetViewFilter
	{
		public long? nuYear, 
						nuMonth, 
						fkUser;
	}
	
	public class TimeTableUser
	{
		public string user,
						hhmm;
	}

	public class TimesheetDTO
	{
		public bool fail = false,
					multiUser = false;

		public List<TimeTable> days = new List<TimeTable>();
		public List<TimeTableUser> users = new List<TimeTableUser>();

		public string total_hhmm;
	}

	public class TimeTable
	{
		public string day,
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

	public class TimesheetView
	{
		public TimesheetDTO ComposedFilters(DevKitDB db, TimesheetViewFilter filter)
		{
			var dto = new TimesheetDTO();

			var dtIni = new DateTime((int)filter.nuYear, (int)filter.nuMonth, 1);
			var dtFim = dtIni.AddMonths(1).AddSeconds(-1);

			if (filter.fkUser == null)
			{
				dto.multiUser = true;

				var lstUsers = (from e in db.User
								where e.bActive == true
								select e).
								ToList();

				var lstUserIds = (from e in lstUsers select e.id).ToList();

				var lstHours = (from e in db.TaskAccumulatorValue
								join eA in db.TaskTypeAccumulator on e.fkTaskAcc equals eA.id
								where eA.bEstimate != true
								where lstUserIds.Contains((long)e.fkUser)
								where e.dtLog > dtIni && e.dtLog < dtFim
								select e).
								ToList();

				long totHH = 0, totMM = 0;

				foreach (var usr in lstUsers)
				{
					var hh = (from e in lstHours
							  where e.fkUser == usr.id
							  select e).
							  Sum(y => y.nuHourValue);

					var mm = (from e in lstHours
							  where e.fkUser == usr.id
							  select e).
							  Sum(y => y.nuMinValue);

					if (hh == null) hh = 0;
					if (mm == null) mm = 0;

					var hoursU = (long)mm / 60;
					hh += hoursU;

					totHH += (long)hh;

					if (mm > 59)
					{						
						mm -= hoursU * 60;						
						totMM += (long)mm;
					}

					var _hours = hh + ":" + mm.ToString().PadLeft(2, '0');

					dto.users.Add(new TimeTableUser
					{
						user = usr.stLogin,
						hhmm = _hours
					});
				}

				var hours = (long)totMM / 60;
				totHH += hours;

				if (totMM > 59)
					totMM -= hours * 60;

				dto.total_hhmm = totHH + ":" + totMM.ToString().PadLeft(2, '0');
			}
			else
			{
				var lstHours = (from e in db.TaskAccumulatorValue
								join eA in db.TaskTypeAccumulator on e.fkTaskAcc equals eA.id
								where eA.bEstimate != true
								where e.fkUser == filter.fkUser
								where e.dtLog > dtIni && e.dtLog < dtFim
								select e).
								ToList();

				var idsTasks = lstHours.Select(y => y.fkTask).Distinct().ToList();

				var lstTasks = (from e in db.Task
								where idsTasks.Contains(e.id)
								select e).
							   ToList();

				long totHH = 0, 
					totMM = 0;

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

						var hours = (long)mm / 60;
						hh += hours;

						totHH += (long)hh;

						if (mm > 59)
						{
							mm -= hours * 60;
							totMM += (long)mm;
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

				var hoursF = (long)totMM / 60;
				totHH += hoursF;

				if (totMM > 59)
					totMM -= hoursF * 60;

				dto.total_hhmm = totHH + ":" + totMM.ToString().PadLeft(2, '0');
			}

			return dto;
		}
	}
}
