using LinqToDB;
using System.Linq;
using System.Text;

namespace DataModel
{
	public class CredenciadoFilter : BaseFilter
    {
		public long? nuCodigo, fkSecao;
        public string nome, especialidade;
    }

    public partial class Credenciado
    {
        public CredenciadoReport ComposedFilters(DevKitDB db, CredenciadoFilter filter)
        {
            var query = from e in db.Credenciado
                        select e;

            if (filter.fkEmpresa != null)
            {
                query = from e in query
                        join emiMedic in db.CredenciadoEmpresa on e.id equals emiMedic.fkCredenciado
                        where emiMedic.fkEmpresa == filter.fkEmpresa
                        select e;
            }

            if (filter.nuCodigo != null && filter.nuCodigo > 0)
            {
                query = from e in query
                        where e.nuCodigo == filter.nuCodigo
                        select e;
            }

            if (filter.fkSecao != null && filter.fkSecao > 0)
            {
                var emp = db.EmpresaSecao.Where(y => y.id == filter.fkSecao).FirstOrDefault().fkEmpresa;

                query = from e in query
                        join convenio in db.CredenciadoEmpresa on e.id equals convenio.fkCredenciado
                        where convenio.fkCredenciado == e.id
                        where convenio.fkEmpresa == emp
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

            return new CredenciadoReport
            {
                count = count,
                results = Loader(db, (query.Skip(filter.skip).Take(filter.take)).ToList())
            };
        }
    }
}
