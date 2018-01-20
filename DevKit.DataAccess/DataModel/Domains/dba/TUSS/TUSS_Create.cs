using LinqToDB;
using System;
using System.Linq;
using System.Threading;

namespace DataModel
{
	// --------------------------
	// functions
	// --------------------------

	public partial class TUSS
    {
		public bool Create(DevKitDB db, ref string apiError)
		{
            this.id = Convert.ToInt64(db.InsertWithIdentity(this));

            return true;
		}
	}
}
