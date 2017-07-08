using LinqToDB;
using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public partial class Person
	{
		public Person LoadAssociations(DevKitDB db)
		{
			var setup = db.GetSetup();

            sdtLastUpdate = dtLastUpdate?.ToString(setup.stDateFormat);
            sdtLastContact = dtLastContact?.ToString(setup.stDateFormat);
            sdtStart = dtStart?.ToString(setup.stDateFormat);

            if (fkUserLastContact != null)
                sfkUserLastContact = db.GetUser(fkUserLastContact).stLogin;

            if (fkUserLastUpdate != null)
                sfkUserLastUpdate = db.GetUser(fkUserLastUpdate).stLogin;

            phones = LoadPhones(db);
			emails = LoadEmails(db);
		
			return this;
		}
        
		List<PersonPhone> LoadPhones(DevKitDB db)
		{
			return (from e in db.PersonPhone where e.fkUser == id select e).
				OrderBy(t => t.stPhone).
				ToList();
		}

		List<PersonEmail> LoadEmails(DevKitDB db)
		{
			return (from e in db.PersonEmail where e.fkUser == id select e).
				OrderByDescending(t => t.id).
				ToList();
		}
	}
}
