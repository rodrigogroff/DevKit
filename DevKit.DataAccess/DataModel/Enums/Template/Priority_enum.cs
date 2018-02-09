using System.Linq;
using System.Collections.Generic;
using DevKit.DataAccess;

namespace DataModel
{
    public class PriorityReport
    {
        public int count;
        public List<ComboItem> results;
    }

    public class EnumPriority
	{
		public List<ComboItem> itens = new List<ComboItem>();

        public const long Emergency = 1,
                            High = 2,
                            Normal = 3,
                            Low = 4,
                            Register = 5;

        public EnumPriority()
		{
			itens.Add(new ComboItem() { id = 1, stName = "Emergência" });
			itens.Add(new ComboItem() { id = 2, stName = "Alta" });
			itens.Add(new ComboItem() { id = 3, stName = "Normal" });
			itens.Add(new ComboItem() { id = 4, stName = "Baixa" });
			itens.Add(new ComboItem() { id = 5, stName = "Registro" });
		}

		public ComboItem Get(long _id)
		{
			return itens.Where(y => y.id == _id).FirstOrDefault();
		}
	}
}
