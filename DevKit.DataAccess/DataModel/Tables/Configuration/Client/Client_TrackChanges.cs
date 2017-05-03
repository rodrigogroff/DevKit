
namespace DataModel
{
	public partial class Client
	{
		public string TrackChanges(DevKitDB db)
		{
			var ret = "";

			var oldEntity = db.Client(this.id);

			if (oldEntity.stName != this.stName)
				ret += "Name: " + oldEntity.stName + " => " + this.stName + ";";

			return ret;
		}
	}
}
