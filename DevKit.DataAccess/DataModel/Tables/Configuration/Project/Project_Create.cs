using LinqToDB;
using System;
using System.Linq;

namespace DataModel
{
	// --------------------------
	// functions
	// --------------------------

	public partial class Project
	{
		bool CheckDuplicate(Project item, DevKitDB db)
		{
			var query = from e in db.Project select e;

			if (item.stName != null)
			{
				var _st = item.stName.ToUpper();
				query = from e in query where e.stName.ToUpper().Contains(_st) select e;
			}

			if (item.id > 0)
				query = from e in query where e.id != item.id select e;

			return query.Any();
		}
		
		public bool Create(DevKitDB db, ref string resp)
		{
			var user = db.currentUser;

			if (CheckDuplicate(this, db))
			{
				resp = "Project name already taken";
				return false;
			}

			var setup = db.GetSetup();
			
			dtCreation = DateTime.Now;
			fkUser = user.id;
			
			id = Convert.ToInt64(db.InsertWithIdentity(this));

			db.Insert ( new ProjectUser { dtJoin = DateTime.Now, fkProject = id, fkUser = fkUser, stRole = "Creator" } );

			var enum_projectTemplate = new EnumProjectTemplate();

			var template = (long)fkProjectTemplate;
			var strType = "";

			switch (template)
			{
				case EnumProjectTemplate.Custom: strType = "Custom"; break;
				case EnumProjectTemplate.CMMI2: strType = "CMMI2"; break;
				case EnumProjectTemplate.SoftwareMaintenance: strType = "Software Maintenance"; break;
			}

			switch (template)
			{
				case EnumProjectTemplate.Custom:
					strType = "Custom";
					break;

				case EnumProjectTemplate.CMMI2: 
				case EnumProjectTemplate.SoftwareMaintenance:

					if (template == EnumProjectTemplate.CMMI2)
					{
						var ttypePlanning = new TaskType { fkProject = id, bManaged = true, bCondensedView = false, bKPA= true, stName = "KPA - Software Planning" };

						ttypePlanning.id = Convert.ToInt64(db.InsertWithIdentity(ttypePlanning));

						#region - categories - 

						var categScope = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR01", stName = "Scope", stDescription = "Scope, non-scope, restrictions, objectives and all produts to be made" };
						categScope.id = Convert.ToInt64(db.InsertWithIdentity(categScope));
						InsertDocumentationFlows(db, ttypePlanning, categScope, setup);
						
						var categProducts = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR02", stName = "Products", stDescription = "All products dimensioned properly" };
						categProducts.id = Convert.ToInt64(db.InsertWithIdentity(categProducts));
						InsertDocumentationFlows(db, ttypePlanning, categProducts, setup);

						var categLifecycle = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR03", stName = "Lifecycle", stDescription = "Your team workflow and project phases" };
						categLifecycle.id = Convert.ToInt64(db.InsertWithIdentity(categLifecycle));
						InsertDocumentationFlows(db, ttypePlanning, categLifecycle, setup);

						var categEstimate = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR04", stName = "Cost estimation", stDescription = "Your project (and phases) cost with technical references" };
						categEstimate.id = Convert.ToInt64(db.InsertWithIdentity(categEstimate));
						InsertDocumentationFlows(db, ttypePlanning, categEstimate, setup);

						var categSchedule = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR05", stName = "Schedule", stDescription = "The activities and dates for your project" };
						categSchedule.id = Convert.ToInt64(db.InsertWithIdentity(categSchedule));
						InsertDocumentationFlows(db, ttypePlanning, categSchedule, setup);

						var categRisk = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR06", stName = "Risks", stDescription = "The risks of the project, and their impact, probability and resolve" };
						categRisk.id = Convert.ToInt64(db.InsertWithIdentity(categRisk));
						InsertDocumentationFlows(db, ttypePlanning, categRisk, setup);

						var categResources = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR07", stName = "Resources", stDescription = "Human resources, resumees, training, skill maps" };
						categResources.id = Convert.ToInt64(db.InsertWithIdentity(categResources));
						InsertDocumentationFlows(db, ttypePlanning, categResources, setup);

						var categEnvironment = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR08", stName = "Project Environment", stDescription = "All the information about the project setup" };
						categEnvironment.id = Convert.ToInt64(db.InsertWithIdentity(categEnvironment));
						InsertDocumentationFlows(db, ttypePlanning, categEnvironment, setup);

						var categData = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR09", stName = "Project Data", stDescription = "Reports, meetings and confidential data" };
						categData.id = Convert.ToInt64(db.InsertWithIdentity(categData));
						InsertDocumentationFlows(db, ttypePlanning, categData, setup);

						var categExePlan = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR10", stName = "Execution Plan", stDescription = "Your project strategy to deliver products" };
						categExePlan.id = Convert.ToInt64(db.InsertWithIdentity(categExePlan));
						InsertDocumentationFlows(db, ttypePlanning, categExePlan, setup);

						var categMilestonePlan = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR11", stName = "Milestones", stDescription = "Feasibility of your team to accomplish work" };
						categMilestonePlan.id = Convert.ToInt64(db.InsertWithIdentity(categMilestonePlan));
						InsertDocumentationFlows(db, ttypePlanning, categMilestonePlan, setup);

						var categCommitment = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR12", stName = "Commitments", stDescription = "Project plan approval by interested parties" };
						categCommitment.id = Convert.ToInt64(db.InsertWithIdentity(categCommitment));
						InsertDocumentationFlows(db, ttypePlanning, categCommitment, setup);

						var categProgress = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR13", stName = "Project Progress", stDescription = "Constant monitoring of predicted vs performed" };
						categProgress.id = Convert.ToInt64(db.InsertWithIdentity(categProgress));
						InsertDocumentationFlows(db, ttypePlanning, categProgress, setup);

						var categComm = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR14", stName = "Communication", stDescription = "Disseminating deadlines, costs, resources, requirements" };
						categComm.id = Convert.ToInt64(db.InsertWithIdentity(categComm));
						InsertDocumentationFlows(db, ttypePlanning, categComm, setup);

						var categRev = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR15", stName = "Revisions", stDescription = "Upon milestones, the team is reviewed" };
						categRev.id = Convert.ToInt64(db.InsertWithIdentity(categRev));
						InsertDocumentationFlows(db, ttypePlanning, categRev, setup);

						var categIssues = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR16", stName = "Issues", stDescription = "Problem records and lessons learned" };
						categIssues.id = Convert.ToInt64(db.InsertWithIdentity(categIssues));
						InsertDocumentationFlows(db, ttypePlanning, categIssues, setup);

						var categCorrections = new TaskCategory { fkTaskType = ttypePlanning.id, stAbreviation = "GPR17", stName = "Corrections", stDescription = "Actions and the monitoring of project problems" };
						categCorrections.id = Convert.ToInt64(db.InsertWithIdentity(categCorrections));
						InsertDocumentationFlows(db, ttypePlanning, categCorrections, setup);

						#endregion
					}

					if (template == EnumProjectTemplate.CMMI2)
					{
						var ttypeRequirements = new TaskType { fkProject = id, bManaged = true, bCondensedView = false, bKPA = true, stName = "KPA - Software Requirements" };

						ttypeRequirements.id = Convert.ToInt64(db.InsertWithIdentity(ttypeRequirements));

						#region - categories - 

						var categKnowledge = new TaskCategory { fkTaskType = ttypeRequirements.id, stAbreviation = "REQ1", stName = "Gather Knowledge", stDescription = "Obtain all requirements from product owners" };
						categKnowledge.id = Convert.ToInt64(db.InsertWithIdentity(categKnowledge));
						InsertDocumentationFlows(db, ttypeRequirements, categKnowledge, setup);

						var categEvaluation = new TaskCategory { fkTaskType = ttypeRequirements.id, stAbreviation = "REQ2", stName = "Evaluation", stDescription = "Requirements are understoood technically and estimated" };
						categEvaluation.id = Convert.ToInt64(db.InsertWithIdentity(categEvaluation));
						InsertDocumentationFlows(db, ttypeRequirements, categEvaluation, setup);

						var categTracking = new TaskCategory { fkTaskType = ttypeRequirements.id, stAbreviation = "REQ3", stName = "Tracking", stDescription = "A bidirectional map of requirements vs delivered products" };
						categTracking.id = Convert.ToInt64(db.InsertWithIdentity(categTracking));
						InsertDocumentationFlows(db, ttypeRequirements, categTracking, setup);

						var categRevisions = new TaskCategory { fkTaskType = ttypeRequirements.id, stAbreviation = "REQ4", stName = "Revisions", stDescription = "Plans and products are reviewed as the products are delivered" };
						categRevisions.id = Convert.ToInt64(db.InsertWithIdentity(categRevisions));
						InsertDocumentationFlows(db, ttypeRequirements, categRevisions, setup);

						var categChanges = new TaskCategory { fkTaskType = ttypeRequirements.id, stAbreviation = "REQ5", stName = "Change Requirements", stDescription = "Follow ups from scope and requirements change" };
						categChanges.id = Convert.ToInt64(db.InsertWithIdentity(categChanges));
						InsertDocumentationFlows(db, ttypeRequirements, categChanges, setup);

						#endregion
					}
					
					{
						var ttype = new TaskType { fkProject = id, bManaged = true, bCondensedView = true, bKPA = false, stName = "Software Analysis" };

						#region - categories - 

						ttype.id = Convert.ToInt64(db.InsertWithIdentity(ttype));

						{
							var categ = new TaskCategory { fkTaskType = ttype.id, stAbreviation = "", stName = "Design Docs", stDescription = "" };
							categ.id = Convert.ToInt64(db.InsertWithIdentity(categ));

							var t = 0;

							db.Insert(new TaskFlow { bForceComplete = false, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Open" });
							db.Insert(new TaskFlow { bForceComplete = false, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Re-Open" });

							{
								var flow_id = Convert.ToInt64(db.InsertWithIdentity(new TaskFlow { bForceComplete = false, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Construction" }));

								db.Insert(new TaskTypeAccumulator
								{
									fkTaskAccType = EnumAccumulatorType.Hours,
									fkTaskCategory = categ.id,
									fkTaskFlow = flow_id,
									fkTaskType = ttype.id,
									stName = "Design Construction Hours",
									bEstimate = false
								});
							}

							{
								var flow_id = Convert.ToInt64(db.InsertWithIdentity(new TaskFlow { bForceComplete = false, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Peer Review" }));

								db.Insert(new TaskTypeAccumulator
								{
									fkTaskAccType = EnumAccumulatorType.Hours,
									fkTaskCategory = categ.id,
									fkTaskFlow = flow_id,
									fkTaskType = ttype.id,
									stName = "Design Peer Review Hours",
									bEstimate = false
								});
							}

							db.Insert(new TaskFlow { bForceComplete = false, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Approval" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Done" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Cancelled" });
						}

						{
							var categ = new TaskCategory { fkTaskType = ttype.id, stAbreviation = "", stName = "Change Requirement", stDescription = "" };
							categ.id = Convert.ToInt64(db.InsertWithIdentity(categ));

							var t = 0;

							db.Insert(new TaskFlow { bForceComplete = false, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Open" });
							db.Insert(new TaskFlow { bForceComplete = false, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Re-Open" });

							{
								var flow_id = Convert.ToInt64(db.InsertWithIdentity(new TaskFlow { bForceComplete = false, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Construction" }));

								db.Insert(new TaskTypeAccumulator
								{
									fkTaskAccType = EnumAccumulatorType.Hours,
									fkTaskCategory = categ.id,
									fkTaskFlow = flow_id,
									fkTaskType = ttype.id,
									stName = "Change Analysis Hours",
									bEstimate = false
								});
							}

							{
								var flow_id = Convert.ToInt64(db.InsertWithIdentity(new TaskFlow { bForceComplete = false, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Peer Review" }));

								db.Insert(new TaskTypeAccumulator
								{
									fkTaskAccType = EnumAccumulatorType.Hours,
									fkTaskCategory = categ.id,
									fkTaskFlow = flow_id,
									fkTaskType = ttype.id,
									stName = "Change Peer Review Hours",
									bEstimate = false
								});
							}

							db.Insert(new TaskFlow { bForceComplete = false, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Approval" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Done" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Cancelled" });
						}

						{
							var categ = new TaskCategory { fkTaskType = ttype.id, stAbreviation = "", stName = "Task Estimation", stDescription = "" };
							categ.id = Convert.ToInt64(db.InsertWithIdentity(categ));

							var t = 0;

							db.Insert(new TaskFlow { bForceComplete = false, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Open" });
							db.Insert(new TaskFlow { bForceComplete = false, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Re-Open" });

							{
								var flow_id = Convert.ToInt64(db.InsertWithIdentity(new TaskFlow { bForceComplete = false, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Estimating" }));

								db.Insert(new TaskTypeAccumulator
								{
									fkTaskAccType = EnumAccumulatorType.Hours,
									fkTaskCategory = categ.id,
									fkTaskFlow = flow_id,
									fkTaskType = ttype.id,
									stName = "Estimating Hours",
									bEstimate = false
								});
							}

							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Done" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Cancelled" });
						}

						#endregion
					}

					{
						var ttype = new TaskType { fkProject = id, bManaged = true, bCondensedView = true, bKPA = false, stName = "Software Development" };

						#region  - categories -

						ttype.id = Convert.ToInt64(db.InsertWithIdentity(ttype));

						{
							var categ = new TaskCategory { fkTaskType = ttype.id, stAbreviation = "", stName = "Resource Build", stDescription = "" };
							categ.id = Convert.ToInt64(db.InsertWithIdentity(categ));

							var t = 0;

							db.Insert(new TaskFlow { bForceComplete = false, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Open" });
							db.Insert(new TaskFlow { bForceComplete = false, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Re-Open" });

							{
								var flow_id = Convert.ToInt64(db.InsertWithIdentity(new TaskFlow { bForceComplete = false, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Development" }));
								
								db.Insert(new TaskTypeAccumulator
								{
									fkTaskAccType = EnumAccumulatorType.Hours,
									fkTaskCategory = categ.id,
									fkTaskFlow = flow_id,
									fkTaskType = ttype.id,
									stName = "Estimate Coding Hours",
									bEstimate = true,
								});

								db.Insert(new TaskTypeAccumulator
								{
									fkTaskAccType = EnumAccumulatorType.Hours,
									fkTaskCategory = categ.id,
									fkTaskFlow = flow_id,
									fkTaskType = ttype.id,
									stName = "Coding Hours",
									bEstimate = false,
								});
							}

							db.Insert(new TaskFlow { bForceComplete = false, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Testing" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Done" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Cancelled" });
						}

						{
							var categ = new TaskCategory { fkTaskType = ttype.id, stAbreviation = "", stName = "Resource Refactory", stDescription = "" };
							categ.id = Convert.ToInt64(db.InsertWithIdentity(categ));

							var t = 0;

							db.Insert(new TaskFlow { bForceComplete = false, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Open" });
							db.Insert(new TaskFlow { bForceComplete = false, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Re-Open" });

							{
								var flow_id = Convert.ToInt64(db.InsertWithIdentity(new TaskFlow { bForceComplete = false, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Development" }));

								db.Insert(new TaskTypeAccumulator
								{
									fkTaskAccType = EnumAccumulatorType.Hours,
									fkTaskCategory = categ.id,
									fkTaskFlow = flow_id,
									fkTaskType = ttype.id,
									stName = "Estimate Coding Hours",
									bEstimate = true,
								});

								db.Insert(new TaskTypeAccumulator
								{
									fkTaskAccType = EnumAccumulatorType.Hours,
									fkTaskCategory = categ.id,
									fkTaskFlow = flow_id,
									fkTaskType = ttype.id,
									stName = "Coding Hours",
									bEstimate = false,
								});
							}

							db.Insert(new TaskFlow { bForceComplete = false, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Testing" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Done" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Cancelled" });
						}

						#endregion
					}

					{
						var ttype = new TaskType { fkProject = id, bManaged = true, bCondensedView = true, bKPA = false, stName = "Software Bugs" };

						#region - categories - 

						ttype.id = Convert.ToInt64(db.InsertWithIdentity(ttype));

						{
							var categ = new TaskCategory { fkTaskType = ttype.id, stAbreviation = "", stName = "Construction Bugs", stDescription = "" };
							categ.id = Convert.ToInt64(db.InsertWithIdentity(categ));

							var t = 0;

							db.Insert(new TaskFlow { bForceComplete = false, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Open" });
							db.Insert(new TaskFlow { bForceComplete = false, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Re-Open" });

							{
								var flow_id = Convert.ToInt64(db.InsertWithIdentity(new TaskFlow { bForceComplete = false, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Development" }));

								db.Insert(new TaskTypeAccumulator
								{
									fkTaskAccType = EnumAccumulatorType.Hours,
									fkTaskCategory = categ.id,
									fkTaskFlow = flow_id,
									fkTaskType = ttype.id,
									stName = "Coding Hours",
									bEstimate = false,
								});
							}

							db.Insert(new TaskFlow { bForceComplete = false, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Testing" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Done" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Cancelled" });
						}

						{
							var categ = new TaskCategory { fkTaskType = ttype.id, stAbreviation = "", stName = "Homologation Bugs", stDescription = "" };
							categ.id = Convert.ToInt64(db.InsertWithIdentity(categ));

							var t = 0;

							db.Insert(new TaskFlow { bForceComplete = false, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Open" });
							db.Insert(new TaskFlow { bForceComplete = false, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Re-Open" });

							{
								var flow_id = Convert.ToInt64(db.InsertWithIdentity(new TaskFlow { bForceComplete = false, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Development" }));

								db.Insert(new TaskTypeAccumulator
								{
									fkTaskAccType = EnumAccumulatorType.Hours,
									fkTaskCategory = categ.id,
									fkTaskFlow = flow_id,
									fkTaskType = ttype.id,
									stName = "Coding Hours",
									bEstimate = false,
								});
							}

							db.Insert(new TaskFlow { bForceComplete = false, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Testing" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Done" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Cancelled" });
						}

						{
							var categ = new TaskCategory { fkTaskType = ttype.id, stAbreviation = "", stName = "Production Bugs", stDescription = "" };
							categ.id = Convert.ToInt64(db.InsertWithIdentity(categ));

							var t = 0;

							db.Insert(new TaskFlow { bForceComplete = false, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Open" });
							db.Insert(new TaskFlow { bForceComplete = false, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Re-Open" });

							{
								var flow_id = Convert.ToInt64(db.InsertWithIdentity(new TaskFlow { bForceComplete = false, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Development" }));

								db.Insert(new TaskTypeAccumulator
								{
									fkTaskAccType = EnumAccumulatorType.Hours,
									fkTaskCategory = categ.id,
									fkTaskFlow = flow_id,
									fkTaskType = ttype.id,
									stName = "Coding Hours",
									bEstimate = false,
								});
							}

							db.Insert(new TaskFlow { bForceComplete = false, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Validation" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Done" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Cancelled" });
						}

						#endregion
					}

					{
						var ttype = new TaskType { fkProject = id, bManaged = false, bCondensedView = false, bKPA = false, stName = "Team Meetings" };

						#region - categories - 

						ttype.id = Convert.ToInt64(db.InsertWithIdentity(ttype));

						{
							var categ = new TaskCategory { fkTaskType = ttype.id, stAbreviation = "", stName = "Everyone", stDescription = "" };
							categ.id = Convert.ToInt64(db.InsertWithIdentity(categ));

							var t = 0;

							db.Insert(new TaskFlow { bForceComplete = false, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Planned" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Done" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Cancelled" });
						}

						{
							var categ = new TaskCategory { fkTaskType = ttype.id, stAbreviation = "", stName = "Analysts", stDescription = "" };
							categ.id = Convert.ToInt64(db.InsertWithIdentity(categ));

							var t = 0;

							db.Insert(new TaskFlow { bForceComplete = false, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Planned" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Done" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Cancelled" });
						}

						{
							var categ = new TaskCategory { fkTaskType = ttype.id, stAbreviation = "", stName = "Developers", stDescription = "" };
							categ.id = Convert.ToInt64(db.InsertWithIdentity(categ));

							var t = 0;

							db.Insert(new TaskFlow { bForceComplete = false, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Planned" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Done" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Cancelled" });
						}

						{
							var categ = new TaskCategory { fkTaskType = ttype.id, stAbreviation = "", stName = "Client and Analysts", stDescription = "" };
							categ.id = Convert.ToInt64(db.InsertWithIdentity(categ));

							var t = 0;

							db.Insert(new TaskFlow { bForceComplete = false, bForceOpen = true, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Planned" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Done" });
							db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = false, fkTaskType = ttype.id, fkTaskCategory = categ.id, nuOrder = t++, stName = "Cancelled" });
						}

						#endregion
					}
					
					break;
			}

			new AuditLog {
				fkUser = user.id,
				fkActionLog = EnumAuditAction.ProjectCreation,
				nuType = EnumAuditType.Project,
				fkTarget = this.id
			}.
			Create(db, "type: " + strType, "");

			return true;
		}

		public void InsertDocumentationFlows(DevKitDB db, TaskType _fktype, TaskCategory _fkcateg, Setup setup)
		{
			var idFlowOpen = Convert.ToInt64(db.InsertWithIdentity(new TaskFlow { bForceComplete = false, bForceOpen = true, fkTaskType = _fktype.id, fkTaskCategory = _fkcateg.id, nuOrder = 1, stName = "Open" }));
			db.Insert(new TaskFlow { bForceComplete = false, bForceOpen = true, fkTaskType = _fktype.id, fkTaskCategory = _fkcateg.id, nuOrder = 2, stName = "Revision" });
			db.Insert(new TaskFlow { bForceComplete = false, bForceOpen = false, fkTaskType = _fktype.id, fkTaskCategory = _fkcateg.id, nuOrder = 3, stName = "Development" });
			db.Insert(new TaskFlow { bForceComplete = false, bForceOpen = false, fkTaskType = _fktype.id, fkTaskCategory = _fkcateg.id, nuOrder = 4, stName = "Peer Review" });
			db.Insert(new TaskFlow { bForceComplete = true, bForceOpen = false, fkTaskType = _fktype.id, fkTaskCategory = _fkcateg.id, nuOrder = 5, stName = "Done" });

			db.Insert( new Task
			{
				fkProject = id,
				bComplete = false,
				stTitle = "KPA: " + _fktype.stName,
				stLocalization = _fkcateg.stName,
				stDescription = "",
				fkTaskType = _fktype.id,
				fkTaskCategory = _fkcateg.id,
				fkUserStart = this.fkUser,
				fkUserResponsible = null,
				fkTaskFlowCurrent = idFlowOpen,
				dtStart = DateTime.Now,
				dtLastEdit = DateTime.Now,
				nuPriority = EnumPriority.Normal,				
				stProtocol = setup.GetProtocol()
			});
		}
	}
}
