
namespace DataModel
{
	public partial class TaskQuestion
	{
		public TaskQuestion LoadAssociations(DevKitDB db)
		{
			var setup = db.GetSetup();

			sfkUserOpen = db.GetUser(this.fkUserOpen).stLogin;
			stProtocol = db.GetTask(this.fkTask).stProtocol;

			return this;
		}
	}
}
