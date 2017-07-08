
namespace DataModel
{
	public partial class ClientGroup
	{
		public string TrackChanges(DevKitDB db)
		{
			var ret = "";

			var oldEntity = db.GetClient(this.id);

			if (oldEntity.stName != this.stName)
				ret += "Name: " + oldEntity.stName + " => " + this.stName + ";";

			return ret;
		}
	}
}
