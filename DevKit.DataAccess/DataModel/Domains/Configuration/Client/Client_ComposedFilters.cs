using LinqToDB;
using System.Linq;
using System.Text;

namespace DataModel
{
	public class ClientFilter : BaseFilter
	{
        public string Parameters()
        {
            return Export();
        }

        string Export()
        {
            var ret = new StringBuilder();

            // base
            ret.Append(skip + ",");
            ret.Append(take + ",");
            ret.Append(busca + ",");

            return ret.ToString();
        }
    }
	
	public partial class Client
	{
		public ClientReport ComposedFilters(DevKitDB db, ClientFilter filter, bool bSaveAudit)
		{
			var user = db.currentUser;
			
			var query = from e in db.Client select e;

            if (!string.IsNullOrEmpty(filter.busca))
                query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			var count = query.Count();

			query = query.OrderBy(y => y.stName);
            
            if (bSaveAudit)
            {
                new AuditLog
                {
                    fkUser = user.id,
                    fkActionLog = EnumAuditAction.ClientListing,
                    nuType = EnumAuditType.Client
                }.
                Create(db, "", "count: " + count);
            }

            return new ClientReport
            {
                count = count,
                results = Loader(db, (query.Skip(filter.skip).Take(filter.take)).ToList(), true)
            };
        }
	}
}
