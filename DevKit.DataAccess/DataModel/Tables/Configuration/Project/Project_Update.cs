using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace DataModel
{
	// --------------------------
	// functions
	// --------------------------

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

						if ((from ne in db.ProjectPhases
							 where ne.stName.ToUpper() == ent.stName.ToUpper() && ne.fkProject == id
							 select ne).Any())
						{
							resp = "Phase already added to project!";
							return false;
						}

						ent.fkProject = id;
						
						db.Insert(ent);
						phases = LoadPhases(db);
						break;
					}
										
				case "removePhase":
					{
						var phaseDel = JsonConvert.DeserializeObject<ProjectPhase>(anexedEntity.ToString());

						if ((from e in db.Tasks where e.fkPhase == phaseDel.id select e).Any())
						{
							resp = "This phase is being used in a task";
							return false;
						}
						
						db.Delete(phaseDel);
						phases = LoadPhases(db);
						break;
					}
			}

			return true;
		}
	}
}
