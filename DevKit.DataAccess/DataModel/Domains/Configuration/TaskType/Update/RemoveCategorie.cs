using LinqToDB;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace DataModel
{
    public partial class TaskType
    {
        public bool Update_RemoveCategorie(DevKitDB db, User user, ref string resp)
        {
            var ent = JsonConvert.DeserializeObject<TaskCategory>(anexedEntity.ToString());

            if ((from e in db.Task where e.fkTaskCategory == ent.id select e).Any())
            {
                resp = "Esta categoria já está sendo usado em uma tarefa";
                return false;
            }

            db.Delete(ent);

            new AuditLog
            {
                fkUser = user.id,
                fkActionLog = EnumAuditAction.TaskTypeRemoveCategory,
                nuType = EnumAuditType.TaskType,
                fkTarget = this.id
            }.
            Create(db, "Category removida: " + ent.stName, "");

            return true;
        }
    }
}
