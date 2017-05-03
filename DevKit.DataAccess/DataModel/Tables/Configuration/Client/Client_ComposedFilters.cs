using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public class ClientFilter
	{
		public int skip, take;
		public string busca;
	}
	
	public partial class Client
	{
		public List<Client> ComposedFilters(DevKitDB db, ref int count, ClientFilter filter)
		{
			var user = db.GetCurrentUser();
			
			var query = from e in db.Clients select e;

			if (filter.busca != null)
				query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			count = query.Count();

			query = query.OrderBy(y => y.stName);

			var results = (query.Skip(() => filter.skip).Take(() => filter.take)).ToList();

			results.ForEach(y => { y = y.LoadAssociations(db); });

			new AuditLog {
				fkUser = user.id,
				fkActionLog = EnumAuditAction.ClientListing,
				nuType = EnumAuditType.Client
			}.
			Create(db, "", "count: " + count);

			return results;
		}
	}
}
