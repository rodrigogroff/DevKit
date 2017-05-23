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

        public const long Money = 1,
                            Hours = 2,
                            Quantity = 3;
        
        public EnumAccumulatorType()
		{
			lst.Add(new AccumulatorType() { id = 1, stName = "Money" });
			lst.Add(new AccumulatorType() { id = 2, stName = "Hours" });
            lst.Add(new AccumulatorType() { id = 3, stName = "Quantity" });
        }

		public AccumulatorType Get(long? _id)
		{
			return lst.Where(y => y.id == _id).FirstOrDefault();
		}

		public string GetName(long? _id)
		{
			var it = lst.Where(y => y.id == _id).FirstOrDefault();

			if (it != null)
				return it.stName;

			return "";
		}

        public string transformMoneyFromLong(long val)
        {
            var ret = val.ToString();

            if (ret.Length < 3)
                ret = ret.PadLeft(3, '0');

            ret = ret.Insert( ret.Length - 2, ",");

            if (ret.Length > 6)
                ret = ret.Insert(ret.Length - 6, ".");

            if (ret.Length > 9)
                ret = ret.Insert(ret.Length - 10, ".");

            return ret;
        }
	}
}
