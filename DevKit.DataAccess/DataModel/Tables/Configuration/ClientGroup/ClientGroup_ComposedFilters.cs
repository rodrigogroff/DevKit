using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public class ClientGroupFilter
	{
		public int skip, take;
		public string busca;
	}
	
	public partial class ClientGroup
	{
		public List<ClientGroup> ComposedFilters(DevKitDB db, ref int count, ClientGroupFilter filter)
		{
			var user = db.GetCurrentUser();
			
			var query = from e in db.ClientGroups select e;

			if (filter.busca != null)
				query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			count = query.Count();

			query = query.OrderBy(y => y.stName);

			var results = (query.Skip(() => filter.skip).Take(() => filter.take)).ToList();

			results.ForEach(y => { y = y.LoadAssociations(db); });

			new AuditLog {
				fkUser = user.id,
				fkActionLog = EnumAuditAction.ClientGroupListing,
				nuType = EnumAuditType.ClientGroup
			}.
			Create(db, "", "count: " + count);

			return results;
		}
	}
}
