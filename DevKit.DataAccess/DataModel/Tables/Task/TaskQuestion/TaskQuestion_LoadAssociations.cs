
namespace DataModel
{
	public partial class TaskQuestion
	{
		public TaskQuestion LoadAssociations(DevKitDB db)
		{
			var setup = db.Setup();

			sfkUserOpen = db.User(this.fkUserOpen).stLogin;
			stProtocol = db.Task(this.fkTask).stProtocol;

			return this;
		}
	}
}
