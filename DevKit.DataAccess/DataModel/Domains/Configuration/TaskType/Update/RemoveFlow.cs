using LinqToDB;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace DataModel
{
    public partial class TaskType
    {
        public bool Update_RemoveFlow(DevKitDB db, User user, ref string resp)
        {
            var ent = JsonConvert.DeserializeObject<TaskFlow>(anexedEntity.ToString());

            if ((from e in db.Task where e.fkTaskFlowCurrent == ent.id select e).Any())
            {
                resp = "This flow is being used in a task";
                return false;
            }

            db.Delete(ent);

            new AuditLog
            {
                fkUser = user.id,
                fkActionLog = EnumAuditAction.CategoryRemoveFlow,
                nuType = EnumAuditType.TaskType,
                fkTarget = this.id
            }.
            Create(db, "Flow delete: " + ent.stName, "");

            return true;
        }
    }
}
