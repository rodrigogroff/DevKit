
namespace DataModel
{
	public partial class Client
	{
		public string TrackChanges(DevKitDB db)
		{
			var ret = "";

			var oldEntity = db.GetClient(this.id);

			if (oldEntity.stName != this.stName)
				ret += "Nome: " + oldEntity.stName + " => " + this.stName + ";";

			return ret;
		}
	}
}
