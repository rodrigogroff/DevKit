using System.Collections.Generic;

namespace DataModel
{
	public partial class LoteGrafica
    {
        public object anexedEntity;

		public string sdtLog = "",
                      sqtdCartoes = "",
						updateCommand = "";

        public List<Empresa> empresas = new List<Empresa>();
	}

    public class LoteGraficaReport
    {
        public int count = 0;
        public List<LoteGrafica> results = new List<LoteGrafica>();
    }
}
