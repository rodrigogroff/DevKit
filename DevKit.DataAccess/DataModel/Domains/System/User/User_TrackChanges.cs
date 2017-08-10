
namespace DataModel
{
	public partial class User
	{
		public string TrackChanges(DevKitDB db, ref bool bProfileChanged)
		{
			var ret = "";

			var oldEntity = db.GetUser(id);
			
			if(oldEntity.stLogin != stLogin)
				ret += "Login: " + oldEntity.stLogin + " => " + stLogin + "; ";

			if (oldEntity.bActive != bActive)
				ret += "Ativo: " + oldEntity.bActive + " => " + bActive + "; ";

            if (oldEntity.fkProfile != fkProfile)
            {
                ret += "Perfil: " + db.GetProfile(oldEntity.fkProfile) + " => " + db.GetProfile(fkProfile) + "; ";
                bProfileChanged = true;
            }

			return ret;
		}
	}
}
