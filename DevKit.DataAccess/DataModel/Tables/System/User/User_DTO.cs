using System.Collections.Generic;

namespace DataModel
{
	public partial class User
	{
        public LoginInfo login;

        public object anexedEntity;

		public string sdtLastLogin = "",
						sdtCreation = "",
						updateCommand = "",
						resetPassword = "";

		public Profile profile;
        
		public List<UserPhone> phones;
		public List<UserEmail> emails;
		public List<UserLog> logs;
	}

	public class UserPasswordChange
	{
		public string stCurrentPassword,
						stNewPassword;
	}

	public class UserLog
	{
		public string sdtLog,
						stUser,
						stDetails;
	}
}
