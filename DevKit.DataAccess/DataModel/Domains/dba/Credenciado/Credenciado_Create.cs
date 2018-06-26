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
            if (db.Credenciado.Any ( y=> y.stCnpj == this.stCnpj))
            {
                apiError = "CPF / CNPJ já utilizado!";
                return false;
            }

            if (this.nuCodigo == null || this.nuCodigo == 0)
            {
                long cod = 2190;

                while (true)
                {
                    if (!db.Credenciado.Any(y => y.nuCodigo == cod))
                        break;

                    cod++;
                }

                this.nuCodigo = cod;
            }

            this.id = Convert.ToInt64(db.InsertWithIdentity(this));

            return true;
		}
	}
}
