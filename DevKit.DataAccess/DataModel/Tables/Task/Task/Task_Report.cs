using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class Task
	{
		public List<Task> Report(DevKitDB db, List<Task> results, bool precached)
        {
            if (precached)
            {
                var lstIdsTaskCategory = results.Select(y => y.fkTaskCategory).Distinct().ToList();
                var lstIdsTaskType = results.Select(y => y.fkTaskType).Distinct().ToList();
                var lstIdsProject = results.Select(y => y.fkProject).Distinct().ToList();
                var lstIdsPhase = results.Select(y => y.fkPhase).Distinct().ToList();
                var lstIdsSprint = results.Select(y => y.fkSprint).Distinct().ToList();
                var lstIdsTaskFlowCurrent = results.Select(y => y.fkTaskFlowCurrent).Distinct().ToList();
                var lstIdVersion = results.Select(y => y.fkVersion).Distinct().ToList();
                var lstIdUserResponsible = results.Select(y => y.fkUserResponsible).Distinct().ToList();

                if (lstIdsTaskCategory.Any())
                {
                    var lstTaskCategory = (from e in db.TaskCategory where lstIdsTaskCategory.Contains(e.id) select e).ToList();
                    foreach (var item in lstTaskCategory) db.Cache["TaskCategory" + item.id] = item;
                }

                if (lstIdsTaskType.Any())
                {
                    var lstTaskType = (from e in db.TaskType where lstIdsTaskType.Contains(e.id) select e).ToList();
                    foreach (var item in lstTaskType) db.Cache["TaskType" + item.id] = item;
                }

                if (lstIdsProject.Any())
                {
                    var lstProject = (from e in db.Project where lstIdsProject.Contains(e.id) select e).ToList();
                    foreach (var item in lstProject) db.Cache["Project" + item.id] = item;
                }

                if (lstIdsPhase.Any())
                {
                    var lstPhase = (from e in db.ProjectPhase where lstIdsPhase.Contains(e.id) select e).ToList();
                    foreach (var item in lstPhase) db.Cache["ProjectPhase" + item.id] = item;
                }

                if (lstIdsSprint.Any())
                {
                    var lstSprint = (from e in db.ProjectSprint where lstIdsSprint.Contains(e.id) select e).ToList();
                    foreach (var item in lstSprint) db.Cache["ProjectSprint" + item.id] = item;
                }

                if (lstIdsTaskFlowCurrent.Any())
                {
                    var lstTaskFlowCurrent = (from e in db.TaskFlow where lstIdsTaskFlowCurrent.Contains(e.id) select e).ToList();
                    foreach (var item in lstTaskFlowCurrent) db.Cache["TaskFlow" + item.id] = item;
                }

                if (lstIdVersion.Any())
                {
                    var lstVersion = (from e in db.ProjectSprintVersion where lstIdVersion.Contains(e.id) select e).ToList();
                    foreach (var item in lstVersion) db.Cache["ProjectSprintVersion" + item.id] = item;
                }

                if (lstIdUserResponsible.Any())
                {
                    var lstUserResponsible = (from e in db.User where lstIdUserResponsible.Contains(e.id) select e).ToList();
                    foreach (var item in lstUserResponsible) db.Cache["User" + item.id] = item;
                }
            }
            
            results.ForEach(y => { y = y.LoadAssociations(db); });

            return results;
        }
    }
}
