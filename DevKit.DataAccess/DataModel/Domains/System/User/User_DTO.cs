using System.Collections.Generic;

namespace DataModel
{
	public partial class User
	{
        public object anexedEntity;

		public string sdtLastLogin = "",
						sdtCreation = "",
						updateCommand = "",
						resetPassword = "";

		public Profile profile;
        public Empresa empresa;
        
		public List<UserPhone> phones;
		public List<UserEmail> emails;
		public List<UserLog> logs;
	}

    public class UserReport
    {
        public int count = 0;
        public List<User> results = new List<User>();
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
