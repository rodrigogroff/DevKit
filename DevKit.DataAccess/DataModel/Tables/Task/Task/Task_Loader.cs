using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
    public class loaderOptionsTask
    {
        public bool bLoadTaskCategory = false,
                    bLoadTaskType = false,
                    bLoadProject = false,
                    bLoadPhase = false,
                    bLoadSprint = false,
                    bLoadTaskFlow = false,
                    bLoadVersion = false,
                    bLoadUsers = false,
                    bLoadProgress = false,
                    bLoadMessages = false,
                    bLoadFlows = false,
                    bLoadAccs = false,
                    bLoadDependencies = false,
                    bLoadCheckpoints = false,
                    bLoadQuestions = false,
                    bLoadClients = false,
                    bLoadClientGroups = false,
                    bLoadCustomSteps = false,
                    bLoadLogs = false;
    }

    public partial class Task
	{
		public List<Task> Loader(DevKitDB db, List<Task> results, loaderOptionsTask options )
        {
            if (options.bLoadTaskCategory)
            {
                var lstIdsTaskCategory = results.Select(y => y.fkTaskCategory).Distinct().ToList();

                if (lstIdsTaskCategory.Any())
                {
                    var lstTaskCategory = (from e in db.TaskCategory where lstIdsTaskCategory.Contains(e.id) select e).ToList();
                    foreach (var item in lstTaskCategory) db.Cache["TaskCategory" + item.id] = item;
                }
            }
            
            if (options.bLoadTaskType)
            {
                var lstIdsTaskType = results.Select(y => y.fkTaskType).Distinct().ToList();

                if (lstIdsTaskType.Any())
                {
                    var lstTaskType = (from e in db.TaskType where lstIdsTaskType.Contains(e.id) select e).ToList();
                    foreach (var item in lstTaskType) db.Cache["TaskType" + item.id] = item;
                }
            }

            if (options.bLoadProject)
            {
                var lstIdsProject = results.Select(y => y.fkProject).Distinct().ToList();

                if (lstIdsProject.Any())
                {
                    var lstProject = (from e in db.Project where lstIdsProject.Contains(e.id) select e).ToList();
                    foreach (var item in lstProject) db.Cache["Project" + item.id] = item;
                }
            }
            
            if (options.bLoadPhase)
            {
                var lstIdsPhase = results.Select(y => y.fkPhase).Distinct().ToList();

                if (lstIdsPhase.Any())
                {
                    var lstPhase = (from e in db.ProjectPhase where lstIdsPhase.Contains(e.id) select e).ToList();
                    foreach (var item in lstPhase) db.Cache["ProjectPhase" + item.id] = item;
                }
            }
            
            if (options.bLoadSprint)
            {
                var lstIdsSprint = results.Select(y => y.fkSprint).Distinct().ToList();

                if (lstIdsSprint.Any())
                {
                    var lstSprint = (from e in db.ProjectSprint where lstIdsSprint.Contains(e.id) select e).ToList();
                    foreach (var item in lstSprint) db.Cache["ProjectSprint" + item.id] = item;
                }
            }

            if (options.bLoadTaskFlow)
            {
                var lstIdsTaskFlowCurrent = results.Select(y => y.fkTaskFlowCurrent).Distinct().ToList();

                if (lstIdsTaskFlowCurrent.Any())
                {
                    var lstTaskFlowCurrent = (from e in db.TaskFlow where lstIdsTaskFlowCurrent.Contains(e.id) select e).ToList();
                    foreach (var item in lstTaskFlowCurrent) db.Cache["TaskFlow" + item.id] = item;
                }
            }
            
            if (options.bLoadVersion)
            {
                var lstIdVersion = results.Select(y => y.fkVersion).Distinct().ToList();

                if (lstIdVersion.Any())
                {
                    var lstVersion = (from e in db.ProjectSprintVersion where lstIdVersion.Contains(e.id) select e).ToList();
                    foreach (var item in lstVersion) db.Cache["ProjectSprintVersion" + item.id] = item;
                }
            }
            
            if (options.bLoadUsers)
            {
                var lstIdUserResponsible = results.Select(y => y.fkUserResponsible).Distinct().ToList();

                if (lstIdUserResponsible.Any())
                {
                    var lstUserResponsible = (from e in db.User where lstIdUserResponsible.Contains(e.id) select e).ToList();
                    foreach (var item in lstUserResponsible) db.Cache["User" + item.id] = item;
                }
            }
           
            results.ForEach(y => { y = y.LoadAssociations(db, options); });

            return results;
        }
    }
}
