using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
    public class DayMonth
    {
        public long id { get; set; }
        public string stName { get; set; }
    }

    public class DayMonthReport
    {
        public int count;
        public List<DayMonth> results;
    }

    public class EnumDayMonth
    {
        public List<DayMonth> lst = new List<DayMonth>();

        public EnumDayMonth()
        {
            for (int t=1; t <= 31; ++t)
                lst.Add(new DayMonth() { id = t, stName = t.ToString() });
        }

        public DayMonth Get(long _id)
        {
            return lst.Where(y => y.id == _id).FirstOrDefault();
        }
    }
}

