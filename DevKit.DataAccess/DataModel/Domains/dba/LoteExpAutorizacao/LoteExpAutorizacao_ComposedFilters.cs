using LinqToDB;
using System.Linq;

namespace DataModel
{
	public class LoteExpAutorizacaoFilter 
    {
		public long? fkEmpresa, nuMes, nuAno;
    }

    public partial class LoteExpAutorizacao
    {
        public LoteExpAutorizacaoReport ComposedFilters(DevKitDB db, LoteExpAutorizacaoFilter filter)
        {
            var query = from e in db.Autorizacao select e;

            if (filter.fkEmpresa != null)
                query = from e in query
                        where e.fkEmpresa == filter.fkEmpresa
                        select e;

            if (filter.nuMes != null)
                query = from e in query
                        where e.nuMes == filter.nuMes
                        select e;

            if (filter.nuAno != null)
                query = from e in query
                        where e.nuAno == filter.nuAno
                        select e;

            var lstMesAno = query.
                            Select(y => y.nuMes.ToString().PadLeft(2,'0') + 
                                        y.nuAno.ToString().PadLeft(4, '0') +
                                        y.fkEmpresa.ToString().PadLeft(4,'0')).
                            ToList().
                            Distinct().
                            ToList();

            return new LoteExpAutorizacaoReport
            {
                count = lstMesAno.Count(),
                results = Loader(db, lstMesAno)
            };
        }
    }
}
