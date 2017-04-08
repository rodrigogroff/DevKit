using LinqToDB;
using System;

namespace DataModel
{
	public partial class AuditLog
	{
		public bool Create(DevKitDB db, string _stLog, string _stDetail)
		{
			stLog = _stLog;
			stDetailLog = _stDetail;
			dtLog = DateTime.Now;

			id = Convert.ToInt64(db.InsertWithIdentity(this));

			return true;
		}
	}
}
