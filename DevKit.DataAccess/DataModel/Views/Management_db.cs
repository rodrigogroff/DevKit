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

		public List<ManagementDTO_KPAType> kpaTypes = new List<ManagementDTO_KPAType>();
		public List<ManagementDTO_CondensedType> cTypes = new List<ManagementDTO_CondensedType>();
		public List<ManagementDTO_Version> versions = new List<ManagementDTO_Version>();
	}

	public class ManagementDTO_KPAType
	{
		public string id;
		public string stName;
		public List<ManagementDTO_KPATypeCategory> categories = new List<ManagementDTO_KPATypeCategory>();
	}

	public class ManagementDTO_KPATypeCategory
	{
		public string stName;
		public string stAbbrev;
		public bool ok = false;
		public string sFlow;
	}

	public class ManagementDTO_CondensedType
	{
		public string stName;		
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
						var ctype = new ManagementDTO_CondensedType()
						{
							stName = type.stName
						};

						dto.cTypes.Add(ctype);
					}
					else
					{
						var mType = new ManagementDTO_KPAType()
						{
							id = type.id.ToString(),
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
							};

							var lstTasks = (from e in db.Tasks
											where e.bComplete == true
											where e.fkTaskCategory == category.id
											select e).
											ToList();

							if (lstTasks.Count() > 0)
							{
								tc.ok = lstTasks.All(y => y.bComplete == true);
								tc.sFlow = db.TaskFlow(lstTasks.LastOrDefault().fkTaskFlowCurrent).stName;
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
