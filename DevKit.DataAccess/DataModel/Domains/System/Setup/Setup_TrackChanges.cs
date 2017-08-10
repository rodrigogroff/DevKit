
namespace DataModel
{
	public partial class Setup
	{
		public string TrackChanges(DevKitDB db)
		{
			var ret = "";

			var oldEntity = db.GetSetup();

			if (oldEntity.stPhoneMask != this.stPhoneMask)
			{
				ret += "Mascara de telefone: " + oldEntity.stPhoneMask + " => " + this.stPhoneMask;
			}

			if (oldEntity.stDateFormat != this.stDateFormat)
			{
				ret += "Formato de data: " + oldEntity.stDateFormat + " => " + this.stDateFormat;
			}

			if (oldEntity.stProtocolFormat != this.stProtocolFormat)
			{
				ret += "Protocol: " + oldEntity.stProtocolFormat + " => " + this.stProtocolFormat;
			}
			
			return ret;
		}
	}
}
