using LinqToDB;
using Newtonsoft.Json;

namespace DataModel
{
    public partial class TaskType
    {
        public bool Update_RemoveAcc(DevKitDB db, User user, ref string resp)
        {
            var ent = JsonConvert.DeserializeObject<TaskTypeAccumulator>(anexedEntity.ToString());

            db.Delete(ent);

            new AuditLog
            {
                fkUser = user.id,
                fkActionLog = EnumAuditAction.CategoryRemoveAccumulator,
                nuType = EnumAuditType.TaskType,
                fkTarget = this.id
            }.
            Create(db, "Acumulador removido: " + ent.stName, "");

            return true;
        }
    }
}
