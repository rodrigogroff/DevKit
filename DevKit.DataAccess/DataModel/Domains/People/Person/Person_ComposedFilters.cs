using LinqToDB;
using System.Linq;
using System.Text;

namespace DataModel
{
	public class PersonFilter : BaseFilter
    {
		public string email,
                      cpf,
                      phone;
        
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
            ret.Append(email + ",");
            ret.Append(cpf + ",");
            ret.Append(phone + ",");

            return ret.ToString();
        }
    }

	public partial class Person
	{
		public PersonReport ComposedFilters(DevKitDB db, PersonFilter filter)
		{
			var query = from e in db.Person select e;

            if (!string.IsNullOrEmpty(filter.busca))
                query = from e in query
                        where e.stName.ToUpper().Contains(filter.busca) || e.stCPF.Equals(filter.busca)
                        select e;

            if (!string.IsNullOrEmpty(filter.cpf))
                query = from e in query where e.stCPF.Equals(filter.cpf) select e;

            if (filter.email != null)
			{
				query = from e in query
						join eMail in db.PersonEmail on e.id equals eMail.fkUser
						where e.id == eMail.fkUser
						where eMail.stEmail.ToUpper().Contains (filter.email)
						select e;
			}

			if (filter.phone != null)
			{
				query = from e in query
						join ePhone in db.PersonPhone on e.id equals ePhone.fkUser
						where e.id == ePhone.fkUser
						where ePhone.stPhone.ToUpper().Contains(filter.phone)
						select e;
			}

			var count = query.Count();

			query = query.OrderBy(y => y.stName);

            return new PersonReport
            {
                count = count,
                results = Loader(db, (query.Skip(filter.skip).Take(filter.take)).ToList())
            };
        }
	}
}
