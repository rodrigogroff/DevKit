using LinqToDB;
using Newtonsoft.Json;

namespace DataModel
{
	public partial class Survey
	{
		public bool Update(DevKitDB db, ref string resp)
		{
			var user = db.currentUser;

			switch (updateCommand)
			{
				case "entity":
					{
						db.Update(this);
						break;
					}

				case "newOption":
					{
						var ent = JsonConvert.DeserializeObject<SurveyOption>(anexedEntity.ToString());

						ent.fkSurvey = id;
                        
						if (ent.id == 0)
							db.Insert(ent);
						else
							db.Update(ent);

						break;
					}

				case "removeOption":
					{
						var ent = JsonConvert.DeserializeObject<SurveyOption>(anexedEntity.ToString());

						db.Delete(ent);
						break;
					}

				case "optionSelect":
					{
						var ent = JsonConvert.DeserializeObject<SurveySelOption>(anexedEntity.ToString());

						db.Insert(new SurveyUserOption
						{
							fkSurvey = this.id,
							fkSurveyOption = ent.id,
							fkUser = user.id
						});
																		
						break;
					}
			}

			return true;
		}
	}
}
