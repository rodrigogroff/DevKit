
namespace DataModel
{
	public partial class TaskTypeAccumulator
	{
		public TaskTypeAccumulator LoadAssociations(DevKitDB db)
		{
			sfkFlow = db.TaskFlow(fkTaskFlow).stName;

			return this;
		}		
	}
}
