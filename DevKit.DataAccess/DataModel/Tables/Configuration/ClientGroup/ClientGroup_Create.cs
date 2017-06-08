﻿using LinqToDB;
using System;
using System.Linq;

namespace DataModel
{
	public partial class ClientGroup
	{
		bool CheckDuplicate(ClientGroup item, DevKitDB db)
		{
			var query = from e in db.ClientGroup select e;

			if (item.stName != null)
			{
				var _st = item.stName.ToUpper();
				query = from e in query where e.stName.ToUpper().Contains(_st) select e;
			}

			if (item.id > 0)
				query = from e in query where e.id != item.id select e;

			return query.Any();
		}
		
		public bool Create(DevKitDB db, long fkCurrentUser, ref string resp)
		{
			var user = db.GetCurrentUser(fkCurrentUser);

			if (CheckDuplicate(this, db))
			{
				resp = "Client name already taken";
				return false;
			}

			var setup = db.GetSetup();
			
			dtStart = DateTime.Now;
			fkUser = user.id;
			
			id = Convert.ToInt64(db.InsertWithIdentity(this));
			
			new AuditLog {
				fkUser = user.id,
				fkActionLog = EnumAuditAction.ClientGroupNew,
				nuType = EnumAuditType.ClientGroup,
				fkTarget = this.id
			}.
			Create(db, "name: " + stName, "");

			return true;
		}
	}
}
