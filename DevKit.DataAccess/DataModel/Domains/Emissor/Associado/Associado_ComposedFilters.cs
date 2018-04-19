using LinqToDB;
using System;
using System.Linq;
using System.Text;

namespace DataModel
{
	public class AssociadoFilter : BaseFilter
    {
        public string matricula,
                        matSaude,
                        fkSecao,
                        email,
                        cpf,
                        phone,
                        tgSituacao,
                        tgExpedicao;
        
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
            ret.Append(fkSecao + ",");
            ret.Append(matricula + ",");
            ret.Append(matSaude + ",");
            ret.Append(email + ",");
            ret.Append(cpf + ",");
            ret.Append(phone + ",");
            ret.Append(tgSituacao + ",");
            ret.Append(tgExpedicao + ",");

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

            if (!string.IsNullOrEmpty(filter.fkSecao))
                query = from e in query
                        where e.fkSecao.ToString() == filter.fkSecao
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

            if (!string.IsNullOrEmpty(filter.matSaude))
                query = from e in query
                        where e.nuMatSaude.ToString() == filter.matSaude
                        select e;

            if (filter.tgSituacao != null)
                query = from e in query
                        where e.tgStatus == Convert.ToInt32(filter.tgSituacao) - 1
                        select e;

            if (filter.tgExpedicao != null)
                query = from e in query
                        where e.tgExpedicao == Convert.ToInt32(filter.tgExpedicao) - 1
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
