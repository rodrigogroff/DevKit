using System.Linq;
using System.Collections.Generic;
using DevKit.DataAccess;

namespace DataModel
{
    public class DayMonthReport
    {
        public int count;
        public List<ComboItem> results;
    }

    public class EnumDayMonth
    {
        public List<ComboItem> itens = new List<ComboItem>();

        public EnumDayMonth()
        {
            for (int t=1; t <= 31; ++t)
                itens.Add(new ComboItem() { id = t, stName = t.ToString() });
        }

        public ComboItem Get(long _id)
        {
            return itens.Where(y => y.id == _id).FirstOrDefault();
        }
    }
}

