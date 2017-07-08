using LinqToDB;
using Newtonsoft.Json;

namespace DataModel
{
	public partial class Task
	{
		public bool Update_newCustomStep(DevKitDB db, User user, ref string resp)
		{
			var ent = JsonConvert.DeserializeObject<TaskCustomStep>(anexedEntity.ToString());

			ent.fkTask = this.id;
            ent.fkUser = user.id;
            ent.bSelected = false;
            
            db.Insert(ent);
            
			return true;
		}

		public bool Update_removeCustomStep(DevKitDB db, User user, ref string resp)
		{
			var ent = JsonConvert.DeserializeObject<TaskCustomStep>(anexedEntity.ToString());

			db.Delete(ent);
            
            return true;
		}
	}
}
