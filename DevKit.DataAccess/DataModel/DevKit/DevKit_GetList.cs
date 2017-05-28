using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class DevKitDB
	{				
        public List<TaskCategory> GetListTaskCategory(long? fkTaskType)
        {
            if (fkTaskType == null) return null;            

            var lst = Cache["ListTaskCategory"] as List<TaskCategory>;

            if (lst == null)
            {
                return (from e in TaskCategory
                        where e.fkTaskType == fkTaskType
                        select e).
                        ToList();
            }
            
            return (from e in lst
                    where e.fkTaskType == fkTaskType
                    select e).
                    ToList();            
        }
    }
}
