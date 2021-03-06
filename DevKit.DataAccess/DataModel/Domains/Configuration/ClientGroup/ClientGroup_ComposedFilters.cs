﻿using LinqToDB;
using System.Linq;
using System.Text;

namespace DataModel
{
	public class ClientGroupFilter : BaseFilter
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
	
	public partial class ClientGroup
	{
		public ClientGroupReport ComposedFilters(DevKitDB db, ClientGroupFilter filter, bool bSaveAuditLog)
		{
			var user = db.currentUser;
			
			var query = from e in db.ClientGroup select e;

            if (!string.IsNullOrEmpty(filter.busca))
                query = from e in query where e.stName.ToUpper().Contains(filter.busca) select e;

			var count = query.Count();

			query = query.OrderBy(y => y.stName);
            
            if (bSaveAuditLog)
            {
                new AuditLog
                {
                    fkUser = user.id,
                    fkActionLog = EnumAuditAction.ClientGroupListing,
                    nuType = EnumAuditType.ClientGroup
                }.
                Create(db, "", "count: " + count);
            }

            return new ClientGroupReport
            {
                count = count,
                results = Loader(db, (query.Skip(filter.skip).Take(filter.take)).ToList(), true)
            };
        }
	}
}
