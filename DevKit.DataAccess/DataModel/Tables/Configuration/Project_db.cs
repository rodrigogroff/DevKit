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

			db.Insert ( new ProjectUser { dtJoin = DateTime.Now, fkProject = id, fkUser = fkUser, stRole = "Creator" } );

			var enum_projectTemplate = new EnumProjectTemplate();

			switch ((long)fkProjectTemplate)
			{
				case EnumProjectTemplate.Custom:
					break;

				case EnumProjectTemplate.CMMI2:

					{
						var ttypePlanning = new TaskType { fkProject = id, bManaged = true, bCondensedView = false, stName = "Software Planning" };

						ttypePlanning.id = Convert.ToInt64(db.InsertWithIdentity(ttypePlanning));

						#region - categories - 

						var categScope = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR1", stName = "Scope", stDescription = "" };
						categScope.id = Convert.ToInt64(db.InsertWithIdentity(categScope));
						InsertDocumentationFlows(db, ttypePlanning.id, categScope.id);

						var categProducts = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR2", stName = "Products", stDescription = "" };
						categProducts.id = Convert.ToInt64(db.InsertWithIdentity(categProducts));
						InsertDocumentationFlows(db, ttypePlanning.id, categProducts.id);

						var categLifecycle = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR3", stName = "Lifecycle", stDescription = "" };
						categLifecycle.id = Convert.ToInt64(db.InsertWithIdentity(categLifecycle));
						InsertDocumentationFlows(db, ttypePlanning.id, categLifecycle.id);

						var categEstimate = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR4", stName = "Cost estimation", stDescription = "" };
						categEstimate.id = Convert.ToInt64(db.InsertWithIdentity(categEstimate));
						InsertDocumentationFlows(db, ttypePlanning.id, categEstimate.id);

						var categSchedule = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR5", stName = "Schedule", stDescription = "" };
						categSchedule.id = Convert.ToInt64(db.InsertWithIdentity(categSchedule));
						InsertDocumentationFlows(db, ttypePlanning.id, categSchedule.id);

						var categRisk = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR6", stName = "Risks", stDescription = "" };
						categRisk.id = Convert.ToInt64(db.InsertWithIdentity(categRisk));
						InsertDocumentationFlows(db, ttypePlanning.id, categRisk.id);

						var categResources = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR7", stName = "Resources", stDescription = "" };
						categResources.id = Convert.ToInt64(db.InsertWithIdentity(categResources));
						InsertDocumentationFlows(db, ttypePlanning.id, categResources.id);

						var categEnvironment = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR8", stName = "Project Environment", stDescription = "" };
						categEnvironment.id = Convert.ToInt64(db.InsertWithIdentity(categEnvironment));
						InsertDocumentationFlows(db, ttypePlanning.id, categEnvironment.id);

						var categData = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR9", stName = "Project Data", stDescription = "" };
						categData.id = Convert.ToInt64(db.InsertWithIdentity(categData));
						InsertDocumentationFlows(db, ttypePlanning.id, categData.id);

						var categExePlan = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR10", stName = "Execution Plan", stDescription = "" };
						categExePlan.id = Convert.ToInt64(db.InsertWithIdentity(categExePlan));
						InsertDocumentationFlows(db, ttypePlanning.id, categExePlan.id);

						var categMilestonePlan = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR11", stName = "Milestones", stDescription = "" };
						categMilestonePlan.id = Convert.ToInt64(db.InsertWithIdentity(categMilestonePlan));
						InsertDocumentationFlows(db, ttypePlanning.id, categMilestonePlan.id);

						var categCommitment = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR12", stName = "Commitments", stDescription = "" };
						categCommitment.id = Convert.ToInt64(db.InsertWithIdentity(categCommitment));
						InsertDocumentationFlows(db, ttypePlanning.id, categCommitment.id);

						var categProgress = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR13", stName = "Project Progress", stDescription = "" };
						categProgress.id = Convert.ToInt64(db.InsertWithIdentity(categProgress));
						InsertDocumentationFlows(db, ttypePlanning.id, categProgress.id);

						var categComm = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR14", stName = "Communication", stDescription = "" };
						categComm.id = Convert.ToInt64(db.InsertWithIdentity(categComm));
						InsertDocumentationFlows(db, ttypePlanning.id, categComm.id);

						var categRev = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR15", stName = "Revisions", stDescription = "" };
						categRev.id = Convert.ToInt64(db.InsertWithIdentity(categRev));
						InsertDocumentationFlows(db, ttypePlanning.id, categRev.id);

						var categIssues = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR16", stName = "Issues", stDescription = "" };
						categIssues.id = Convert.ToInt64(db.InsertWithIdentity(categIssues));
						InsertDocumentationFlows(db, ttypePlanning.id, categIssues.id);

						var categCorrections = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR17", stName = "Corrections", stDescription = "" };
						categCorrections.id = Convert.ToInt64(db.InsertWithIdentity(categCorrections));
						InsertDocumentationFlows(db, ttypePlanning.id, categCorrections.id);

						#endregion

					}

					{
						var ttypeRequirements = new TaskType { fkProject = id, bManaged = true, bCondensedView = false, stName = "Software Requirements" };

						ttypeRequirements.id = Convert.ToInt64(db.InsertWithIdentity(ttypeRequirements));

						#region - categories - 

						var categKnowledge = new TaskCategory { fkTaskType = ttypeRequirements.id, stAbreviation = "REQ1", stName = "Gather Knowledge", stDescription = "" };
						categKnowledge.id = Convert.ToInt64(db.InsertWithIdentity(categKnowledge));
						InsertDocumentationFlows(db, ttypeRequirements.id, categKnowledge.id);

						var categEvaluation = new TaskCategory { fkTaskType = ttypeRequirements.id, stAbreviation = "REQ2", stName = "Evaluation", stDescription = "" };
						categEvaluation.id = Convert.ToInt64(db.InsertWithIdentity(categEvaluation));
						InsertDocumentationFlows(db, ttypeRequirements.id, categEvaluation.id);

						var categTracking = new TaskCategory { fkTaskType = ttypeRequirements.id, stAbreviation = "REQ3", stName = "Tracking", stDescription = "" };
						categTracking.id = Convert.ToInt64(db.InsertWithIdentity(categTracking));
						InsertDocumentationFlows(db, ttypeRequirements.id, categTracking.id);

						var categRevisions = new TaskCategory { fkTaskType = ttypeRequirements.id, stAbreviation = "REQ4", stName = "Revisions", stDescription = "" };
						categRevisions.id = Convert.ToInt64(db.InsertWithIdentity(categRevisions));
						InsertDocumentationFlows(db, ttypeRequirements.id, categRevisions.id);

						var categChanges = new TaskCategory { fkTaskType = ttypeRequirements.id, stAbreviation = "REQ5", stName = "Change Requirements", stDescription = "" };
						categChanges.id = Convert.ToInt64(db.InsertWithIdentity(categChanges));
						InsertDocumentationFlows(db, ttypeRequirements.id, categChanges.id);

						#endregion
					}

					{
						var ttype = new TaskType { fkProject = id, bManaged = true, bCondensedView = true, stName = "Software Analysis" };

						ttype.id = Convert.ToInt64(db.InsertWithIdentity(ttype));

						{
							var categ = new TaskCategory { fkTaskType = ttype.id, stAbreviation = "", stName = "Design Docs", stDescription = "" };
							categ.id = Convert.ToInt64(db.InsertWithIdentity(categ));

							var t = 0;

							db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Open" });
							db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Re-Open" });
							db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = null, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Construction" });
							db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = null, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Peer Review" });
							db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = null, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Approval" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = null, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Done" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = null, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Cancelled" });
						}

						{
							var categ = new TaskCategory { fkTaskType = ttype.id, stAbreviation = "", stName = "Change Requirement", stDescription = "" };
							categ.id = Convert.ToInt64(db.InsertWithIdentity(categ));

							var t = 0;

							db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Open" });
							db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Re-Open" });
							db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = null, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Construction" });
							db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = null, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Peer Review" });
							db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = null, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Approval" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = null, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Done" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = null, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Cancelled" });
						}

						{
							var categ = new TaskCategory { fkTaskType = ttype.id, stAbreviation = "", stName = "Task Estimation", stDescription = "" };
							categ.id = Convert.ToInt64(db.InsertWithIdentity(categ));

							var t = 0;

							db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Open" });
							db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Re-Open" });
							db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = null, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Estimating" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = null, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Done" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = null, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Cancelled" });
						}
					}

					{
						var ttype = new TaskType { fkProject = id, bManaged = true, bCondensedView = true, stName = "Software Development" };

						ttype.id = Convert.ToInt64(db.InsertWithIdentity(ttype));

						{
							var categ = new TaskCategory { fkTaskType = ttype.id, stAbreviation = "", stName = "Resource Build", stDescription = "" };
							categ.id = Convert.ToInt64(db.InsertWithIdentity(categ));

							var t = 0;

							db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Open" });
							db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Re-Open" });
							db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = null, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Development" });
							db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = null, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Homologation" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = null, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Done" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = null, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Cancelled" });
						}
					}

					{
						var ttype = new TaskType { fkProject = id, bManaged = true, bCondensedView = true, stName = "Software Bugs" };

						ttype.id = Convert.ToInt64(db.InsertWithIdentity(ttype));

						{
							var categ = new TaskCategory { fkTaskType = ttype.id, stAbreviation = "", stName = "Construction Bugs", stDescription = "" };
							categ.id = Convert.ToInt64(db.InsertWithIdentity(categ));

							var t = 0;

							db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Open" });
							db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Re-Open" });
							db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = null, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Development" });
							db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = null, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Validation" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = null, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Done" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = null, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Cancelled" });
						}

						{
							var categ = new TaskCategory { fkTaskType = ttype.id, stAbreviation = "", stName = "Homologation Bugs", stDescription = "" };
							categ.id = Convert.ToInt64(db.InsertWithIdentity(categ));

							var t = 0;

							db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Open" });
							db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Re-Open" });
							db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = null, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Development" });
							db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = null, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Validation" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = null, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Done" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = null, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Cancelled" });
						}

						{
							var categ = new TaskCategory { fkTaskType = ttype.id, stAbreviation = "", stName = "Production Bugs", stDescription = "" };
							categ.id = Convert.ToInt64(db.InsertWithIdentity(categ));

							var t = 0;

							db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Open" });
							db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Re-Open" });
							db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = null, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Development" });
							db.Insert(new TaskFlow { bForceComplete = null, bForceOpen = null, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Validation" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = null, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Done" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = null, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Cancelled" });
						}
					}

					break;
			}

			return true;
		}

		public void InsertDocumentationFlows(DevKitDB db, long _fktype, long _fkcateg)
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
