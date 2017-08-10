
namespace DataModel
{
	public partial class TaskType
	{
		public string TrackChanges(DevKitDB db)
		{
			var ret = "";

			var oldEntity = db.GetTaskType(this.id);

			if (oldEntity.stName != this.stName)
			{
				ret += "Nome: " + oldEntity.stName + " => " + this.stName + "; ";
			}

			if (oldEntity.bManaged != this.bManaged)
			{
				if (oldEntity.bManaged != null)
					ret += "Gerenciado: " + oldEntity.bManaged + " => " + this.bManaged + "; ";
				else
					ret += "Gerenciado: (null) => " + this.bManaged + "; ";
			}

			if (oldEntity.bCondensedView != this.bCondensedView)
			{
				if (oldEntity.bCondensedView != null)
					ret += "Condensado: " + oldEntity.bCondensedView + " => " + this.bCondensedView + "; ";
				else
					ret += "Condensado: (null) => " + this.bCondensedView + "; ";
			}

			if (oldEntity.bKPA != this.bKPA)
			{
				if (oldEntity.bKPA != null)
					ret += "KPA: " + oldEntity.bKPA + " => " + this.bKPA + "; ";
				else
					ret += "KPA: (null) => " + this.bKPA + "; ";
			}

			return ret;
		}
	}
}
