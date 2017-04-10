
namespace DataModel
{
	public partial class Setup
	{
		public string TrackChanges(DevKitDB db)
		{
			var ret = "";

			var oldEntity = db.Setup();

			if (oldEntity.stPhoneMask != this.stPhoneMask)
			{
				ret += "PhoneMask: " + oldEntity.stPhoneMask + " => " + this.stPhoneMask;
			}

			if (oldEntity.stDateFormat != this.stDateFormat)
			{
				ret += "DateFormat: " + oldEntity.stDateFormat + " => " + this.stDateFormat;
			}

			if (oldEntity.stProtocolFormat != this.stProtocolFormat)
			{
				ret += "ProtocolFormat: " + oldEntity.stProtocolFormat + " => " + this.stProtocolFormat;
			}
			
			return ret;
		}
	}
}
