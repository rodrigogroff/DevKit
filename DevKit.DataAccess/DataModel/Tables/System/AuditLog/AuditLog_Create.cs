using LinqToDB;
using System;

namespace DataModel
{
	public partial class AuditLog
	{
		public bool Create(DevKitDB db, string log, string detail)
		{
			stLog = log;
			stDetailLog = detail;
			dtLog = DateTime.Now;

			id = Convert.ToInt64(db.InsertWithIdentity(this));

			return true;
		}
	}
}
