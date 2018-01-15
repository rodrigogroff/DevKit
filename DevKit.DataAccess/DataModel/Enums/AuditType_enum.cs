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
                            Setup = 7,
                            Client = 8,
                            ClientGroup = 9,
                            Person = 10;

		public EnumAuditType()
		{
			lst.Add(new AuditType() { id = Project, stName = "Project" });
			lst.Add(new AuditType() { id = Sprint, stName = "Sprint" });
			lst.Add(new AuditType() { id = TaskType, stName = "TaskType" });
			lst.Add(new AuditType() { id = Task, stName = "Task" });
			lst.Add(new AuditType() { id = User, stName = "User" });
			lst.Add(new AuditType() { id = Profile, stName = "Profile" });
			lst.Add(new AuditType() { id = Setup, stName = "Setup" });
			lst.Add(new AuditType() { id = Client, stName = "Client" });
			lst.Add(new AuditType() { id = ClientGroup, stName = "ClientGroup" });
            lst.Add(new AuditType() { id = Person, stName = "Person" });

            lst.OrderBy(y => y.stName);
		}

		public AuditType Get(long _id)
		{
			return lst.Where(y => y.id == _id).FirstOrDefault();
		}
	}
}
