using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public class AuditType
	{
		public long id { get; set; }
		public string stName { get; set; }
	}

	public class EnumAuditType
	{
		public List<AuditType> lst = new List<AuditType>();

		public const long Project = 1,
							Sprint = 2,
							TaskType = 3,
							Task = 4,
							User = 5,
							Profile = 6,
							Setup = 7;

		public EnumAuditType()
		{
			lst.Add(new AuditType() { id = Project, stName = "Project" });
			lst.Add(new AuditType() { id = Sprint, stName = "Sprint" });
			lst.Add(new AuditType() { id = TaskType, stName = "TaskType" });
			lst.Add(new AuditType() { id = Task, stName = "Task" });
			lst.Add(new AuditType() { id = User, stName = "User" });
			lst.Add(new AuditType() { id = Profile, stName = "Profile" });
			lst.Add(new AuditType() { id = Setup, stName = "Setup" });

			lst.OrderBy(y => y.stName);
		}

		public AuditType Get(long _id)
		{
			return lst.Where(y => y.id == _id).FirstOrDefault();
		}
	}
}
