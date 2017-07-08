using LinqToDB;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace DataModel
{
    public partial class TaskType
    {
        public bool Update_Entity(DevKitDB db, User user, ref string resp)
        {
            new AuditLog
            {
                fkUser = user.id,
                fkActionLog = EnumAuditAction.TaskTypeUpdate,
                nuType = EnumAuditType.TaskType,
                fkTarget = this.id
            }.
                        Create(db, TrackChanges(db), "");

            db.Update(this);

            return true;
        }
    }
}
