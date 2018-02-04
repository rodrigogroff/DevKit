using LinqToDB;
using System.Linq;
using System.Text;

namespace DataModel
{
	public class MedicoFilter : BaseFilter
    {
		public long? nuCodigo;
        public string nome, especialidade;
    }

    public partial class Medico
    {
        public MedicoReport ComposedFilters(DevKitDB db, MedicoFilter filter)
        {
            var query = from e in db.Medico
                        select e;

            if (filter.fkEmpresa != null)
            {
                query = from e in query
                        join emiMedic in db.MedicoEmpresa on e.id equals emiMedic.fkMedico
                        where emiMedic.fkEmpresa == filter.fkEmpresa
                        select e;
            }

            if (filter.nuCodigo != null && filter.nuCodigo > 0)
            {
                query = from e in query
                        where e.nuCodigo == filter.nuCodigo
                        select e;
            }

            if (!string.IsNullOrEmpty(filter.nome))
            {
                query = from e in query
                        where e.stNome.ToUpper().Contains(filter.nome.ToUpper())
                        select e;
            }

            if (!string.IsNullOrEmpty(filter.especialidade))
            {
                query = from e in query
                        join espec in db.Especialidade on e.fkEspecialidade equals espec.id
                        where espec.stNome.ToUpper().Contains (filter.especialidade.ToUpper())
                        select e;
            }

            var count = query.Count();

            query = query.OrderBy(y => y.stNome);

            return new MedicoReport
            {
                count = count,
                results = Loader(db, (query.Skip(filter.skip).Take(filter.take)).ToList())
            };
        }
    }
}
