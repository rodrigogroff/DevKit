using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class Task
	{
		public string GetValueForType(DevKitDB db, string _type, long task_id, long task_acc, long accVal_id = 0, List<long> lstRange = null )
		{
			var ret = "";

			switch (_type)
			{
				case "Money":

					#region - code - 

					if (accVal_id == 0)
						ret = ( from e in db.TaskAccumulatorValues
								where accVal_id == 0 || e.id == accVal_id 
								where task_id ==0 || e.fkTask == task_id
								where lstRange == null || lstRange.Contains ( (long)e.fkTask)
								where e.fkTaskAcc == task_acc
								select e).Select(y => y.nuValue).ToString();
					else
						ret = ( from e in db.TaskAccumulatorValues
								where task_id == 0 || e.fkTask == task_id
								where e.fkTaskAcc == task_acc
								select e).Sum(y => y.nuValue).ToString();

					#endregion

					break;

				case "Hours":

					#region - code - 

					long? hh = 0, mm = 0;

					if (accVal_id == 0)
					{
						hh = (from e in db.TaskAccumulatorValues
							  where task_id == 0 || e.fkTask == task_id
							  where lstRange == null || lstRange.Contains((long)e.fkTask)
							  where e.fkTaskAcc == task_acc
							  select e).Sum(y => y.nuHourValue) * 60;

						if (hh == null) hh = 0;
					}						
					else
					{
						hh = (from e in db.TaskAccumulatorValues
							  where e.id == accVal_id
							  where task_id == 0 || e.fkTask == task_id
							  where e.fkTaskAcc == task_acc
							  select e).FirstOrDefault().nuHourValue;

						if (hh != null)
							hh = hh * 60;
						else
							hh = 0;
					}

					if (accVal_id == 0)
					{
						mm = (from e in db.TaskAccumulatorValues
							  where task_id == 0 || e.fkTask == task_id
							  where lstRange == null || lstRange.Contains((long)e.fkTask)
							  where e.fkTaskAcc == task_acc
							  select e).Sum(y => y.nuMinValue);

						if (mm == null)
							mm = 0;
					}
					else
					{
						mm = (from e in db.TaskAccumulatorValues
							  where e.id == accVal_id
							  where task_id == 0 || e.fkTask == task_id
							  where e.fkTaskAcc == task_acc
							  select e).FirstOrDefault().nuMinValue;

						if (mm == null)
							mm = 0;
					}

					var tot = hh + mm;

					if (tot != 0)
					{
						var totHours = (hh + mm) / 60;
						var totMins = tot - totHours * 60;

						ret = totHours.ToString() + ":" + totMins.ToString().PadLeft(2, '0');
					}
					else
						ret = "00:00";

					#endregion

					break;
			}

			return ret;
		}		
	}
}
