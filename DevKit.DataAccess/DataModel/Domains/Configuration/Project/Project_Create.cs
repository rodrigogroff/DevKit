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
				case EnumProjectTemplate.SoftwareMaintenance: strType = "Software"; break;
			}

			switch (template)
			{
				case EnumProjectTemplate.Custom:
					
					break;

				case EnumProjectTemplate.SoftwareMaintenance:

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
