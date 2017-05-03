using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class Task
	{
		public List<TaskTypeAccumulator> LoadAccs(DevKitDB db)
		{
			var setup = db.Setup();
			var stypes = new EnumAccumulatorType().lst;

			var ret = (from e in db.TaskTypeAccumulators
					   where e.fkTaskCategory == this.fkTaskCategory
					   select e).
					   ToList();

			foreach (var item in ret)
			{
				item.sfkTaskAccType = stypes.
					Where(y => y.id == item.fkTaskAccType).
						FirstOrDefault().
							stName;

				item.snuTotal = GetValueForType(db, item.sfkTaskAccType, id, item.id);

				var logs = (from e in db.TaskAccumulatorValues
							where e.fkTask == id
							where e.fkTaskAcc == item.id
							select e).
							OrderByDescending(y => y.id).
							ToList();

				item.logs = new List<LogAccumulatorValue>();

				foreach (var l in logs)
				{
					item.logs.Add(new LogAccumulatorValue()
					{
						id = l.id,
						sfkUser = db.User(l.fkUser).stLogin,
						sdtLog = l.dtLog?.ToString(setup.stDateFormat),
						sValue = GetValueForType(db, item.sfkTaskAccType, id, item.id, l.id)
					});
				}
			}

			return ret;
		}
	}
}
