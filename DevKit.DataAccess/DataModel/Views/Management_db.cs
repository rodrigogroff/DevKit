using System.Linq;
using System.Collections.Generic;

namespace DataModel
{
	public class ManagementFilter
	{
		public long? fkProject;
	}

	public class Management_dto
	{
		public bool fail = false;
	}

	public class Management
	{
		public Management_dto ComposedFilters(DevKitDB db, ManagementFilter filter, User user)
		{
			var dto = new Management_dto();

			return dto;
		}
	}
}
