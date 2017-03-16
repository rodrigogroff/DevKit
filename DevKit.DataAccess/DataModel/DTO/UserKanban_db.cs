using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public class UserKanbanFilter
	{
		public int skip, take;
		public string busca;
	}

	public class UserKanban_dto
	{
		public List<KanbanProject> projects = new List<KanbanProject>();
	}

	public class KanbanProject
	{
		public Project project;
		public List<KanbanSprint> sprints = new List<KanbanSprint>();
	}

	public class KanbanSprint
	{
		public ProjectSprint sprint;
		public List<KanbanTaskType> tasktypes = new List<KanbanTaskType>();
	}

	public class KanbanTaskType
	{
		public TaskType tasktype;
		public List<KanbanTaskCategory> categories = new List<KanbanTaskCategory>();
	}

	public class KanbanTaskCategory
	{
		public TaskCategory category;
		public List<KanbanTaskFlow> flows = new List<KanbanTaskFlow>();
	}

	public class KanbanTaskFlow
	{
		public TaskFlow flow;
		public List<Task> tasks = new List<Task>();
	}

	public class UserKanban
	{
		public UserKanban_dto ComposedFilters(DevKitDB db, UserKanbanFilter filter, User user)
		{
			var dto = new UserKanban_dto();

			var dbUserprojects = (	from e in db.Projects
									join pu in db.ProjectUsers on e.id equals pu.fkProject
									where pu.fkUser == user.id
									select e).ToList();

			dto.projects = new List<KanbanProject>();

			foreach (var project in dbUserprojects)
			{
				var kb_proj = new KanbanProject()
				{
					project = project
				};

				var lstUsertasks = ( from e in db.Tasks
									where e.fkProject == project.id
									where e.fkUserResponsible == user.id
									select e).
									ToList();

				var lstSprints = (from e in lstUsertasks
								  join sp in db.ProjectSprints on e.fkSprint equals sp.id
								  select sp).Distinct().
								  OrderBy(y => y.stName).
								  ToList();
				
				foreach (var sp in lstSprints)
				{
					var ks = new KanbanSprint()
					{
						sprint = sp
					};

					var lstTaskType = (from e in lstUsertasks
									   join tt in db.TaskTypes on e.fkTaskType equals tt.id
									   select tt).Distinct().
									   OrderBy(y => y.stName).
									   ToList();

					foreach (var tt in lstTaskType)
					{
						var ktt = new KanbanTaskType()
						{
							tasktype = tt
						};

						var lstCategories = (from e in lstUsertasks
											 join cat in db.TaskCategories on e.fkTaskCategory equals cat.id

											 where e.fkTaskType == tt.id
											 where e.fkSprint == sp.id
											 where e.fkProject == project.id

											 select cat).Distinct().
											 OrderBy(y => y.stName).
											 ToList();

						foreach (var cat in lstCategories)
						{
							var ktc = new KanbanTaskCategory()
							{
								category = cat
							};

							var flows = (from e in db.TaskFlows where e.fkTaskType == tt.id select e).OrderBy( y=> y.nuOrder).ToList();

							foreach (var _flow in flows)
							{
								var ktf = new KanbanTaskFlow()
								{
									flow = _flow
								};

								ktf.tasks = (from e in lstUsertasks
											 where e.fkTaskType == tt.id
											 where e.fkSprint == sp.id
											 where e.fkProject == project.id
											 where e.fkTaskCategory == cat.id
											 where e.fkTaskFlowCurrent == _flow.id
											 select e).
											 OrderBy(y => y.id).
											 ToList();

								ktc.flows.Add(ktf);
							}

							ktt.categories.Add(ktc);
						}

						ks.tasktypes.Add(ktt);
					}
					
					kb_proj.sprints.Add(ks);
				}
				
				dto.projects.Add(kb_proj);
			}
				
			return dto;
		}
	}
}
