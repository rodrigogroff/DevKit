using LinqToDB;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace DataModel
{
	public partial class Project
	{
		public bool Update(DevKitDB db, ref string resp)
		{
			if (CheckDuplicate(this, db))
			{
				resp = "Project name already taken";
				return false;
			}

			switch (updateCommand)
			{
				case "entity":
					{
						db.Update(this);
						break;
					}

				case "newUser":
					{
						var ent = JsonConvert.DeserializeObject<ProjectUser>(anexedEntity.ToString());

						if (ent.id == 0)
						{
							if ((from ne in db.ProjectUsers
								 where ne.fkUser == ent.fkUser && ne.fkProject == id
								 select ne).Any())
							{
								resp = "User already added to project!";
								return false;
							}
						
							ent.fkProject = id;
							ent.dtJoin = DateTime.Now;

							db.Insert(ent);
						}							
						else
							db.Update(ent);

						users = LoadUsers(db);
						break;
					}

				case "removeUser":
					{
						db.Delete(JsonConvert.DeserializeObject<ProjectUser>(anexedEntity.ToString()));
						users = LoadUsers(db);
						break;
					}

				case "newPhase":
					{
						var ent = JsonConvert.DeserializeObject<ProjectPhase>(anexedEntity.ToString());

						if (ent.id == 0)
						{ 
							if ((from ne in db.ProjectPhases
								 where ne.stName.ToUpper() == ent.stName.ToUpper() && ne.fkProject == id
								 select ne).Any())
							{
								resp = "Phase already added to project!";
								return false;
							}
							
							ent.fkProject = id;

							db.Insert(ent);
						}							
						else
							db.Update(ent);

						phases = LoadPhases(db);
						break;
					}

				case "removePhase":
					{
						var mdlDel = JsonConvert.DeserializeObject<ProjectPhase>(anexedEntity.ToString());

						if ((from e in db.Tasks where e.fkPhase == mdlDel.id select e).Any())
						{
							resp = "This phase is being used in a task";
							return false;
						}

						db.Delete(mdlDel);
						phases = LoadPhases(db);
						break;
					}

				case "newSprint":
					{
						var ent = JsonConvert.DeserializeObject<ProjectSprint>(anexedEntity.ToString());

						if (ent.id == 0)
						{
							if ((from ne in db.ProjectSprints
								 where ne.fkPhase == id
								 where ne.stName.ToUpper() == ent.stName.ToUpper() && ne.fkProject == id
								 select ne).Any())
							{
								resp = "Sprint already added to project!";
								return false;
							}

							ent.fkProject = id;

							db.Insert(ent);
						}
						else
							db.Update(ent);

						sprints = LoadSprints(db);
						break;
					}

				case "removeSprint":
					{
						var mdlDel = JsonConvert.DeserializeObject<ProjectSprint>(anexedEntity.ToString());

						if ((from e in db.Tasks where e.fkSprint == mdlDel.id select e).Any())
						{
							resp = "This sprint is being used in a task";
							return false;
						}

						db.Delete(mdlDel);
						sprints = LoadSprints(db);
						break;
					}

			}

			return true;
		}
	}
}
