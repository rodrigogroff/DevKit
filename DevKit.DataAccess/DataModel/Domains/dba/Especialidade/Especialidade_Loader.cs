using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class Especialidade
    {
		public List<Especialidade> Loader(DevKitDB db, List<Especialidade> results)
        {
            results.ForEach(y => { y = y.LoadAssociations(db); });

            return results;
        }
    }
}
