using System.Collections.Generic;

namespace DataModel
{
	public partial class TaskType
	{
		public List<TaskCategory> categories;
		public Project project;
		
		public string updateCommand = "";
		public object anexedEntity;
	}
}
