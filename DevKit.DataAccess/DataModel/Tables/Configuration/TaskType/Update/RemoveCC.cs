using LinqToDB;
using Newtonsoft.Json;

namespace DataModel
{
    public partial class TaskType
    {
        public bool Update_RemoveCC(DevKitDB db, User user, ref string resp)
        {
            var ent = JsonConvert.DeserializeObject<TaskCheckPoint>(anexedEntity.ToString());

            db.Delete(ent);

            new AuditLog
            {
                fkUser = user.id,
                fkActionLog = EnumAuditAction.CategoryRemoveCheckPoint,
                nuType = EnumAuditType.TaskType,
                fkTarget = this.id
            }.
            Create(db, "Checkpoint deleted: " + ent.stName, "");

            return true;
        }
    }
}
