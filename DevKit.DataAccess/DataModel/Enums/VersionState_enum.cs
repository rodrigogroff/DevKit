using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public class VersionState
	{
		public long id { get; set; }
		public string stName { get; set; }
	}

    public class VersionStateReport
    {
        public int count;
        public List<VersionState> results;
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
			lst.Add(new VersionState() { id = Analysis, stName = "Análise" });
			lst.Add(new VersionState() { id = Development, stName = "Desenvolvimento" });
			lst.Add(new VersionState() { id = Homologation, stName = "Homologação" });
			lst.Add(new VersionState() { id = Production, stName = "Produção" });
			lst.Add(new VersionState() { id = Closed, stName = "Encerrada" });
		}

		public VersionState Get(long _id)
		{
			return lst.Where(y => y.id == _id).FirstOrDefault();
		}
	}
}
