
namespace DataModel
{
	public partial class Associado
    {
		public string TrackChanges(DevKitDB db, ref bool bProfileChanged)
		{
			var ret = "";

			var oldEntity = db.GetAssociado(this.id);
			
			if(oldEntity.stName != this.stName)
				ret += "Name: " + oldEntity.stName + " => " + this.stName + "; ";

			return ret;
		}
	}
}
