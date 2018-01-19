using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public partial class Empresa
	{
		public Empresa LoadAssociations(DevKitDB db)
		{
            var setup = db.GetSetup();

            sqtdCartoes = db.Person.
                            Where(y => y.fkEmpresa == id).
                                Count().
                                ToString();

            return this;
		}
	}
}
