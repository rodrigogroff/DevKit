using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class LoteGrafica
    {
		public List<LoteGrafica> Loader(DevKitDB db, List<LoteGrafica> results)
        {
            results.ForEach(y => { y = y.LoadAssociations(db); });

            return results;
        }
    }
}
