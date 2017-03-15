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
}
