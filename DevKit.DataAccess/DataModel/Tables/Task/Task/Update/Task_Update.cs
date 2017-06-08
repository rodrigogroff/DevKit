
namespace DataModel
{
	public partial class Task
	{
		public bool Update(DevKitDB db, long fkCurrentUser, ref string resp)
		{
			var user = db.GetCurrentUser(fkCurrentUser);

			switch (updateCommand)
			{
				case "entity": return Update_Entity(db, user, ref resp);
				case "newAcc": return Update_newAcc(db, user, ref resp);
				case "removeAccValue": return Update_removeAccValue(db, user, ref resp);
				case "newSubtask": return Update_newSubtask(db, user, ref resp);
				case "removeSubtask": return Update_removeSubtask(db, user, ref resp);
				case "newQuestion": return Update_newQuestion(db, user, ref resp);
				case "removeQuestion": return Update_removeQuestion(db, user, ref resp);
				case "newClient": return Update_newClient(db, user, ref resp);
				case "removeClient": return Update_removeClient(db, user, ref resp);
				case "newClientGroup": return Update_newClientGroup(db, user, ref resp);
				case "removeClientGroup": return Update_removeClientGroup(db, user, ref resp);
                case "newCustomStep": return Update_newCustomStep(db, user, ref resp);
                case "removeCustomStep": return Update_removeCustomStep(db, user, ref resp);
            }

			return true;
		}
	}
}
