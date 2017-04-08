using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public class AuditAction
	{
		public long id { get; set; }
		public string stName { get; set; }
	}

	public class EnumAuditAction
	{
		public List<AuditAction> lst = new List<AuditAction>();

		public const long Login = 1,
							ProjectListing = 2,
							ProjectCreation = 3,
							ProjectUpdate = 4,
							ProjectUpdateAddUser = 5,
							ProjectUpdateUpdateUser = 6,
							ProjectUpdateRemoveUser = 7,
							ProjectUpdateAddPhase = 8,
							ProjectUpdateUpdatePhase = 9,
							ProjectUpdateRemovePhase = 10,
							ProjectUpdateAddSprint = 11,
							ProjectUpdateUpdateSprint = 12,
							ProjectUpdateRemoveSprint = 13,
							ProjectDelete = 14,
							SprintAddVersion = 15,
							SprintUpdateVersion = 16,
							SprintRemoveVersion = 17,
							TaskTypeAdd = 18,
							TaskTypeDelete = 19,
							TaskTypeUpdate = 20,
							TaskTypeAddCategory = 21,
							TaskTypeUpdateCategory = 22,
							TaskTypeRemoveCategory = 23,
							CategoryAddFlow = 24,
							CategoryUpdateFlow = 25,
							CategoryRemoveFlow = 26,
							CategoryAddAccumulator = 27,
							CategoryUpdateAccumulator = 28,
							CategoryRemoveAccumulator = 29;

		public EnumAuditAction()
		{
			lst.Add(new AuditAction() { id = Login, stName = "System \\ Login" });
			lst.Add(new AuditAction() { id = ProjectListing, stName = "Configuration \\ Project Listing" });
			lst.Add(new AuditAction() { id = ProjectCreation, stName = "Configuration \\ Project Creation" });
			lst.Add(new AuditAction() { id = ProjectUpdate, stName = "Configuration \\ Project Update" });
			lst.Add(new AuditAction() { id = ProjectUpdateAddUser, stName = "Configuration \\ Project update, adding user" });
			lst.Add(new AuditAction() { id = ProjectUpdateUpdateUser, stName = "Configuration \\ Project update, updating user" });
			lst.Add(new AuditAction() { id = ProjectUpdateRemoveUser, stName = "Configuration \\ Project update, removing user" });
			lst.Add(new AuditAction() { id = ProjectUpdateAddPhase, stName = "Configuration \\ Project update, adding phase" });
			lst.Add(new AuditAction() { id = ProjectUpdateUpdatePhase, stName = "Configuration \\ Project update, updating phase" });
			lst.Add(new AuditAction() { id = ProjectUpdateRemovePhase, stName = "Configuration \\ Project update, removing phase" });
			lst.Add(new AuditAction() { id = ProjectUpdateAddSprint, stName = "Configuration \\ Project update, adding sprint" });
			lst.Add(new AuditAction() { id = ProjectUpdateUpdateSprint, stName = "Configuration \\ Project update, updating sprint" });
			lst.Add(new AuditAction() { id = ProjectUpdateRemoveSprint, stName = "Configuration \\ Project update, removing sprint" });
			lst.Add(new AuditAction() { id = ProjectDelete, stName = "Configuration \\ Project Delete" });
			lst.Add(new AuditAction() { id = SprintAddVersion, stName = "Configuration \\ Sprint update, adding version" });
			lst.Add(new AuditAction() { id = SprintUpdateVersion, stName = "Configuration \\ Sprint update, updating version" });
			lst.Add(new AuditAction() { id = SprintRemoveVersion, stName = "Configuration \\ Sprint update, removing version" });
			lst.Add(new AuditAction() { id = TaskTypeAdd, stName = "Configuration \\ Task Type Added" });
			lst.Add(new AuditAction() { id = TaskTypeDelete, stName = "Configuration \\ Task Type Delete" });
			lst.Add(new AuditAction() { id = TaskTypeUpdate, stName = "Configuration \\ Task Type Update" });
			lst.Add(new AuditAction() { id = TaskTypeAddCategory, stName = "Configuration \\ Task Type, adding category" });
			lst.Add(new AuditAction() { id = TaskTypeUpdateCategory, stName = "Configuration \\ Task Type, updating category" });
			lst.Add(new AuditAction() { id = TaskTypeRemoveCategory, stName = "Configuration \\ Task Type, removing category" });
			lst.Add(new AuditAction() { id = CategoryAddFlow, stName = "Configuration \\ Task Category, adding flow" });
			lst.Add(new AuditAction() { id = CategoryUpdateFlow, stName = "Configuration \\ Task Category, updating flow" });
			lst.Add(new AuditAction() { id = CategoryRemoveFlow, stName = "Configuration \\ Task Category, removing flow" });
			lst.Add(new AuditAction() { id = CategoryAddAccumulator, stName = "Configuration \\ Task Category, adding accumulator" });
			lst.Add(new AuditAction() { id = CategoryUpdateAccumulator, stName = "Configuration \\ Task Category, updating accumulator" });
			lst.Add(new AuditAction() { id = CategoryRemoveAccumulator, stName = "Configuration \\ Task Category, removing accumulator" });

			lst.OrderBy(y => y.stName);
		}

		public AuditAction Get(long _id)
		{
			return lst.Where(y => y.id == _id).FirstOrDefault();
		}
	}
}
