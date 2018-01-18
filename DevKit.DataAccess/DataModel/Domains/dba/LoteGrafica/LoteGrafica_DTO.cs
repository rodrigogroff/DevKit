using System.Collections.Generic;

namespace DataModel
{
	public partial class LoteGrafica
    {
        public object anexedEntity;

		public string   sdtLog = "",
                        sqtdCartoes = "",
                        stgAtivo = "",
						updateCommand = "";

        public List<Empresa> empresas = new List<Empresa>();
	}

    public class LoteGraficaReport
    {
        public int count = 0;
        public List<LoteGrafica> results = new List<LoteGrafica>();
    }

    // ------------

    public partial class NovoLoteGraficaDTO
    {
        public string id,empresa, qtdCartoes;
        public bool selecionado;
    }

    public class NovoLoteGraficaReport
    {
        public int count = 0;
        public List<NovoLoteGraficaDTO> results = new List<NovoLoteGraficaDTO>();
    }
}
