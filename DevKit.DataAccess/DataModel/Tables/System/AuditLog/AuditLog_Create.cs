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

			if (stLog.Length > 999)
				stLog = stLog.Substring(0, 996) + "...";

			id = Convert.ToInt64(db.InsertWithIdentity(this));

			return true;
		}
	}
}
