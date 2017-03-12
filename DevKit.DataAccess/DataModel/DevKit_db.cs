using System.Collections;

namespace DataModel
{
	public partial class DevKitDB
	{
		private Hashtable Cache = new Hashtable();
				
		public Setup Setup()
		{
			var ret = Cache["Setup"] as Setup;
			if (ret == null) { ret = Setups.Find(1); Cache["Setup"] = ret; }
			return ret;
		}

		public User User(long? id)
		{
			var tag = "User" + id; var ret = Cache[tag] as User;
			if (ret == null) { ret = Users.Find((long)id); Cache[tag] = ret; }
			return ret;
		}

		public Profile Profile(long? id)
		{
			var tag = "Profile" + id; var ret = Cache[tag] as Profile;
			if (ret == null) { ret = Profiles.Find((long)id); Cache[tag] = ret; }
			return ret;
		}

		public Project Project(long? id)
		{
			var tag = "Project" + id; var ret = Cache[tag] as Project;
			if (ret == null) { ret = Projects.Find((long)id); Cache[tag] = ret; }
			return ret;
		}

		public ProjectPhase ProjectPhase(long? id)
		{
			var tag = "ProjectPhase" + id; var ret = Cache[tag] as ProjectPhase;
			if (ret == null) { ret = ProjectPhases.Find((long)id); Cache[tag] = ret; }
			return ret;
		}

		public ProjectSprint ProjectSprint(long? id)
		{
			var tag = "ProjectSprint" + id; var ret = Cache[tag] as ProjectSprint;
			if (ret == null) { ret = ProjectSprints.Find((long)id); Cache[tag] = ret; }
			return ret;
		}

		public ProjectSprintVersion ProjectSprintVersion(long? id)
		{
			var tag = "ProjectSprintVersion" + id; var ret = Cache[tag] as ProjectSprintVersion;
			if (ret == null) { ret = ProjectSprintVersions.Find((long)id); Cache[tag] = ret; }
			return ret;
		}
	}
}
