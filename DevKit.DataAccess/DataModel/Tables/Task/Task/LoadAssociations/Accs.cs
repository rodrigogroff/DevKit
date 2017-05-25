using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class Task
	{
		public List<TaskTypeAccumulator> LoadAccs(DevKitDB db)
		{
			var setup = db.GetSetup();
			var lstAccTypes = new EnumAccumulatorType().lst;

			var lstAccs = (from e in db.TaskTypeAccumulator
					   where e.fkTaskCategory == this.fkTaskCategory
					   select e).
					   ToList();

			foreach (var item in lstAccs)
			{
				item.sfkTaskAccType = lstAccTypes.
					Where(y => y.id == item.fkTaskAccType).
						FirstOrDefault().
							stName;

				item.snuTotal = GetValueForType(db, item.sfkTaskAccType, id, item.id);

				var logs = (from e in db.TaskAccumulatorValue
							where e.fkTask == id
							where e.fkTaskAcc == item.id
							select e).
							OrderByDescending(y => y.id).
							ToList();

				item.logs = new List<LogAccumulatorValue>();

				foreach (var log in logs)
				{
					item.logs.Add(new LogAccumulatorValue()
					{
						id = log.id,
						sfkUser = db.GetUser(log.fkUser).stLogin,
						sdtLog = log.dtLog?.ToString(setup.stDateFormat),
						sValue = GetValueForType(db, item.sfkTaskAccType, id, item.id, log.id)
					});
				}
			}

			return lstAccs;
		}
	}
}
