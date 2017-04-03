using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public class ProjectTemplate
	{
		public long id { get; set; }
		public string stName { get; set; }
	}

	public class EnumProjectTemplate
	{
		public List<ProjectTemplate> lst = new List<ProjectTemplate>();

		public const long Custom = 1,
						  CMMI2 = 2;

		public EnumProjectTemplate()
		{
			lst.Add(new ProjectTemplate() { id = Custom, stName = "Custom" });
			lst.Add(new ProjectTemplate() { id = CMMI2, stName = "CMMI-2" });
		}

		public ProjectTemplate Get(long? _id)
		{
			return lst.Where(y => y.id == _id).FirstOrDefault();
		}
	}
}
