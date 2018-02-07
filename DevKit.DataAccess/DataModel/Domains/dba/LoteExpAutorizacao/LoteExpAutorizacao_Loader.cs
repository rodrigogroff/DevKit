using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System;

namespace DataModel
{
	public partial class LoteExpAutorizacao
    {
		public List<LoteExpAutorizacaoDTO> Loader(DevKitDB db, List<string> results)
        {
            var ret = new List<LoteExpAutorizacaoDTO>();

            foreach (var item in results)
            {
                var nuMes = Convert.ToInt32(item.Substring(0, 2));
                var nuAno = Convert.ToInt32(item.Substring(2, 4));
                var fkEmpresa = Convert.ToInt64(item.Substring(6, 4));

                ret.Add(new LoteExpAutorizacaoDTO
                {
                    fkEmpresa = fkEmpresa.ToString(),
                    empresa = db.Empresa.Where(y => y.id == fkEmpresa).FirstOrDefault().nuEmpresa.ToString(),
                    nuMes = nuMes.ToString(),
                    nuAno = nuAno.ToString(),
                    qtdAuts = db.Autorizacao.Where(y => y.fkEmpresa == fkEmpresa && y.nuMes == nuMes && y.nuAno == nuAno).Count().ToString(),
                    qtdAssocs = db.Autorizacao.Where(y => y.fkEmpresa == fkEmpresa && y.nuMes == nuMes && y.nuAno == nuAno).Select (y=>y.fkAssociado).Distinct().Count().ToString(),                    
                });
            }

            return ret;
        }
    }
}
