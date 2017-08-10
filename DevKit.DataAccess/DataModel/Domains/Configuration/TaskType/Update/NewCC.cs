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
                    resp = "Check Point já usado";
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
                Create(db, "Nova categoria: " + ent.stName, "");
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
                Create(db, "Edição de acumulador: " + ent.stName, "");
            }

            return true;
        }
    }
}
