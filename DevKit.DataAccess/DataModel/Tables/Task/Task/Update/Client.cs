using LinqToDB;
using Newtonsoft.Json;

namespace DataModel
{
	public partial class Task
	{
		public bool Update_newClient(DevKitDB db, User user, ref string resp)
		{
			var ent = JsonConvert.DeserializeObject<TaskClient>(anexedEntity.ToString());

			ent.fkTask = this.id;

			db.Insert(ent);

			return true;
		}

		public bool Update_removeClient(DevKitDB db, User user, ref string resp)
		{
			var ent = JsonConvert.DeserializeObject<TaskClient>(anexedEntity.ToString());

			db.Delete(ent);

			return true;
		}
	}
}
