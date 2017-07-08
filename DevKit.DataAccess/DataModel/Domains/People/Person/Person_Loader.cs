using System.Collections.Generic;

namespace DataModel
{
	public partial class Person
	{
		public List<Person> Loader(DevKitDB db, List<Person> results)
        {
            results.ForEach(y => { y = y.LoadAssociations(db); });

            return results;
        }
    }
}
