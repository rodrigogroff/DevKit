using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class Client
	{
		public List<Client> Loader(DevKitDB db, List<Client> results, bool precached)
        {
            if (precached)
            {
                var lstIdsUser = results.Select(y => y.fkUser).Distinct().ToList();
                
                if (lstIdsUser.Any())
                {
                    var lst = (from e in db.User where lstIdsUser.Contains(e.id) select e).ToList();
                    foreach (var item in lst) db.Cache["User" + item.id] = item;
                }

                var lstIdsEstado = results.Select(y => y.fkEstado).Distinct().ToList();

                if (lstIdsEstado.Any())
                {
                    var lst = (from e in db.Estado where lstIdsEstado.Contains(e.id) select e).ToList();
                    foreach (var item in lst) db.Cache["Estado" + item.id] = item;
                }

                var lstIdsCidade = results.Select(y => y.fkCidade).Distinct().ToList();

                if (lstIdsCidade.Any())
                {
                    var lst = (from e in db.Cidade where lstIdsCidade.Contains(e.id) select e).ToList();
                    foreach (var item in lst) db.Cache["Cidade" + item.id] = item;
                }
            }
            
            results.ForEach(y => { y = y.LoadAssociations(db); });

            return results;
        }
    }
}
