using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public class ProjectTemplate
	{
		public long id { get; set; }
		public string stName { get; set; }
	}

    public class ProjectTemplateReport
    {
        public int count;
        public List<ProjectTemplate> results;
    }

    public class EnumProjectTemplate
	{
		public List<ProjectTemplate> lst = new List<ProjectTemplate>();

		public const long Custom = 1,
						  SoftwareMaintenance = 3;

		public EnumProjectTemplate()
		{
			lst.Add(new ProjectTemplate() { id = Custom, stName = "Customizado" });
			lst.Add(new ProjectTemplate() { id = SoftwareMaintenance, stName = "Software" });
		}

		public ProjectTemplate Get(long? _id)
		{
			return lst.Where(y => y.id == _id).FirstOrDefault();
		}
	}
}
