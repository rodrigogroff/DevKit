using LinqToDB;
using Newtonsoft.Json;

namespace DataModel
{
	public partial class Task
	{
		public bool Update_newClientGroup(DevKitDB db, User user, ref string resp)
		{
			var ent = JsonConvert.DeserializeObject<TaskClientGroup>(anexedEntity.ToString());

			ent.fkTask = this.id;

			db.Insert(ent);

			return true;
		}

		public bool Update_removeClientGroup(DevKitDB db, User user, ref string resp)
		{
			var ent = JsonConvert.DeserializeObject<TaskClientGroup>(anexedEntity.ToString());

			db.Delete(ent);
            
			return true;
		}
	}
}
