
namespace DataModel
{
	public partial class TaskTypeAccumulator
	{
		public TaskTypeAccumulator LoadAssociations(DevKitDB db)
		{
			sfkFlow = db.GetTaskFlow(fkTaskFlow)?.stName;

			return this;
		}		
	}
}
