
namespace DataModel
{
	public partial class Task
	{
		public Task LoadAssociations(DevKitDB db, bool IsListing = false)
		{
			var setup = db.GetSetup();

			sdtStart = dtStart?.ToString(setup.stDateFormat);
			snuPriority = new EnumPriority().Get((long)nuPriority).stName;
			sfkUserStart = db.GetUser(fkUserStart)?.stLogin;

			sfkTaskCategory = db.GetTaskCategory(fkTaskCategory)?.stName;
			sfkTaskType = db.GetTaskType(fkTaskType)?.stName;
			sfkProject = db.GetProject(fkProject)?.stName;
			sfkPhase = db.GetProjectPhase(fkPhase)?.stName;
			sfkSprint = db.GetProjectSprint(fkSprint)?.stName;
			sfkTaskFlowCurrent = db.GetTaskFlow(fkTaskFlowCurrent)?.stName;
			sfkVersion = db.GetProjectSprintVersion(fkVersion)?.stName;
			sfkUserResponsible = db.GetUser(fkUserResponsible)?.stLogin;

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
