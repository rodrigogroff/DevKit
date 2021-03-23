
using System.Collections.Generic;
using System.Linq;

namespace Master.Data.Const
{
    public class Month
    {
        public long id { get; set; }
        public string stName { get; set; }
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
            lst.Add(new Month() { id = January, stName = "Janeiro" });
            lst.Add(new Month() { id = February, stName = "Fevereiro" });
            lst.Add(new Month() { id = March, stName = "Março" });
            lst.Add(new Month() { id = April, stName = "Abril" });
            lst.Add(new Month() { id = May, stName = "Maio" });
            lst.Add(new Month() { id = June, stName = "Junho" });
            lst.Add(new Month() { id = July, stName = "Julho" });
            lst.Add(new Month() { id = August, stName = "Agosto" });
            lst.Add(new Month() { id = September, stName = "Setembro" });
            lst.Add(new Month() { id = October, stName = "Outubro" });
            lst.Add(new Month() { id = November, stName = "Novembro" });
            lst.Add(new Month() { id = December, stName = "Dezembro" });
        }

        public Month Get(long _id)
        {
            return lst.Where(y => y.id == _id).FirstOrDefault();
        }
    }
}
