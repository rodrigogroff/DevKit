using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public class AccumulatorType
	{
		public long id { get; set; }
		public string stName { get; set; }
	}

	public class EnumAccumulatorType
	{
		public List<AccumulatorType> lst = new List<AccumulatorType>();

		public EnumAccumulatorType()
		{
			lst.Add(new AccumulatorType() { id = 1, stName = "Money" });
			lst.Add(new AccumulatorType() { id = 2, stName = "Hours" });
		}

		public string GetName(long? _id)
		{
			var it = lst.Where(y => y.id == _id).FirstOrDefault();

			if (it != null)
				return it.stName;

			return "";
		}
	}
}
