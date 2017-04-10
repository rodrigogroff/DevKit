using System.Collections.Generic;

namespace DataModel
{
	public partial class Project
	{
		public string TrackChanges(DevKitDB db)
		{
			var ret = "";

			var oldEntity = db.Project(this.id);

			if (oldEntity.stName != this.stName)
			{
				ret += "From: " + oldEntity.stName + " => " + this.stName + ";";
			}

			return ret;
		}
	}
}
