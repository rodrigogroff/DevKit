using System.Collections.Generic;

namespace DataModel
{
	public partial class Profile
	{
        public int qttyPermissions = 0;

        public List<User> users = new List<User>();
		public List<ProfileLog> logs = new List<ProfileLog>();
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
