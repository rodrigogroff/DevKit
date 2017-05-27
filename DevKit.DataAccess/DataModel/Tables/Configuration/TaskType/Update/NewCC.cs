using LinqToDB;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace DataModel
{
    public partial class TaskType
    {
        public bool Update_NewCC(DevKitDB db, User user, ref string resp)
        {
            var ent = JsonConvert.DeserializeObject<TaskCheckPoint>(anexedEntity.ToString());

            if (ent.id == 0)
            {
                if ((from e in db.TaskCheckPoint
                     where e.fkCategory == ent.fkCategory
                     where e.stName.ToUpper() == ent.stName.ToUpper()
                     select e).Any())
                {
                    resp = "Check Point already added to task type!";
                    return false;
                }

                db.Insert(ent);

                new AuditLog
                {
                    fkUser = user.id,
                    fkActionLog = EnumAuditAction.CategoryAddAccumulator,
                    nuType = EnumAuditType.TaskType,
                    fkTarget = this.id
                }.
                Create(db, "New category check point: " + ent.stName, "");
            }
            else
            {
                db.Update(ent);

                new AuditLog
                {
                    fkUser = user.id,
                    fkActionLog = EnumAuditAction.CategoryEditCheckPoint,
                    nuType = EnumAuditType.TaskType,
                    fkTarget = this.id
                }.
                Create(db, "Edit accumulator: " + ent.stName, "");
            }

            return true;
        }
    }
}
