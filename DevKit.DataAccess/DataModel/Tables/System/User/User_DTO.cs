using System.Collections.Generic;

namespace DataModel
{
	public partial class User
	{ 
		public string sdtLastLogin = "";
		public string sdtCreation = "";

		public Profile profile;

		public List<UserPhone> phones;
		public List<UserEmail> emails;
		
		public string updateCommand = "";
		public object anexedEntity;
	}
}
