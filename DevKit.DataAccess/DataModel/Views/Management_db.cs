using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public class ManagementFilter
	{
		public long? fkProject;
	}

	public class ManagementDTO
	{
		public bool fail = false;

		public ManagementDTO_CondensedType sa, sd, sb;
		public List<ManagementDTO_KPAType> kpaTypes = new List<ManagementDTO_KPAType>();
		public List<ManagementDTO_Version> versions = new List<ManagementDTO_Version>();
	}

	public class ManagementDTO_KPAType
	{
		public long id;
		public string stName;
		
		public List<ManagementDTO_KPATypeCategory> categories = new List<ManagementDTO_KPATypeCategory>();
	}

	public class ManagementDTO_KPATypeCategory
	{
		public string stName,
						stAbbrev,
						stDescription,
						sFlow;

		public bool ok = false;		
	}

	public class ManagementDTO_CondensedType
	{
		public string stName;
		public int? open,
					reopen,
					development,
					construction,
					peerreview,
					testing,
					approval,
					cancelled, 
					done,
					total;
	}

	public class ManagementDTO_Version
	{
		public string stName;
	}

	public class Management
	{
		public ManagementDTO ComposedFilters(DevKitDB db, ManagementFilter filter, List<long?> lstUserProjects)
		{
			var dto = new ManagementDTO();

			if (filter.fkProject == null)
			{
				dto.fail = true;
			}
			else if (!lstUserProjects.Contains(filter.fkProject))
			{
				dto.fail = true;
			}
			else
			{
				dto.fail = false;
				
				var lstTypes = (from e in db.TaskTypes
								where e.bManaged == true
								where e.fkProject == filter.fkProject
								select e).
								ToList();

				foreach (var type in lstTypes)
				{
					if (type.bCondensedView == true)
					{
						if (type.stName.ToUpper() == "SOFTWARE ANALYSIS")
						{
							dto.sa = new ManagementDTO_CondensedType
							{
								stName = type.stName,

								open = (from e in db.Tasks
										join eF in db.TaskFlows on e.fkTaskFlowCurrent equals eF.id
										where e.fkTaskFlowCurrent == eF.id && eF.stName.ToUpper() == "OPEN"
										where e.fkTaskType == type.id
										select e).Count(),

								reopen = (from e in db.Tasks
										  join eF in db.TaskFlows on e.fkTaskFlowCurrent equals eF.id
										  where e.fkTaskFlowCurrent == eF.id && eF.stName.ToUpper() == "RE-OPEN"
										  where e.fkTaskType == type.id
										  select e).Count(),

								construction = (from e in db.Tasks
											   join eF in db.TaskFlows on e.fkTaskFlowCurrent equals eF.id
											   where e.fkTaskFlowCurrent == eF.id && eF.stName.ToUpper() == "CONSTRUCTION"
											   where e.fkTaskType == type.id
											   select e).Count(),

								peerreview = (from e in db.Tasks
											join eF in db.TaskFlows on e.fkTaskFlowCurrent equals eF.id
											where e.fkTaskFlowCurrent == eF.id && eF.stName.ToUpper() == "PEER REVIEW"
											where e.fkTaskType == type.id
											select e).Count(),

								cancelled = (from e in db.Tasks
											 join eF in db.TaskFlows on e.fkTaskFlowCurrent equals eF.id
											 where e.fkTaskFlowCurrent == eF.id && eF.stName.ToUpper() == "CANCELLED"
											 where e.fkTaskType == type.id
											 where e.bComplete == false
											 select e).Count(),

								done = (from e in db.Tasks
										join eF in db.TaskFlows on e.fkTaskFlowCurrent equals eF.id
										where e.fkTaskFlowCurrent == eF.id && eF.stName.ToUpper() == "DONE"
										where e.fkTaskType == type.id
										where e.bComplete == false
										select e).Count()
							};

							dto.sa.total = dto.sa.open + dto.sa.reopen + dto.sa.construction + dto.sa.peerreview + dto.sa.cancelled + dto.sa.done;

							if (dto.sa.open == 0) dto.sa.open = null;
							if (dto.sa.reopen == 0) dto.sa.reopen = null;
							if (dto.sa.construction == 0) dto.sa.construction = null;
							if (dto.sa.peerreview == 0) dto.sa.peerreview = null;
							if (dto.sa.cancelled == 0) dto.sa.cancelled = null;
							if (dto.sa.done == 0) dto.sa.done = null;
						}
						else if (type.stName.ToUpper() == "SOFTWARE DEVELOPMENT")
						{
							dto.sd = new ManagementDTO_CondensedType
							{
								stName = type.stName,

								open = (from e in db.Tasks
										join eF in db.TaskFlows on e.fkTaskFlowCurrent equals eF.id
										where e.fkTaskFlowCurrent == eF.id && eF.stName.ToUpper() == "OPEN"
										where e.fkTaskType == type.id
										select e).Count(),

								reopen = (from e in db.Tasks
										  join eF in db.TaskFlows on e.fkTaskFlowCurrent equals eF.id
										  where e.fkTaskFlowCurrent == eF.id && eF.stName.ToUpper() == "RE-OPEN"
										  where e.fkTaskType == type.id
										  select e).Count(),

								development = (from e in db.Tasks
											   join eF in db.TaskFlows on e.fkTaskFlowCurrent equals eF.id
											   where e.fkTaskFlowCurrent == eF.id && eF.stName.ToUpper() == "DEVELOPMENT"
											   where e.fkTaskType == type.id
											   select e).Count(),

								testing = (from e in db.Tasks
											join eF in db.TaskFlows on e.fkTaskFlowCurrent equals eF.id
											where e.fkTaskFlowCurrent == eF.id && eF.stName.ToUpper() == "TESTING"
											where e.fkTaskType == type.id
											select e).Count(),

								cancelled = (from e in db.Tasks
											 join eF in db.TaskFlows on e.fkTaskFlowCurrent equals eF.id
											 where e.fkTaskFlowCurrent == eF.id && eF.stName.ToUpper() == "CANCELLED"
											 where e.fkTaskType == type.id
											 where e.bComplete == false
											 select e).Count(),

								done = (from e in db.Tasks
										join eF in db.TaskFlows on e.fkTaskFlowCurrent equals eF.id
										where e.fkTaskFlowCurrent == eF.id && eF.stName.ToUpper() == "DONE"
										where e.fkTaskType == type.id
										where e.bComplete == false
										select e).Count()
							};

							dto.sd.total = dto.sd.open + dto.sd.reopen + dto.sd.development + dto.sd.testing + dto.sd.cancelled + dto.sd.done;

							if (dto.sd.open == 0) dto.sd.open = null;
							if (dto.sd.reopen == 0) dto.sd.reopen = null;
							if (dto.sd.development == 0) dto.sd.development = null;
							if (dto.sd.testing == 0) dto.sd.testing = null;
							if (dto.sd.cancelled == 0) dto.sd.cancelled = null;
							if (dto.sd.done == 0) dto.sd.done = null;							
						}
						else if (type.stName.ToUpper() == "SOFTWARE BUGS")
						{
							dto.sb = new ManagementDTO_CondensedType
							{
								stName = type.stName,

								open = (from e in db.Tasks
										join eF in db.TaskFlows on e.fkTaskFlowCurrent equals eF.id
										where e.fkTaskFlowCurrent == eF.id && eF.stName.ToUpper() == "OPEN"
										where e.fkTaskType == type.id
										select e).Count(),

								reopen = (from e in db.Tasks
										  join eF in db.TaskFlows on e.fkTaskFlowCurrent equals eF.id
										  where e.fkTaskFlowCurrent == eF.id && eF.stName.ToUpper() == "RE-OPEN"
										  where e.fkTaskType == type.id
										  select e).Count(),

								development = (from e in db.Tasks
											   join eF in db.TaskFlows on e.fkTaskFlowCurrent equals eF.id
											   where e.fkTaskFlowCurrent == eF.id && eF.stName.ToUpper() == "DEVELOPMENT"
											   where e.fkTaskType == type.id
											   select e).Count(),

								testing = (from e in db.Tasks
										   join eF in db.TaskFlows on e.fkTaskFlowCurrent equals eF.id
										   where e.fkTaskFlowCurrent == eF.id && eF.stName.ToUpper() == "TESTING"
										   where e.fkTaskType == type.id
										   select e).Count(),

								cancelled = (from e in db.Tasks
											 join eF in db.TaskFlows on e.fkTaskFlowCurrent equals eF.id
											 where e.fkTaskFlowCurrent == eF.id && eF.stName.ToUpper() == "CANCELLED"
											 where e.fkTaskType == type.id
											 where e.bComplete == false
											 select e).Count(),

								done = (from e in db.Tasks
										join eF in db.TaskFlows on e.fkTaskFlowCurrent equals eF.id
										where e.fkTaskFlowCurrent == eF.id && eF.stName.ToUpper() == "DONE"
										where e.fkTaskType == type.id
										where e.bComplete == false
										select e).Count()
							};

							dto.sb.total = dto.sb.open + dto.sb.reopen + dto.sb.development + dto.sb.testing + dto.sb.cancelled + dto.sb.done;

							if (dto.sb.open == 0) dto.sb.open = null;
							if (dto.sb.reopen == 0) dto.sb.reopen = null;
							if (dto.sb.development == 0) dto.sb.development = null;
							if (dto.sb.testing == 0) dto.sb.testing = null;
							if (dto.sb.cancelled == 0) dto.sb.cancelled = null;
							if (dto.sb.done == 0) dto.sb.done = null;
						}
					}
					else
					{
						var mType = new ManagementDTO_KPAType
						{
							id = type.id,
							stName = type.stName							
						};

						var lstCategs = (from e in db.TaskCategories
										 where e.fkTaskType == type.id
										 select e).
										ToList();

						foreach (var category in lstCategs)
						{
							var tc = new ManagementDTO_KPATypeCategory
							{
								stName = category.stName,
								stAbbrev = category.stAbreviation,
								stDescription = category.stDescription
							};

							var lstTasks = (from e in db.Tasks
											where e.fkTaskCategory == category.id
											select e).
											ToList();

							if (lstTasks.Count() > 0)
							{
								tc.ok = lstTasks.All(y => y.bComplete == true);
								tc.sFlow = "State: " + db.TaskFlow(lstTasks.LastOrDefault().fkTaskFlowCurrent).stName;
							}
							else
							{
								tc.ok = false;
								tc.sFlow = "// No tasks created at this time!";
							}							

							mType.categories.Add(tc);
						}

						dto.kpaTypes.Add(mType);
					}					
				}
			}

			return dto;
		}
	}
}
