using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class Task
	{
		public List<TaskCustomStep> LoadCustomSteps(DevKitDB db)
		{
            var setup = db.GetSetup();

            var ret = (from e in db.TaskCustomStep
					   where e.fkTask == id
					   select e).
					   OrderByDescending(t => t.id).
					   ToList();

			foreach (var item in ret)
            {
                if (item.bSelected == true)
                {
                    item.sdtLog = item.dtLog?.ToString(setup.stDateFormat);
                    item.sfkUser = db.GetUser(item.fkUser).stLogin;
                }                
            }
			
			return ret;
		}
	}
}
