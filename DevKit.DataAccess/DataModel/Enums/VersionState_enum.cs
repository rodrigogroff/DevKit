using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public class VersionState
	{
		public long id { get; set; }
		public string stName { get; set; }
	}

	public class EnumVersionState
	{
		public List<VersionState> lst = new List<VersionState>();

		public const long Analysis = 1,
							Development = 2,
							Homologation = 3,
							Production = 4,
							Closed = 5;

		public EnumVersionState()
		{
			lst.Add(new VersionState() { id = 1, stName = "Analysis" });
			lst.Add(new VersionState() { id = 2, stName = "Development" });
			lst.Add(new VersionState() { id = 3, stName = "Homologation" });
			lst.Add(new VersionState() { id = 4, stName = "Production" });
			lst.Add(new VersionState() { id = 5, stName = "Closed" });
		}

		public VersionState Get(long _id)
		{
			return lst.Where(y => y.id == _id).FirstOrDefault();
		}
	}
}
