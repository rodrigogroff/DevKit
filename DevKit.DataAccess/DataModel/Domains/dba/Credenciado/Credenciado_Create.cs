using LinqToDB;
using System;
using System.Linq;
using System.Threading;

namespace DataModel
{
	// --------------------------
	// functions
	// --------------------------

	public partial class Credenciado
    {
		public bool Create(DevKitDB db, ref string apiError)
		{
            if (this.nuCodigo == null || this.nuCodigo == 0)
            {
                long cod = 0;

                while (true)
                {
                    cod = Convert.ToInt64(new Util().GetRandomString(4));

                    if (!db.Credenciado.Any(y => y.nuCodigo == cod))
                        break;
                }

                this.nuCodigo = cod;
            }

            this.id = Convert.ToInt64(db.InsertWithIdentity(this));

            return true;
		}
	}
}
