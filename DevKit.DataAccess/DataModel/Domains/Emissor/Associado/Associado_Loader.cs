using System.Collections.Generic;

namespace DataModel
{
	public partial class Associado
    {
		public List<Associado> Loader(DevKitDB db, List<Associado> results)
        {
            results.ForEach(y => { y = y.LoadAssociations(db); });

            return results;
        }
    }
}
