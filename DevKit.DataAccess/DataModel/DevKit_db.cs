using System.Linq;
using System.Collections;
using System.Threading;
using System.Collections.Generic;

namespace DataModel
{
	public partial class DevKitDB
	{				
		// ----------------------------------
		// Current system user 
		// ----------------------------------

		public User currentUser = null;

		public User GetCurrentUser()
		{
			if (currentUser == null)
				currentUser = (from ne in Users
							   where ne.stLogin.ToUpper() == Thread.CurrentPrincipal.Identity.Name.ToUpper()
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

		// ----------------------------------
		// Tables by Id (using cache)
		// ----------------------------------

		Hashtable Cache = new Hashtable();

		Setup _setup = null;

		public Setup Setup()
		{
			if (_setup == null)
				_setup = Setups.Find(1); 
			return _setup;
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

		public CompanyNews News(long? id)
		{
			if (id == null) return null;
			var tag = "CompanyNews" + id; var ret = Cache[tag] as CompanyNews;
			if (ret == null) { ret = CompanyNews.Find((long)id); Cache[tag] = ret; }
			return ret;
		}

		public Survey Survey(long? id)
		{
			if (id == null) return null;
			var tag = "Survey" + id; var ret = Cache[tag] as Survey;
			if (ret == null) { ret = Surveys.Find((long)id); Cache[tag] = ret; }
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

		public TaskCheckPoint TaskCheckPoint(long? id)
		{
			if (id == null) return null;
			var tag = "TaskCheckPoint" + id; var ret = Cache[tag] as TaskCheckPoint;
			if (ret == null) { ret = TaskCheckPoints.Find((long)id); Cache[tag] = ret; }
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
