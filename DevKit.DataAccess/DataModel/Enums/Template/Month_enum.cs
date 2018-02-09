using System.Linq;
using System.Collections.Generic;
using DevKit.DataAccess;

namespace DataModel
{
    public class MonthReport
    {
        public int count;
        public List<ComboItem> results;
    }

    public class EnumMonth
    {
        public List<ComboItem> itens = new List<ComboItem>();

        public EnumMonth()
        {
            int t = 1;

            itens.Add(new ComboItem() { id = t++, stName = "Janeiro" });
            itens.Add(new ComboItem() { id = t++, stName = "Fevereiro" });
            itens.Add(new ComboItem() { id = t++, stName = "Março" });
            itens.Add(new ComboItem() { id = t++, stName = "Abril" });
            itens.Add(new ComboItem() { id = t++, stName = "Maio" });
            itens.Add(new ComboItem() { id = t++, stName = "Junho" });
            itens.Add(new ComboItem() { id = t++, stName = "Julho" });
            itens.Add(new ComboItem() { id = t++, stName = "Agosto" });
            itens.Add(new ComboItem() { id = t++, stName = "Setembro" });
            itens.Add(new ComboItem() { id = t++, stName = "Outubro" });
            itens.Add(new ComboItem() { id = t++, stName = "Novembro" });
            itens.Add(new ComboItem() { id = t++, stName = "Dezembro" });
        }

        public ComboItem Get(long _id)
        {
            return itens.Where(y => y.id == _id).FirstOrDefault();
        }
    }
}

