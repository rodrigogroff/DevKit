using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System;

namespace DataModel
{
	// --------------------------
	// functions
	// --------------------------

	public partial class Setup
	{
		public bool Update(DevKitDB db, ref string resp)
		{
			db.Update(this);

			return true;
		}
	}
}
