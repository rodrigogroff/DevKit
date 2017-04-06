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
		public long id;

		public bool ok = false;

		public string stName,
						stAbbrev,
						stDescription,
						sFlow;
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
		public string	stPhase,
						stSprint,
						stVersion,
						stVersionState;

		public int? analysis, 
					development,
					bugs, 
					construction,
					homologation,
					production;

		public string	analysisH,
						developmentH,
						bugsH,
						constructionH,
						homologationH,
						productionH,
						totBugsH,
						workPct, 
						reworkPct;
	}

	public class Management
	{
		public ManagementDTO ComposedFilters(DevKitDB db, ManagementFilter filter, List<long?> lstUserProjects)
		{
			var dto = new ManagementDTO();
			var vs = new EnumVersionState();

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

				long	fkTaskType_SoftwareAnalysis = 0,
						fkTaskType_SoftwareDevelopment = 0,
						fkTaskType_SoftwareBugs = 0;

				foreach (var type in lstTypes)
				{
					if (type.bCondensedView == true)
					{
						if (type.stName.ToUpper() == "SOFTWARE ANALYSIS")
						{
							fkTaskType_SoftwareAnalysis = type.id;

							#region - code - 

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

							#endregion
						}
						else if (type.stName.ToUpper() == "SOFTWARE DEVELOPMENT")
						{
							fkTaskType_SoftwareDevelopment = type.id;

							#region - code - 

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

							#endregion
						}
						else if (type.stName.ToUpper() == "SOFTWARE BUGS")
						{
							fkTaskType_SoftwareBugs = type.id;

							#region - code - 

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

							#endregion
						}
					}
					else
					{
						#region - kpas -

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

								var last = lstTasks.LastOrDefault();

								tc.id = last.id;
								tc.sFlow = "State: " + db.TaskFlow(last.fkTaskFlowCurrent).stName;
							}
							else
							{
								tc.ok = false;
								tc.sFlow = "// No tasks created at this time!";
							}							

							mType.categories.Add(tc);
						}

						dto.kpaTypes.Add(mType);

						#endregion
					}
				}

				#region - versions (stats) - 

				var fkConstructionBug = (from e in db.TaskCategories
										 where e.fkTaskType == fkTaskType_SoftwareBugs
										 where e.stName == "Construction Bugs"
										 select e.id).
										 FirstOrDefault();

				var fkHomologationBug = (from e in db.TaskCategories
										 where e.fkTaskType == fkTaskType_SoftwareBugs
										 where e.stName == "Homologation Bugs"
										 select e.id).
										 FirstOrDefault();

				var fkProductionBug = (from e in db.TaskCategories
										 where e.fkTaskType == fkTaskType_SoftwareBugs
										 where e.stName == "Production Bugs"
										 select e.id).
										 FirstOrDefault();

				var lstPhases = (from e in db.ProjectPhases
								 where e.fkProject == filter.fkProject
								 select e).
								 OrderBy(y => y.id).
								 ToList();

				var lstValidVersions = new List<long>();

				foreach (var phase in lstPhases)
				{
					var lstSprints = (from e in db.ProjectSprints
									  where e.fkProject == filter.fkProject && e.fkPhase == phase.id
									  select e).
									  OrderBy(y => y.id).
									  ToList();

					foreach (var sprint in lstSprints)
					{
						var lstVersions = (from e in db.ProjectSprintVersions
										   where e.fkSprint == sprint.id
										   select e).
										   OrderBy(y => y.id).
										   ToList();

						foreach (var version in lstVersions)
						{
							if (version.fkVersionState == EnumVersionState.Closed)
								continue;

							lstValidVersions.Add(version.id);
						}
					}
				}

				long fkAcc_A = (from e in db.TaskTypeAccumulators
								where e.fkTaskType == fkTaskType_SoftwareAnalysis
								where e.stName == "Design Construction Hours"
								select e.id).
								FirstOrDefault();

				long fkAcc_D = (from e in db.TaskTypeAccumulators
								where e.fkTaskType == fkTaskType_SoftwareDevelopment
								where e.stName == "Coding Hours"
								select e.id).
								FirstOrDefault();

				long fkAcc_ED = (from e in db.TaskTypeAccumulators
								 where e.fkTaskType == fkTaskType_SoftwareDevelopment
								 where e.stName == "Estimate Coding Hours"
								 select e.id).
								 FirstOrDefault();

				long fkAcc_B_Construction = (from e in db.TaskTypeAccumulators
											 where e.fkTaskType == fkTaskType_SoftwareBugs
											 where e.fkTaskCategory == fkConstructionBug
											 where e.stName == "Coding Hours"
											 select e.id).
											 FirstOrDefault();

				long fkAcc_B_Homologation = (from e in db.TaskTypeAccumulators
											 where e.fkTaskType == fkTaskType_SoftwareBugs
											 where e.fkTaskCategory == fkHomologationBug
											 where e.stName == "Coding Hours"
											 select e.id).
											 FirstOrDefault();

				long fkAcc_B_Production = (from e in db.TaskTypeAccumulators
										   where e.fkTaskType == fkTaskType_SoftwareBugs
										   where e.fkTaskCategory == fkProductionBug
										   where e.stName == "Coding Hours"
										   select e.id).
										   FirstOrDefault();

				var lstValidTasks = (from e in db.Tasks where lstValidVersions.Contains((long)e.fkVersion) select e).ToList();
				
				foreach (var ver in lstValidVersions)
				{
					var version = db.ProjectSprintVersion(ver);
					var sprint = db.ProjectSprint(version.fkSprint);
					var phase = db.ProjectPhase(sprint.fkPhase);
					
					// -- ANALYSIS ----------------------------------

					var task_A_IdList = ( from e in lstValidTasks
										  where e.fkPhase == phase.id
										  where e.fkSprint == sprint.id
										  where e.fkVersion == version.id
										  where e.fkTaskType == fkTaskType_SoftwareAnalysis
										  select e.id).
										  ToList();

					int? _analysis = task_A_IdList.Count();

					long? _analysis_HH = (	from e in db.TaskAccumulatorValues
											where task_A_IdList.Contains((long)e.fkTask)
											where e.fkTaskAcc == fkAcc_A
											select e).Sum(y => y.nuHourValue);

					long? _analysis_MM = (  from e in db.TaskAccumulatorValues
											where task_A_IdList.Contains((long)e.fkTask)
											where e.fkTaskAcc == fkAcc_A
											select e).Sum(y => y.nuMinValue);

					if (_analysis_MM > 59)
					{
						long hours = (long)_analysis_MM / 60;
						_analysis_HH += hours;
						_analysis_MM -= hours * 60;
					}

					string _analysis_H = "A ";

					if (_analysis_HH != null)
						_analysis_H += _analysis_HH.ToString() + ":";
							
					if (_analysis_MM != null)
						_analysis_H += _analysis_MM.ToString();
					else 
						if (_analysis_HH != null)
							_analysis_H += "00";
														
					// DEVELOPMENT ------------------------------------

					var task_D_IdList = (from e in lstValidTasks
											where e.fkPhase == phase.id
											where e.fkSprint == sprint.id
											where e.fkVersion == version.id
											where e.fkTaskType == fkTaskType_SoftwareDevelopment
											select e.id).ToList();

					int ? _development = task_D_IdList.Count();

					long? _development_HH = (from e in db.TaskAccumulatorValues
											where task_D_IdList.Contains((long)e.fkTask)
											where e.fkTaskAcc == fkAcc_D
											select e).Sum(y => y.nuHourValue);

					if (_development_HH == null) _development_HH = 0;

					long? _development_MM = (from e in db.TaskAccumulatorValues
											where task_A_IdList.Contains((long)e.fkTask)
											where e.fkTaskAcc == fkAcc_D
											select e).Sum(y => y.nuMinValue);

					if (_development_MM == null) _development_MM = 0;

					long? _development_EHH = (from e in db.TaskAccumulatorValues
												where task_D_IdList.Contains((long)e.fkTask)
												where e.fkTaskAcc == fkAcc_ED
												select e).Sum(y => y.nuHourValue);

					if (_development_EHH == null) _development_EHH = 0;

					long? _development_EMM = (from e in db.TaskAccumulatorValues
												where task_A_IdList.Contains((long)e.fkTask)
												where e.fkTaskAcc == fkAcc_ED
												select e).Sum(y => y.nuMinValue);

					if (_development_EMM == null) _development_EMM = 0;

					string _workPct = "";

					if ((_development_HH > 0 || _development_MM > 0) && (_development_EHH > 0 || _development_EMM > 0 ))
					{
						long tot_Est = (long)_development_EHH * 60 + (long)_development_EMM;
						long tot_W = (long)_development_HH * 60 + (long)_development_MM;

						_workPct = (tot_W * 100 / tot_Est).ToString() + "%";
					}

					if (_development_MM > 59)
					{
						long hours = (long)_development_MM / 60;
						_development_HH += hours;
						_development_MM -= hours * 60;
					}

					string _development_H = "E " + _development_EHH + ":" + _development_EMM.ToString().PadLeft(2,'0') + 
											" / D " + _development_HH + ":" + _development_MM.ToString().PadLeft(2, '0');

					// BUGS ------------------------------------

					long _tot_bug_HH = 0, _tot_bug_MM = 0;

							

					var task_B_IdList = (from e in lstValidTasks
										 where e.fkPhase == phase.id
										 where e.fkSprint == sprint.id
										 where e.fkVersion == version.id
										 where e.fkTaskType == fkTaskType_SoftwareBugs
										 select e.id).
										 ToList();

					int ? _bugs = task_B_IdList.Count();

					// ---------- construction hours ------------------

					int? _construction = (	from e in lstValidTasks
											where e.fkPhase == phase.id
											where e.fkSprint == sprint.id
											where e.fkVersion == version.id
											where e.fkTaskType == fkTaskType_SoftwareBugs
											where e.fkTaskCategory == fkConstructionBug
											select e).Count();

					long? _bug_construction_HH = (from e in db.TaskAccumulatorValues
											where task_B_IdList.Contains((long)e.fkTask)
											where e.fkTaskAcc == fkAcc_B_Construction
													select e).Sum(y => y.nuHourValue);

					long? _bug_construction_MM = (	from e in db.TaskAccumulatorValues
													where task_B_IdList.Contains((long)e.fkTask)
													where e.fkTaskAcc == fkAcc_B_Construction
													select e).Sum(y => y.nuMinValue);

					if (_bug_construction_MM > 59)
					{
						long hours = (long)_bug_construction_MM / 60;
						_bug_construction_HH += hours;
						_bug_construction_MM -= hours * 60;
					}

					string _constructionH = "B ";

					if (_bug_construction_HH != null)
					{
						_tot_bug_HH += (long)_bug_construction_HH;
						_constructionH += _bug_construction_HH.ToString() + ":";
					}

					if (_bug_construction_MM != null)
					{
						_tot_bug_MM += (long)_bug_construction_MM;
						_constructionH += _bug_construction_MM.ToString().PadLeft(2, '0');
					}								
					else
						if (_bug_construction_HH != null)
						_constructionH += "00";

					// --------------

					int? _homologation = (	from e in lstValidTasks
											where e.fkPhase == phase.id
											where e.fkSprint == sprint.id
											where e.fkVersion == version.id
											where e.fkTaskType == fkTaskType_SoftwareBugs
											where e.fkTaskCategory == fkHomologationBug
											select e).Count();

					long? _bug_homologation_HH = (from e in db.TaskAccumulatorValues
													where task_B_IdList.Contains((long)e.fkTask)
													where e.fkTaskAcc == fkAcc_B_Homologation
													select e).Sum(y => y.nuHourValue);

					long? _bug_homologation_MM = (from e in db.TaskAccumulatorValues
													where task_B_IdList.Contains((long)e.fkTask)
													where e.fkTaskAcc == fkAcc_B_Homologation
													select e).Sum(y => y.nuMinValue);

					if (_bug_homologation_MM > 59)
					{
						long hours = (long)_bug_homologation_MM / 60;
						_bug_homologation_HH += hours;
						_bug_homologation_MM -= hours * 60;
					}

					string _homologationH = "B ";

					if (_bug_homologation_HH != null)
					{
						_tot_bug_HH += (long)_bug_homologation_HH;
						_homologationH += _bug_homologation_HH.ToString() + ":";
					}								

					if (_bug_homologation_MM != null)
					{
						_tot_bug_MM += (long)_bug_homologation_MM;
						_homologationH += _bug_homologation_MM.ToString().PadLeft(2, '0');
					}								
					else
						if (_bug_homologation_HH != null)
						_homologationH += "00";

					// ---------------------------

					int? _production = (	from e in lstValidTasks
											where e.fkPhase == phase.id
											where e.fkSprint == sprint.id
											where e.fkVersion == version.id
											where e.fkTaskType == fkTaskType_SoftwareBugs
											where e.fkTaskCategory == fkProductionBug
											select e).Count();

					long? _bug_production_HH = (from e in db.TaskAccumulatorValues
													where task_B_IdList.Contains((long)e.fkTask)
													where e.fkTaskAcc == fkAcc_B_Production
													select e).Sum(y => y.nuHourValue);

					long? _bug_production_MM = (from e in db.TaskAccumulatorValues
													where task_B_IdList.Contains((long)e.fkTask)
													where e.fkTaskAcc == fkAcc_B_Production
													select e).Sum(y => y.nuMinValue);

					if (_bug_production_MM > 59)
					{
						long hours = (long)_bug_production_MM / 60;
						_bug_production_HH += hours;
						_bug_production_MM -= hours * 60;
					}

					string _productionH = "B ";

					if (_bug_production_HH != null)
					{
						_tot_bug_HH += (long)_bug_production_HH;
						_productionH += _bug_production_HH.ToString() + ":";
					}								

					if (_bug_production_MM != null)
					{
						_tot_bug_MM += (long)_bug_production_MM;
						_productionH += _bug_production_MM.ToString().PadLeft(2, '0');
					}								
					else
						if (_bug_production_HH != null)
							_productionH += "00";

					// ---- end 

					if (_tot_bug_MM > 59)
					{
						long hours = (long)_tot_bug_MM / 60;
						_tot_bug_HH += hours;
						_tot_bug_MM -= hours * 60;
					}

					var _totBugsH = "B " + _tot_bug_HH + ":" + _tot_bug_MM;

					string _reworkPct = "";

					if ((_development_HH > 0 || _development_MM > 0) && (_development_EHH > 0 || _development_EMM > 0))
					{
						long tot_Est = _tot_bug_HH * 60 + _tot_bug_MM;
						long tot_W = (long)_development_HH * 60 + (long)_development_MM;

						_reworkPct = (tot_Est * 100 / tot_W).ToString() + "%";
					}

					if (_tot_bug_HH > 0 && _tot_bug_MM > 0)
					{
						long tot_Est = _tot_bug_HH * 60 + _tot_bug_MM;

						// construction
						if (_bug_construction_HH == null) _bug_construction_HH = 0;
						if (_bug_construction_MM == null) _bug_construction_MM = 0;
								
						if (_bug_construction_HH > 0 || _bug_construction_MM > 0)
						{
							long tot_W = (long)_bug_construction_HH * 60 + (long)_bug_construction_MM;

							_constructionH += " (" + (tot_W * 100 / tot_Est).ToString() + "%)";
						}

						// homologation
						if (_bug_homologation_HH == null) _bug_homologation_HH = 0;
						if (_bug_homologation_MM == null) _bug_homologation_MM = 0;

						if (_bug_homologation_HH > 0 || _bug_homologation_MM > 0)
						{
							long tot_W = (long)_bug_homologation_HH * 60 + (long)_bug_homologation_MM;

							_homologationH += " (" + (tot_W * 100 / tot_Est).ToString() + "%)";
						}

						// production
						if (_bug_production_HH == null) _bug_production_HH = 0;
						if (_bug_production_MM == null) _bug_production_MM = 0;

						if (_bug_production_HH > 0 || _bug_production_MM > 0)
						{
							long tot_W = (long)_bug_production_HH * 60 + (long)_bug_production_MM;

							_productionH += " (" + (tot_W * 100 / tot_Est).ToString() + "%)";
						}
					}

					if (_analysis == 0) _analysis = null;
					if (_development == 0) _development = null;
					if (_bugs == 0) _bugs = null;
					if (_construction == 0) _construction = null;
					if (_homologation == 0) _homologation = null;
					if (_production == 0) _production = null;

					dto.versions.Add(new ManagementDTO_Version
					{
						stPhase = phase.stName,
						stSprint = sprint.stName,
						stVersion = version.stName,
						stVersionState = vs.Get((long)version.fkVersionState).stName,
						analysis = _analysis,
						analysisH = _analysis_H,
						development = _development,
						developmentH = _development_H,
						workPct = _workPct,
						reworkPct = _reworkPct,
						bugs = _bugs,
						construction = _construction,
						constructionH = _constructionH,
						homologation = _homologation,
						homologationH = _homologationH,
						production = _production,
						productionH = _productionH,
						totBugsH = _totBugsH
					});
				}

				#endregion
			}

			return dto;
		}
	}
}
