using System.Collections.Generic;

namespace DataModel
{
	public partial class User
	{
		public object anexedEntity;

		public string sdtLastLogin = "",
						sdtCreation = "",
						updateCommand = "";

		public Profile profile;

		public List<UserPhone> phones;
		public List<UserEmail> emails;
		public List<UserLog> logs;
	}

	public class UserLog
	{
		public string sdtLog,
						stUser,
						stDetails;
	}
}
