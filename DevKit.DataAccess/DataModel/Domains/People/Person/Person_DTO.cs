using System.Collections.Generic;

namespace DataModel
{
	public partial class Person
	{
        public object anexedEntity;

		public string sdtLastContact = "",
                      sdtLastUpdate = "",
                      sfkUserLastContact = "",
                      sfkUserLastUpdate = "",
                      sdtStart = "",
					  updateCommand = "";

		public List<PersonPhone> phones;
		public List<PersonEmail> emails;
	}

    public class PersonReport
    {
        public int count = 0;
        public List<Person> results = new List<Person>();
    }
}
