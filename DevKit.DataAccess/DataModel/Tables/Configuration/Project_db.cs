using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace DataModel
{
	public class ProjectFilter
	{
		public int skip, take;
		public string busca;
	}

	// --------------------------
	// properties
	// --------------------------

	public partial class Project
	{
		public string stUser = "";
		public string sdtCreation = "";

		public List<ProjectUser> users;
		public List<ProjectPhase> phases;
		public List<ProjectSprint> sprints;

		public string updateCommand = "";
		public object anexedEntity;
	}

	public partial class ProjectUser
	{
		public string stUser = "";
		public string sdtJoin = "";
	}

	public partial class ProjectPhase
	{
		public string sdtStart = "";
		public string sdtEnd = "";
	}

	// --------------------------
	// functions
	// --------------------------

	public partial class Project
	{
		public IQueryable<Project> ComposedFilters(DevKitDB db, ProjectFilter filter, List<long?> lstUserProjects)
		{
			var query = from e in db.Projects select e;

			if (filter.busca != null)
				query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			if (lstUserProjects.Count() > 0)
				query = from e in query where lstUserProjects.Contains(e.id) select e;

			return query;
		}

		public Project LoadAssociations(DevKitDB db)
		{
			var setup = db.Setup();

			var mdlUser = db.User(this.fkUser);

			stUser = mdlUser?.stLogin;
			sdtCreation = dtCreation?.ToString(setup.stDateFormat);

			users = LoadUsers(db);
			phases = LoadPhases(db);
			sprints = LoadSprints(db);

			return this;
		}

		List<ProjectSprint> LoadSprints(DevKitDB db)
		{
			var lst = (from e in db.ProjectSprints where e.fkProject == id select e).
				OrderBy(t => t.id).
				ToList();

			return lst;
		}

		List<ProjectUser> LoadUsers(DevKitDB db)
		{
			var setup = db.Setup();

			var lst = (from e in db.ProjectUsers where e.fkProject == id select e).
				OrderBy(t => t.id).
				ToList();

			foreach (var item in lst)
			{
				item.stUser = db.User(item.fkUser).stLogin;
				item.sdtJoin = item.dtJoin?.ToString(setup.stDateFormat);
			}

			return lst;
		}

		List<ProjectPhase> LoadPhases(DevKitDB db)
		{
			var setup = db.Setup();

			var lst = (from e in db.ProjectPhases where e.fkProject == id select e).
				OrderBy(t => t.id).
				ToList();

			foreach (var item in lst)
			{
				item.sdtStart = item.dtStart?.ToString(setup.stDateFormat);
				item.sdtEnd = item.dtEnd?.ToString(setup.stDateFormat);
			}

			return lst;
		}

		bool CheckDuplicate(Project item, DevKitDB db)
		{
			var query = from e in db.Projects select e;

			if (item.stName != null)
			{
				var _st = item.stName.ToUpper();
				query = from e in query where e.stName.ToUpper().Contains(_st) select e;
			}

			if (item.id > 0)
				query = from e in query where e.id != item.id select e;

			return query.Any();
		}

		public bool Create(DevKitDB db, User usr, ref string resp)
		{
			if (CheckDuplicate(this, db))
			{
				resp = "Project name already taken";
				return false;
			}

			dtCreation = DateTime.Now;
			fkUser = usr.id;
			
			id = Convert.ToInt64(db.InsertWithIdentity(this));

			var enum_projectTemplate = new EnumProjectTemplate();

			switch ((long)fkProjectTemplate)
			{
				case EnumProjectTemplate.Custom:
					break;

				case EnumProjectTemplate.CMMI2:
										
					var ttypePlanning = new TaskType { fkProject = id, stName = "Software Planning" };

					ttypePlanning.id = Convert.ToInt64(db.InsertWithIdentity(ttypePlanning));

					var categScope = new TaskCategory { bManagement = true, fkTaskType = ttypePlanning.id, stAbreviation = "GPR1", stName = "Scope", stDescription = "" };

					categScope.id = Convert.ToInt64(db.InsertWithIdentity(categScope));

					InsertCMMI2_DocumentationFlows(db, ttypePlanning.id, categScope.id);

					var categProducts = new TaskCategory { bManagement = true, fkTaskType = ttypePlanning.id, stAbreviation = "GPR2", stName = "Products", stDescription = "" };

					categProducts.id = Convert.ToInt64(db.InsertWithIdentity(categProducts));

					InsertCMMI2_DocumentationFlows(db, ttypePlanning.id, categProducts.id);

					var categLifecycle = new TaskCategory { bManagement = true, fkTaskType = ttypePlanning.id, stAbreviation = "GPR3", stName = "Lifecycle", stDescription = "" };

					categLifecycle.id = Convert.ToInt64(db.InsertWithIdentity(categLifecycle));

					InsertCMMI2_DocumentationFlows(db, ttypePlanning.id, categLifecycle.id);

					var categEstimate = new TaskCategory { bManagement = true, fkTaskType = ttypePlanning.id, stAbreviation = "GPR4", stName = "Cost estimation", stDescription = "" };

					categEstimate.id = Convert.ToInt64(db.InsertWithIdentity(categEstimate));

					InsertCMMI2_DocumentationFlows(db, ttypePlanning.id, categEstimate.id);

					var categSchedule = new TaskCategory { bManagement = true, fkTaskType = ttypePlanning.id, stAbreviation = "GPR5", stName = "Schedule", stDescription = "" };

					categSchedule.id = Convert.ToInt64(db.InsertWithIdentity(categSchedule));

					InsertCMMI2_DocumentationFlows(db, ttypePlanning.id, categSchedule.id);

					var categRisk = new TaskCategory { bManagement = true, fkTaskType = ttypePlanning.id, stAbreviation = "GPR6", stName = "Risks", stDescription = "" };

					categRisk.id = Convert.ToInt64(db.InsertWithIdentity(categRisk));

					InsertCMMI2_DocumentationFlows(db, ttypePlanning.id, categRisk.id);

					var categResources = new TaskCategory { bManagement = true, fkTaskType = ttypePlanning.id, stAbreviation = "GPR7", stName = "Resources", stDescription = "" };

					categResources.id = Convert.ToInt64(db.InsertWithIdentity(categResources));

					InsertCMMI2_DocumentationFlows(db, ttypePlanning.id, categResources.id);

					var categData = new TaskCategory { bManagement = true, fkTaskType = ttypePlanning.id, stAbreviation = "GPR8", stName = "Project Data", stDescription = "" };

					categData.id = Convert.ToInt64(db.InsertWithIdentity(categData));

					InsertCMMI2_DocumentationFlows(db, ttypePlanning.id, categData.id);

					var categExePlan = new TaskCategory { bManagement = true, fkTaskType = ttypePlanning.id, stAbreviation = "GPR9", stName = "Execution Plan", stDescription = "" };

					categExePlan.id = Convert.ToInt64(db.InsertWithIdentity(categExePlan));

					InsertCMMI2_DocumentationFlows(db, ttypePlanning.id, categExePlan.id);

					break;
			}

			return true;
		}

		public void InsertCMMI2_DocumentationFlows(DevKitDB db, long _fktype, long _fkcateg)
		{
			db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = true, fkTaskType = _fktype, fkTaskCategory = _fkcateg, nuOrder = 1, stName = "Open" });
			db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = true, fkTaskType = _fktype, fkTaskCategory = _fkcateg, nuOrder = 2, stName = "Revision" });
			db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = null, fkTaskType = _fktype, fkTaskCategory = _fkcateg, nuOrder = 3, stName = "Development" });
			db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = null, fkTaskType = _fktype, fkTaskCategory = _fkcateg, nuOrder = 4, stName = "Peer Review" });
			db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = null, fkTaskType = _fktype, fkTaskCategory = _fkcateg, nuOrder = 5, stName = "Done" });
		}
		
		public bool Update(DevKitDB db, ref string resp)
		{
			if (CheckDuplicate(this, db))
			{
				resp = "Project name already taken";
				return false;
			}

			switch (updateCommand)
			{
				case "entity":
					{
						db.Update(this);
						break;
					}

				case "newUser":
					{
						var ent = JsonConvert.DeserializeObject<ProjectUser>(anexedEntity.ToString());

						if ((from ne in db.ProjectUsers
							 where ne.fkUser == ent.fkUser && ne.fkProject == id
							 select ne).Any())
						{
							resp = "User already added to project!";
							return false;
						}

						ent.fkProject = id;
						ent.dtJoin = DateTime.Now;

						db.Insert(ent);
						users = LoadUsers(db);
						break;
					}

				case "removeUser":
					{
						db.Delete(JsonConvert.DeserializeObject<ProjectUser>(anexedEntity.ToString()));
						users = LoadUsers(db);
						break;
					}

				case "newPhase":
					{
						var ent = JsonConvert.DeserializeObject<ProjectPhase>(anexedEntity.ToString());

						if ((from ne in db.ProjectPhases
							 where ne.stName.ToUpper() == ent.stName.ToUpper() && ne.fkProject == id
							 select ne).Any())
						{
							resp = "Phase already added to project!";
							return false;
						}

						ent.fkProject = id;
						
						db.Insert(ent);
						phases = LoadPhases(db);
						break;
					}
										
				case "removePhase":
					{
						var phaseDel = JsonConvert.DeserializeObject<ProjectPhase>(anexedEntity.ToString());

						if ((from e in db.Tasks where e.fkPhase == phaseDel.id select e).Any())
						{
							resp = "This phase is being used in a task";
							return false;
						}
						
						db.Delete(phaseDel);
						phases = LoadPhases(db);
						break;
					}
			}

			return true;
		}

		public bool CanDelete(DevKitDB db, ref string resp)
		{
			if ((from e in db.Tasks where e.fkProject == id select e).Any())
			{
				resp = "This project is being used in a task";
				return false;
			}

			return true;
		}

		public void Delete(DevKitDB db)
		{
			foreach (var item in (from e in db.ProjectSprints where e.fkProject == id select e))
			{
				item.Delete(db);
			}

			foreach (var item in (from e in db.ProjectPhases where e.fkProject == id select e))
			{
				db.Delete(item);
			}

			foreach (var item in (from e in db.ProjectUsers where e.fkProject == id select e))
			{
				db.Delete(item);
			}

			db.Delete(this);
		}
	}
}
