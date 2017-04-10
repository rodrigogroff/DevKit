
namespace DataModel
{
	public partial class ProjectSprint
	{
		public string TrackChanges(DevKitDB db)
		{
			var ret = "";

			var oldEntity = db.ProjectSprint(this.id);

			if (oldEntity.stName != this.stName)
			{
				ret += "Name: " + oldEntity.stName + " => " + this.stName + "; ";
			}

			if (oldEntity.stDescription != this.stDescription)
			{
				ret += "Description: " + oldEntity.stDescription + " => " + this.stDescription + "; ";
			}

			return ret;
		}
	}
}
