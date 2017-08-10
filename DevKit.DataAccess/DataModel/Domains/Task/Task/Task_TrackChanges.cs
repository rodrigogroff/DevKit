
namespace DataModel
{
	public partial class Task
	{
		public string TrackChanges(DevKitDB db)
		{
			var ret = "";

			var oldEntity = db.GetTask(this.id);

			if (oldEntity.stDescription != this.stDescription)
				ret += "Descrição: " + oldEntity.stDescription + " => " + this.stDescription + ";";

			if (oldEntity.stLocalization != this.stLocalization)
				ret += "Localização: " + oldEntity.stLocalization + " => " + this.stLocalization + ";";

			if (oldEntity.stTitle != this.stTitle)
				ret += "Título: " + oldEntity.stTitle + " => " + this.stTitle + ";";

			return ret;
		}
	}
}
