using DevKit.DataAccess;
using LinqToDB;
using SyCrafEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
    public class PrecoMedicamentoFilter
    {
        public string codigo = "", desc = "";

        public int skip, take;
    }

    public class SaudeValorMedicamentoReport
    {
        public int count = 0;

        public List<SaudeValorMedicamento> results = new List<SaudeValorMedicamento>();
    }

    public partial class SaudeValorMedicamento
    {
        public string sfkFabricanteMedicamento,
                      sfkUnidade,
                      svrFracao,
                      sbFracionar,
                      svrValor;

        public SaudeValorMedicamentoReport Listagem(DevKitDB db, PrecoMedicamentoFilter filter)
        {
            var ret = new SaudeValorMedicamentoReport();

            var query = from e in db.SaudeValorMedicamento
                        where e.fkEmpresa == db.currentUser.fkEmpresa
                        select e;

            if (!string.IsNullOrEmpty(filter.codigo))
                query = query.Where(y => y.nuCodInterno.ToString() == filter.codigo);

            if (!string.IsNullOrEmpty(filter.desc))
                query = query.Where(y => y.stDesc.ToUpper() == filter.desc.ToUpper());

            ret.count = query.Count();
            ret.results = (query.Skip(filter.skip).Take(filter.take)).ToList();

            foreach (var item in ret.results)
            {
                item.LoadAssociations(db);
            }

            return ret;
        }

        public void LoadAssociations(DevKitDB db)
        {
            var mon = new money();

            if (fkFabricanteMedicamento != null)
                sfkFabricanteMedicamento = db.SaudeFabricanteMedicamentoEmpresa.FirstOrDefault(y => y.id == this.id).stNome;

            if (fkUnidade != null)
                sfkUnidade = db.SaudeUnidadeEmpresa.FirstOrDefault(y => y.id == this.id).stNome;

            if (bFracionar == true) sbFracionar = "Sim"; else sbFracionar = "Não";

            if (vrFracao != null)
                svrFracao = mon.setMoneyFormat((long)vrFracao);

            if (vrValor != null)
                svrValor = mon.setMoneyFormat((long)vrValor);
        }
    }
}
