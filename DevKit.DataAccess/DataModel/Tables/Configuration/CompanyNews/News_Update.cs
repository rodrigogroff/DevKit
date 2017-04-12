using LinqToDB;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace DataModel
{
	public partial class CompanyNews
	{
		public bool Update(DevKitDB db, ref string resp)
		{
			var user = db.GetCurrentUser();

			if (CheckDuplicate(this, db))
			{
				resp = "News type already taken";
				return false;
			}

			switch (updateCommand)
			{
				case "entity":
					{
						/*
						new AuditLog {
							fkUser = user.id,
							fkActionLog = EnumAuditAction.ProjectUpdate,
							nuType = EnumAuditType.Project,
							fkTarget = this.id
						}.
						Create(db, TrackChanges(db), "");
						*/

						db.Update(this);

						//logs = LoadLogs(db);

						break;
					}
			}

			return true;
		}
	}
}
