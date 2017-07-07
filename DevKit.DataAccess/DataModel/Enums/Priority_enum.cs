using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public class Priority
	{
		public long id { get; set; }
		public string stName { get; set; }
	}

    public class PriorityReport
    {
        public int count;
        public List<Priority> results;
    }

    public class EnumPriority
	{
		public List<Priority> lst = new List<Priority>();

		public const long	Emergency = 1,
							High = 2,
							Normal = 3,
							Low = 4,
							Register = 5;

		public EnumPriority()
		{
			lst.Add(new Priority() { id = 1, stName = "Emergency" });
			lst.Add(new Priority() { id = 2, stName = "High" });
			lst.Add(new Priority() { id = 3, stName = "Normal" });
			lst.Add(new Priority() { id = 4, stName = "Low" });
			lst.Add(new Priority() { id = 5, stName = "Register" });
		}

		public Priority Get(long _id)
		{
			return lst.Where(y => y.id == _id).FirstOrDefault();
		}
	}
}
