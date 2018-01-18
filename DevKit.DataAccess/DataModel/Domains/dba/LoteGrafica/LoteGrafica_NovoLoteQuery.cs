using LinqToDB;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataModel
{
    public partial class LoteGrafica
    {
        public NovoLoteGraficaReport NovoLoteQuery(DevKitDB db)
        {
            var queryEmpresas = (from e in db.Person
                                 join emp in db.Empresa on e.fkEmpresa equals emp.id
                                 where e.tgExpedicao == 0
                                 where e.tgStatus == 0
                                 select emp).
                                 Distinct().
                                 ToList();

            var res = new List<NovoLoteGraficaDTO>();

            foreach (var empresa in queryEmpresas)
            {
                res.Add(new NovoLoteGraficaDTO
                {
                    selecionado = false,
                    id = empresa.id.ToString(),
                    empresa = empresa.nuEmpresa + " - " + empresa.stNome,
                    qtdCartoes = (from e in db.Person
                                  where e.tgExpedicao == 0
                                  where e.tgStatus == 0
                                  where e.fkEmpresa == empresa.id
                                  select e).
                                  Count().
                                  ToString()
                });
            }

            return new NovoLoteGraficaReport
            {
                count = queryEmpresas.Count(),
                results = res
            };
        }
    }
}
