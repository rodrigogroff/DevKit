using System.Linq;
using System.Collections;
using System.Threading;
using System.Collections.Generic;

namespace DataModel
{
	public partial class DevKitDB
	{
		string identityName = Thread.CurrentPrincipal.Identity.Name.ToUpper();

		public User currentUser = null;

		public User GetCurrentUser()
		{
			if (currentUser == null)
				currentUser = (from ne in Users
							   where ne.stLogin.ToUpper() == identityName
							   select ne).FirstOrDefault();

			return currentUser;
		}

		public List<long?> GetCurrentUserProjects()
		{
			if (currentUser == null)
				currentUser = GetCurrentUser();

			return (from e in ProjectUsers
					where e.fkUser == currentUser.id
					select e.fkProject).
					ToList();
		}

		public List<long?> GetCurrentUserProjects(long userId)
		{
			return (from e in ProjectUsers
					where e.fkUser == userId
					select e.fkProject).
					ToList();
		}

		private Hashtable Cache = new Hashtable();
				
		public Setup Setup()
		{
			var ret = Cache["Setup"] as Setup;
			if (ret == null) { ret = Setups.Find(1); Cache["Setup"] = ret; }
			return ret;
		}

		public User User(long? id)
		{
			if (id == null) return null;
			var tag = "User" + id; var ret = Cache[tag] as User;
			if (ret == null) { ret = Users.Find((long)id); Cache[tag] = ret; }
			return ret;
		}

		public Profile Profile(long? id)
		{
			if (id == null) return null;
			var tag = "Profile" + id; var ret = Cache[tag] as Profile;
			if (ret == null) { ret = Profiles.Find((long)id); Cache[tag] = ret; }
			return ret;
		}

		public Project Project(long? id)
		{
			if (id == null) return null;
			var tag = "Project" + id; var ret = Cache[tag] as Project;
			if (ret == null) { ret = Projects.Find((long)id); Cache[tag] = ret; }
			return ret;
		}

		public ProjectPhase ProjectPhase(long? id)
		{
			if (id == null) return null;
			var tag = "ProjectPhase" + id; var ret = Cache[tag] as ProjectPhase;
			if (ret == null) { ret = ProjectPhases.Find((long)id); Cache[tag] = ret; }
			return ret;
		}

		public ProjectSprint ProjectSprint(long? id)
		{
			if (id == null) return null;
			var tag = "ProjectSprint" + id; var ret = Cache[tag] as ProjectSprint;
			if (ret == null) { ret = ProjectSprints.Find((long)id); Cache[tag] = ret; }
			return ret;
		}

		public ProjectSprintVersion ProjectSprintVersion(long? id)
		{
			if (id == null) return null;
			var tag = "ProjectSprintVersion" + id; var ret = Cache[tag] as ProjectSprintVersion;
			if (ret == null) { ret = ProjectSprintVersions.Find((long)id); Cache[tag] = ret; }
			return ret;
		}

		public TaskType TaskType(long? id)
		{
			if (id == null) return null;
			var tag = "TaskType" + id; var ret = Cache[tag] as TaskType;
			if (ret == null) { ret = TaskTypes.Find((long)id); Cache[tag] = ret; }
			return ret;
		}

		public TaskTypeAccumulator TaskTypeAccumulator(long? id)
		{
			if (id == null) return null;
			var tag = "TaskTypeAccumulator" + id; var ret = Cache[tag] as TaskTypeAccumulator;
			if (ret == null) { ret = TaskTypeAccumulators.Find((long)id); Cache[tag] = ret; }
			return ret;
		}

		public TaskCategory TaskCategory(long? id)
		{
			if (id == null) return null;
			var tag = "TaskCategory" + id; var ret = Cache[tag] as TaskCategory;
			if (ret == null) { ret = TaskCategories.Find((long)id); Cache[tag] = ret; }
			return ret;
		}

		public TaskFlow TaskFlow(long? id)
		{
			if (id == null) return null;
			var tag = "TaskFlow" + id; var ret = Cache[tag] as TaskFlow;
			if (ret == null) { ret = TaskFlows.Find((long)id); Cache[tag] = ret; }
			return ret;
		}

		public Task Task(long? id)
		{
			if (id == null) return null;
			var tag = "Task" + id; var ret = Cache[tag] as Task;
			if (ret == null) { ret = Tasks.Find((long)id); Cache[tag] = ret; }
			return ret;
		}
	}
}
