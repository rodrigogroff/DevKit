using LinqToDB;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace DataModel
{
    public partial class TaskType
    {
        public bool Update_NewCategorie(DevKitDB db, User user, ref string resp)
        {
            var ent = JsonConvert.DeserializeObject<TaskCategory>(anexedEntity.ToString());

            ent.fkTaskType = id;

            if (ent.id == 0)
            {
                if ((from ne in db.TaskCategory
                     where ne.stName.ToUpper() == ent.stName.ToUpper()
                     where ne.fkTaskType == id
                     select ne).Any())
                {
                    resp = "Category already added to task type!";
                    return false;
                }

                ent.id = Convert.ToInt64(db.InsertWithIdentity(ent));

                int order = 1;

                db.Insert(new TaskFlow()
                {
                    fkTaskCategory = ent.id,
                    fkTaskType = id,
                    nuOrder = order++,
                    bForceOpen = true,
                    stName = "Open"
                });

                db.Insert(new TaskFlow()
                {
                    fkTaskCategory = ent.id,
                    fkTaskType = id,
                    nuOrder = order++,
                    bForceOpen = true,
                    stName = "Re-Open"
                });

                db.Insert(new TaskFlow()
                {
                    fkTaskCategory = ent.id,
                    fkTaskType = id,
                    nuOrder = order++,
                    stName = "Analysis"
                });

                db.Insert(new TaskFlow()
                {
                    fkTaskCategory = ent.id,
                    fkTaskType = id,
                    nuOrder = order++,
                    stName = "Development"
                });

                db.Insert(new TaskFlow()
                {
                    fkTaskCategory = ent.id,
                    fkTaskType = id,
                    nuOrder = order++,
                    bForceComplete = true,
                    stName = "Closed"
                });

                db.Insert(new TaskFlow()
                {
                    fkTaskCategory = ent.id,
                    fkTaskType = id,
                    nuOrder = order++,
                    bForceComplete = true,
                    stName = "Cancelled"
                });

                new AuditLog
                {
                    fkUser = user.id,
                    fkActionLog = EnumAuditAction.TaskTypeUpdate,
                    nuType = EnumAuditType.TaskType,
                    fkTarget = this.id
                }.
                Create(db, "New category: " + ent.stName, "");
            }
            else
            {
                if (ent.nuExpiresMinutes != null || ent.nuExpiresHours != null || ent.nuExpiresDays != null)
                    ent.bExpires = true;
                else
                    ent.bExpires = false;

                db.Update(ent);

                new AuditLog
                {
                    fkUser = user.id,
                    fkActionLog = EnumAuditAction.TaskTypeUpdateCategory,
                    nuType = EnumAuditType.TaskType,
                    fkTarget = this.id
                }.
                Create(db, "Edit category: " + ent.stName, "");
            }

            return true;
        }
    }
}
