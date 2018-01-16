
namespace DataModel
{
	public partial class Task
	{
		public Task LoadAssociations(DevKitDB db, loaderOptionsTask options )
		{
			var setup = db.GetSetup();

			sdtStart = dtStart?.ToString(setup.stDateFormat);
			snuPriority = new EnumPriority().Get((long)nuPriority).stName;

            if (options.bLoadUsers)
			    sfkUserStart = db.GetUser(fkUserStart)?.stLogin;

            if (options.bLoadTaskCategory)
                sfkTaskCategory = db.GetTaskCategory(fkTaskCategory)?.stName;

            if (options.bLoadTaskType)
                sfkTaskType = db.GetTaskType(fkTaskType)?.stName;

            if (options.bLoadProject)
                sfkProject = db.GetProject(fkProject)?.stName;

            if (options.bLoadPhase)
			    sfkPhase = db.GetProjectPhase(fkPhase)?.stName;

            if (options.bLoadSprint)
			    sfkSprint = db.GetProjectSprint(fkSprint)?.stName;

            if (options.bLoadTaskFlow)
			    sfkTaskFlowCurrent = db.GetTaskFlow(fkTaskFlowCurrent)?.stName;

            if (options.bLoadVersion)
			    sfkVersion = db.GetProjectSprintVersion(fkVersion)?.stName;

            if (options.bLoadUsers)
			    sfkUserResponsible = db.GetUser(fkUserResponsible)?.stLogin;
            
            if (options.bLoadProgress)
	            usrProgress = LoadProgress(db);

            if (options.bLoadMessages)
	            usrMessages = LoadMessages(db);

            if (options.bLoadFlows)
                flows = LoadFlows(db);

            if (options.bLoadAccs)
	            accs = LoadAccs(db);				

            if (options.bLoadDependencies)
	            dependencies = LoadDependencies(db);

            if (options.bLoadCheckpoints)
	            checkpoints = LoadCheckpoints(db);

            if (options.bLoadQuestions)
	            questions = LoadQuestions(db);

            if (options.bLoadCustomSteps)
                customSteps = LoadCustomSteps(db);

            if (options.bLoadLogs)
                logs = LoadLogs(db);			

			return this;
		}
	}
}
