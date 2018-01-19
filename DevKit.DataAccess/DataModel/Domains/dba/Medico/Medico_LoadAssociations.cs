using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public partial class Medico
    {
		public Medico LoadAssociations(DevKitDB db)
		{
            this.sfkEspecialidade = db.Especialidade.
                                    Where(y => y.id == this.fkEspecialidade).
                                    FirstOrDefault().
                                    stNome;

            return this;
		}
	}
}
