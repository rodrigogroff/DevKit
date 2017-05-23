
namespace DataModel
{
	public partial class Task
	{
		public Task LoadAssociations(DevKitDB db, bool IsListing = false)
		{
			var setup = db.Setup();

			sdtStart = dtStart?.ToString(setup.stDateFormat);
			snuPriority = new EnumPriority().Get((long)nuPriority).stName;
			sfkUserStart = db.User(fkUserStart)?.stLogin;

			sfkTaskCategory = db.TaskCategory(fkTaskCategory)?.stName;
			sfkTaskType = db.TaskType(fkTaskType)?.stName;
			sfkProject = db.Project(fkProject)?.stName;
			sfkPhase = db.ProjectPhase(fkPhase)?.stName;
			sfkSprint = db.ProjectSprint(fkSprint)?.stName;
			sfkTaskFlowCurrent = db.TaskFlow(fkTaskFlowCurrent)?.stName;
			sfkVersion = db.ProjectSprintVersion(fkVersion)?.stName;
			sfkUserResponsible = db.User(fkUserResponsible)?.stLogin;

			if (!IsListing)
			{
				usrProgress = LoadProgress(db);
				usrMessages = LoadMessages(db);
				flows = LoadFlows(db);
				accs = LoadAccs(db);				
				dependencies = LoadDependencies(db);
				checkpoints = LoadCheckpoints(db);
				questions = LoadQuestions(db);
				clients = LoadClients(db);
				clientGroups = LoadClientGroups(db);
                customSteps = LoadCustomSteps(db);

                logs = LoadLogs(db);
			}

			return this;
		}
	}
}
