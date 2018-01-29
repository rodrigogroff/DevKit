using LinqToDB;
using System.Linq;
using System.Text;

namespace DataModel
{
	public class AssociadoFilter : BaseFilter
    {
		public string   matricula,
                        email,
                        cpf,
                        phone;
        
        public string Parameters()
        {
            return Export();
        }

        string Export()
        {
            var ret = new StringBuilder();
            
            ret.Append(skip + ",");
            ret.Append(take + ",");            
            ret.Append(busca + ",");
            ret.Append(fkEmpresa + ",");

            ret.Append(matricula + ",");
            ret.Append(email + ",");
            ret.Append(cpf + ",");
            ret.Append(phone + ",");

            return ret.ToString();
        }
    }

	public partial class Associado
    {
		public AssociadoReport ComposedFilters(DevKitDB db, AssociadoFilter filter)
		{
            var query = from e in db.Associado
                        where e.fkEmpresa == db.currentUser.fkEmpresa
                        select e;

            if (!string.IsNullOrEmpty(filter.busca))
                query = from e in query
                        where e.stName.ToUpper().Contains(filter.busca) || e.stCPF.Equals(filter.busca)
                        select e;

            if (!string.IsNullOrEmpty(filter.cpf))
                query = from e in query where e.stCPF.Equals(filter.cpf) select e;

            if (filter.email != null)
				query = from e in query
						join eMail in db.AssociadoEmail on e.id equals eMail.fkUser
						where e.id == eMail.fkUser
						where eMail.stEmail.ToUpper().Contains (filter.email)
						select e;

			if (filter.phone != null)
				query = from e in query
						join ePhone in db.AssociadoTelefone on e.id equals ePhone.fkUser
						where e.id == ePhone.fkUser
						where ePhone.stPhone.ToUpper().Contains(filter.phone)
						select e;

            if (!string.IsNullOrEmpty(filter.matricula))
                query = from e in query
                        where e.nuMatricula.ToString() == filter.matricula
                        select e;

            var count = query.Count();

			query = query.OrderBy(y => y.stName);

            return new AssociadoReport
            {
                count = count,
                results = Loader(db, (query.Skip(filter.skip).Take(filter.take)).ToList())
            };
        }
	}
}
