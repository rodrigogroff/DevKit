using LinqToDB;
using System;

namespace DataModel
{
	public partial class CompanyNews
	{
		public bool Update(DevKitDB db, long fkCurrentUser, ref string resp)
		{
			var user = db.GetCurrentUser(fkCurrentUser);

			if (CheckDuplicate(this, db))
			{
				resp = "News title already taken";
				return false;
			}

			switch (updateCommand)
			{
				case "entity":
					{
						db.Update(this);
						break;
					}

				case "maskAsRead":
					{
						db.Insert(new UserNewsRead {
							dtLog = DateTime.Now,
							fkNews = this.id,
							fkUser = user.id
						});

						break;
					}
			}

			return true;
		}
	}
}
