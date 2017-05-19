using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public class UserKanbanViewFilter
	{
		public string busca;
		public bool? complete;

		public long? nuPriority,
						fkProject,
						fkPhase,
						fkSprint,
						fkUserStart,
						fkUserAssigned,
						fkTaskType,
						fkTaskFlowCurrent,
						fkTaskCategory;
	}

	public class UserKanban_dto
	{
		public bool fail = false;

		public List<KanbanProject> projects = new List<KanbanProject>();
	}

	public class KanbanProject
	{
		public string project;
		public long project_id;

		public List<KanbanSprint> sprints = new List<KanbanSprint>();
	}

	public class KanbanSprint
	{
		public string sprint;
		public long sprint_id;

		public List<KanbanTaskType> tasktypes = new List<KanbanTaskType>();
	}

	public class KanbanTaskType
	{
		public string tasktype;
		public long tasktype_id;

		public List<KanbanTaskCategory> categories = new List<KanbanTaskCategory>();
	}

	public class KanbanTaskAcc
	{
		public string stName,
					  sValue;
	}

	public class KanbanTaskCategory
	{
		public string category;
		public long category_id;

		public List<KanbanTaskFlow> flows = new List<KanbanTaskFlow>();
		public List<KanbanTaskAcc> accs = new List<KanbanTaskAcc>();
	}

	public class KanbanTaskFlow
	{
		public string flow;
		public long flow_id;

		public List<Task> tasks = new List<Task>();		
	}

	public class UserKanbanView
	{
		public UserKanban_dto ComposedFilters(DevKitDB db, UserKanbanViewFilter filter)
		{
			var dto = new UserKanban_dto();
			var task_helper = new Task();
			var accTypeEnum = new EnumAccumulatorType();
			var user = db.GetCurrentUser();

			var dbUserprojects = ( from e in db.Projects
									join pu in db.ProjectUsers on e.id equals pu.fkProject
									where pu.fkUser == user.id
									where filter.fkProject == null || e.id == filter.fkProject
									select e ).
									ToList();

			if (dbUserprojects.Count() == 0)
				dto.fail = true;

			dto.projects = new List<KanbanProject>();

			var foundProject = false;

			foreach (var project in dbUserprojects)
			{
				var kb_proj = new KanbanProject()
				{
					project = project.stName,
					project_id = project.id
				};

				var lstUsertasks = (	from e in db.Tasks
										where e.fkProject == project.id										
										where filter.complete == null || e.bComplete == filter.complete
										where filter.nuPriority == null || e.nuPriority == filter.nuPriority
										where filter.fkPhase == null || e.fkPhase == filter.fkPhase
										where filter.fkSprint == null || e.fkSprint == filter.fkSprint
										where filter.fkUserStart == null || e.fkUserStart == filter.fkUserStart
										where filter.fkUserAssigned == null || e.fkUserResponsible == filter.fkUserAssigned
										where filter.fkTaskType == null || e.fkTaskType == filter.fkTaskType
										where filter.fkTaskFlowCurrent == null || e.fkTaskFlowCurrent == filter.fkTaskFlowCurrent
										where filter.fkTaskCategory == null || e.fkTaskCategory == filter.fkTaskCategory
										select e).
										ToList();


                var lstSprints = (from e in lstUsertasks
                                  join sp in db.ProjectSprints on e.fkSprint equals sp.id
                                  select sp).Distinct().
                  OrderBy(y => y.stName).
                  ToList();
                
                if (lstUsertasks.Count() > 0)
                {
                    if (lstSprints.Count == 0)
                        continue;

                    foundProject = true;
                }					
				else
					continue;
                                
                foreach (var sprint in lstSprints)
				{
					var ks = new KanbanSprint()
					{
						sprint = sprint.stName,
						sprint_id = sprint.id
					};

					var lstTaskType = (from e in lstUsertasks
									   join tt in db.TaskTypes on e.fkTaskType equals tt.id
									   select tt).Distinct().
									   OrderBy(y => y.stName).
									   ToList();

					foreach (var tasktype in lstTaskType)
					{
						var ktt = new KanbanTaskType()
						{
							tasktype = tasktype.stName,
							tasktype_id = tasktype.id
						};

						var lstCategories = (from e in lstUsertasks
											 join cat in db.TaskCategories on e.fkTaskCategory equals cat.id

											 where e.fkTaskType == tasktype.id
											 where e.fkSprint == sprint.id
											 where e.fkProject == project.id

											 select cat).Distinct().
											 OrderBy(y => y.stName).
											 ToList();

						foreach (var category in lstCategories)
						{
							var ktc = new KanbanTaskCategory()
							{
								category = category.stName,
								category_id = category.id
							};

							var flows = (from e in db.TaskFlows
										 where e.fkTaskType == tasktype.id
										 where e.fkTaskCategory == category.id
										 select e).
										 OrderBy( y=> y.nuOrder).
										 ToList();

							var currentTaskRang = new List<long>();

							foreach (var flow in flows)
							{
								var ktf = new KanbanTaskFlow()
								{
									flow = flow.stName,
									flow_id = flow.id
								};

								ktf.tasks = (from e in lstUsertasks
											 where e.fkTaskType == tasktype.id
											 where e.fkSprint == sprint.id
											 where e.fkProject == project.id
											 where e.fkTaskCategory == category.id
											 where e.fkTaskFlowCurrent == flow.id
											 select e).
											 OrderBy(y => y.id).
											 ToList();

								var count = ktf.tasks.Count();

								if (count > 0)
									ktf.flow += " (" + count + ")";

								currentTaskRang.AddRange(ktf.tasks.Select ( y=> y.id));

								ktc.flows.Add(ktf);
							}

							var accs = (from e in db.TaskTypeAccumulators
										where e.fkTaskType == tasktype.id
										where e.fkTaskCategory == category.id
										select e).
										ToList();

							foreach (var i_acc in accs)
							{
								ktc.accs.Add(new KanbanTaskAcc()
								{
									stName = i_acc.stName,
									sValue = task_helper.GetValueForType (db, accTypeEnum.GetName(i_acc.fkTaskAccType), 0, i_acc.id, 0, currentTaskRang ),
								});
							}

							ktt.categories.Add(ktc);
						}

						ks.tasktypes.Add(ktt);
					}
					
					kb_proj.sprints.Add(ks);
				}
				
				dto.projects.Add(kb_proj);
			}

			dto.fail = !foundProject;

			return dto;
		}
	}
}
