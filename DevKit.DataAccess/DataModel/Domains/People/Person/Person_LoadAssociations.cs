using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System;

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

            if (fkUserAdd != null)
                sfkUserAdd = db.GetUser(fkUserAdd).stLogin;

            if (fkUserLastContact != null)
                sfkUserLastContact = db.GetUser(fkUserLastContact).stLogin;

            if (fkUserLastUpdate != null)
                sfkUserLastUpdate = db.GetUser(fkUserLastUpdate).stLogin;

            if (nuYearBirth != null)
                snuAge = (DateTime.Now.Year - nuYearBirth).ToString();

            phones = LoadPhones(db);
			emails = LoadEmails(db);
            enderecos = LoadEnderecos(db);

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

        List<PersonAddress> LoadEnderecos(DevKitDB db)
        {
            return (from e in db.PersonAddress where e.fkUser == id select e).
                OrderByDescending(t => t.id).
                ToList();
        }
    }
}
