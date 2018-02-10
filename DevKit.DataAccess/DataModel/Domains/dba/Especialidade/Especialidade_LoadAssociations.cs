using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public partial class Especialidade
    {
		public Especialidade LoadAssociations(DevKitDB db)
		{
            var setup = db.GetSetup();

            sqtdCredenciados = db.Credenciado.Count(y => y.fkEspecialidade == this.id).ToString();

            return this;
		}
	}
}
