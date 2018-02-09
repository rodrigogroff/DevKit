using System.Linq;
using System.Collections.Generic;
using DevKit.DataAccess;

namespace DataModel
{
    public class ProjectTemplateReport
    {
        public int count;
        public List<ComboItem> results;
    }

    public class EnumProjectTemplate
	{
		public List<ComboItem> itens = new List<ComboItem>();

        public const long Custom = 1,
                          SoftwareMaintenance = 3;

        public EnumProjectTemplate()
		{
			itens.Add(new ComboItem() { id = 1, stName = "Customizado" });
			itens.Add(new ComboItem() { id = 3, stName = "Software" });
		}

		public ComboItem Get(long? _id)
		{
			return itens.Where(y => y.id == _id).FirstOrDefault();
		}
	}
}
