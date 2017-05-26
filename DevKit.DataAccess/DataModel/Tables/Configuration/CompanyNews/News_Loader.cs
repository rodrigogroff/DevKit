using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class CompanyNews
    {
		public List<CompanyNews> Loader(DevKitDB db, List<CompanyNews> results, bool precached)
        {
            if (precached)
            {
                var lstIdsUser = results.Select(y => y.fkUser).Distinct().ToList();
                
                if (lstIdsUser.Any())
                {
                    var lst = (from e in db.User where lstIdsUser.Contains(e.id) select e).ToList();
                    foreach (var item in lst) db.Cache["User" + item.id] = item;
                }

                var lstIdsProject = results.Select(y => y.fkProject).Distinct().ToList();

                if (lstIdsProject.Any())
                {
                    var lst = (from e in db.Project where lstIdsProject.Contains(e.id) select e).ToList();
                    foreach (var item in lst) db.Cache["Project" + item.id] = item;
                }
            }
            
            results.ForEach(y => { y = y.LoadAssociations(db); });

            return results;
        }
    }
}
