using System.Collections.Generic;

namespace DataModel
{
    public class LoteExpAutorizacaoDTO
    {
        public string fkEmpresa,
                      empresa,
                      nuMes,
                      nuAno,
                      qtdAuts,
                      qtdAssocs;
    }

    public class LoteExpAutorizacaoReport
    {
        public int count = 0;

        public List<LoteExpAutorizacaoDTO> results = new List<LoteExpAutorizacaoDTO>();
    }
}
