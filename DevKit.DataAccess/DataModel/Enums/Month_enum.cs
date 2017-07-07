using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
    public class Month
    {
        public long id { get; set; }
        public string stName { get; set; }
    }

    public class MonthReport
    {
        public int count;
        public List<Month> results;
    }

    public class EnumMonth
    {
        public List<Month> lst = new List<Month>();

        public const long January = 1,
                            February = 2,
                            March = 3,
                            April = 4,
                            May = 5,
                            June = 6,
                            July = 7,
                            August = 8,
                            September = 9,
                            October = 10,
                            November = 11,
                            December = 12;

        public EnumMonth()
        {
            lst.Add(new Month() { id = January, stName = "January" });
            lst.Add(new Month() { id = February, stName = "February" });
            lst.Add(new Month() { id = March, stName = "March" });
            lst.Add(new Month() { id = April, stName = "April" });
            lst.Add(new Month() { id = May, stName = "May" });
            lst.Add(new Month() { id = June, stName = "June" });
            lst.Add(new Month() { id = July, stName = "July" });
            lst.Add(new Month() { id = August, stName = "August" });
            lst.Add(new Month() { id = September, stName = "September" });
            lst.Add(new Month() { id = October, stName = "October" });
            lst.Add(new Month() { id = November, stName = "November" });
            lst.Add(new Month() { id = December, stName = "December" });
        }

        public Month Get(long _id)
        {
            return lst.Where(y => y.id == _id).FirstOrDefault();
        }
    }
}

