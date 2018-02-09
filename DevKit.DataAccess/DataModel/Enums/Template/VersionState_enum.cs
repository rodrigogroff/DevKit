using System.Linq;
using System.Collections.Generic;
using DevKit.DataAccess;

namespace DataModel
{
    public class VersionStateReport
    {
        public int count;
        public List<ComboItem> results;
    }

    public class EnumVersionState
	{
		public List<ComboItem> lst = new List<ComboItem>();

        public const long Analysis = 1,
                            Development = 2,
                            Homologation = 3,
                            Production = 4,
                            Closed = 5;

        public EnumVersionState()
		{
			lst.Add(new ComboItem() { id = 1, stName = "Análise" });
			lst.Add(new ComboItem() { id = 2, stName = "Desenvolvimento" });
			lst.Add(new ComboItem() { id = 3, stName = "Homologação" });
			lst.Add(new ComboItem() { id = 4, stName = "Produção" });
			lst.Add(new ComboItem() { id = 5, stName = "Encerrada" });
		}

		public ComboItem Get(long _id)
		{
			return lst.Where(y => y.id == _id).FirstOrDefault();
		}
	}
}
