using System.Linq;
using System.Collections.Generic;
using DevKit.DataAccess;

namespace DataModel
{
	public class EnumAuditType
	{
		public List<ComboItem> itens = new List<ComboItem>();

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
            itens.Add(new ComboItem() { id = 1, stName = "Project" });
            itens.Add(new ComboItem() { id = 2, stName = "Sprint" });
            itens.Add(new ComboItem() { id = 3, stName = "TaskType" });
            itens.Add(new ComboItem() { id = 4, stName = "Task" });
            itens.Add(new ComboItem() { id = 5, stName = "User" });
            itens.Add(new ComboItem() { id = 6, stName = "Profile" });
            itens.Add(new ComboItem() { id = 7, stName = "Setup" });
            itens.Add(new ComboItem() { id = 8, stName = "Client" });
            itens.Add(new ComboItem() { id = 9, stName = "ClientGroup" });
            itens.Add(new ComboItem() { id = 10, stName = "Person" });

            itens.OrderBy(y => y.stName);
		}

		public ComboItem Get(long _id)
		{
			return itens.Where(y => y.id == _id).FirstOrDefault();
		}
	}
}
