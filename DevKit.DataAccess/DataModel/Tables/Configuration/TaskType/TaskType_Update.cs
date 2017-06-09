
namespace DataModel
{
    public partial class TaskType
    {
        public bool Update(DevKitDB db, ref string resp)
        {
            var user = db.currentUser;

            if (CheckDuplicate(this, db))
            {
                resp = "Task type name already taken";
                return false;
            }

            switch (updateCommand)
            {
                case "entity": return Update_Entity(db, user, ref resp);
                case "newCategorie": return Update_NewCategorie(db, user, ref resp);
                case "removeCategorie": return Update_RemoveCategorie(db, user, ref resp);
                case "newFlow": return Update_NewFlow(db, user, ref resp);
                case "removeFlow": return Update_RemoveFlow(db, user, ref resp);
                case "newAcc": return Update_NewAcc(db, user, ref resp);
                case "removeAcc": return Update_RemoveAcc(db, user, ref resp);
                case "newCC": return Update_NewCC(db, user, ref resp);
                case "removeCC": return Update_RemoveCC(db, user, ref resp);
            }

            return true;
        }
    }
}
