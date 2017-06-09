using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public class ClientFilter : BaseFilter
	{
		
	}
	
	public partial class Client
	{
		public List<Client> ComposedFilters(DevKitDB db, ref int count, ClientFilter filter)
		{
			var user = db.currentUser;
			
			var query = from e in db.Client select e;

            if (!string.IsNullOrEmpty(filter.busca))
                query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			count = query.Count();

			query = query.OrderBy(y => y.stName);
            
            new AuditLog
            {
                fkUser = user.id,
                fkActionLog = EnumAuditAction.ClientListing,
                nuType = EnumAuditType.Client
            }.
            Create(db, "", "count: " + count);

            return Loader(db, (query.Skip(() => filter.skip).Take(() => filter.take)).ToList(), true);
        }
	}
}
