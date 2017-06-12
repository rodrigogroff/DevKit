using System.Collections.Generic;

namespace DataModel
{
	public partial class Profile
	{
        public LoginInfo login;

        public int qttyPermissions = 0;

		public List<User> users;
		public List<ProfileLog> logs;
	}

    public class ProfileReport
    {
        public int count = 0;
        public List<Profile> results = new List<Profile>();
    }

    public class ProfileLog
	{
		public string	sdtLog, 
						stUser, 
						stDetails;
	}
}
