using LinqToDB;
using System.Linq;
using Newtonsoft.Json;

namespace DataModel
{
	public partial class ProjectSprint
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

				case "newVersion":
					{
						var ent = JsonConvert.DeserializeObject<ProjectSprintVersion>(anexedEntity.ToString());

						if (ent.id == 0)
							if ((from ne in db.ProjectSprintVersions
								 where ne.stName == ent.stName && ne.fkSprint == id
								 select ne).
								 Any())
						{
							resp = "Version already added to project!";
							return false;
						}

						ent.fkSprint = id;
						
						if (ent.id == 0)
							db.Insert(ent);
						else
							db.Update(ent);

						versions = LoadVersions(db);
						break;
					}

				case "removeVersion":
					{
						var versionDel = JsonConvert.DeserializeObject<ProjectSprintVersion>(anexedEntity.ToString());

						if ((from e in db.Tasks where e.fkVersion == id select e).Any())
						{
							resp = "This version is being used in a task";
							return false;
						}

						db.Delete(versionDel);
						versions = LoadVersions(db);
						break;
					}
			}

			return true;
		}
	}
}
