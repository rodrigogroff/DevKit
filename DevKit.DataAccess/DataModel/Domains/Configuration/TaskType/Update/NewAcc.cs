using LinqToDB;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace DataModel
{
    public partial class TaskType
    {
        public bool Update_NewAcc(DevKitDB db, User user, ref string resp)
        {
            var ent = JsonConvert.DeserializeObject<TaskTypeAccumulator>(anexedEntity.ToString());

            if (ent.id == 0)
            {
                if ((from e in db.TaskTypeAccumulator
                     where e.fkTaskFlow == ent.fkTaskFlow
                     where e.fkTaskCategory == ent.fkTaskCategory
                     where e.fkTaskType == ent.fkTaskType
                     where e.stName.ToUpper() == ent.stName.ToUpper()
                     select e).Any())
                {
                    resp = "Accumulator already added to task type!";
                    return false;
                }

                ent.fkTaskType = id;

                db.Insert(ent);

                new AuditLog
                {
                    fkUser = user.id,
                    fkActionLog = EnumAuditAction.CategoryAddAccumulator,
                    nuType = EnumAuditType.TaskType,
                    fkTarget = this.id
                }.
                Create(db, "New accumulator: " + ent.stName, "");
            }
            else
            {
                db.Update(ent);

                new AuditLog
                {
                    fkUser = user.id,
                    fkActionLog = EnumAuditAction.CategoryUpdateAccumulator,
                    nuType = EnumAuditType.TaskType,
                    fkTarget = this.id
                }.
                Create(db, "Edit accumulator: " + ent.stName, "");
            }

            return true;
        }
    }
}
