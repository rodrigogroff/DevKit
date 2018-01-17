using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
	public partial class LoteGrafica
	{
		public LoteGrafica LoadAssociations(DevKitDB db)
		{
            var setup = db.GetSetup();

            sdtLog = dtLog?.ToString(setup.stDateFormat);

            sqtdCartoes = db.LoteGraficaCartao.
                            Where(y => y.fkLoteGrafica == id).
                                Count().ToString();

            var lstEmpresas = db.LoteGraficaCartao.
                            Where(y => y.fkLoteGrafica == id).
                            Select(y => y.fkEmpresa).
                            ToList();

            empresas = db.Empresa.
                        Where(y => lstEmpresas.Contains(y.id)).
                        ToList();
            
            return this;
		}
	}
}
