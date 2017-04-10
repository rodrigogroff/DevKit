
namespace DataModel
{
	public partial class Task
	{
		public string TrackChanges(DevKitDB db)
		{
			var ret = "";

			var oldEntity = db.Task(this.id);

			if (oldEntity.stDescription != this.stDescription)
				ret += "Description: " + oldEntity.stDescription + " => " + this.stDescription + ";";

			if (oldEntity.stLocalization != this.stLocalization)
				ret += "Localization: " + oldEntity.stLocalization + " => " + this.stLocalization + ";";

			if (oldEntity.stTitle != this.stTitle)
				ret += "Title: " + oldEntity.stTitle + " => " + this.stTitle + ";";

			return ret;
		}
	}
}
