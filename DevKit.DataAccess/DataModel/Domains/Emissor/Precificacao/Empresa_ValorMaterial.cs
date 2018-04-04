using DevKit.DataAccess;
using LinqToDB;
using SyCrafEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
    public class PrecoMaterialFilter
    {
        public string codigo = "";

        public int skip, take;
    }

    public class SaudeValorMaterialReport
    {
        public int count = 0;

        public List<SaudeValorMaterial> results = new List<SaudeValorMaterial>();
    }

    public partial class SaudeValorMaterial
    {
        public string sfkFabricanteMaterial,
                      sfkUnidade,
                      svrFracao,
                      sbFracionar,
                      svrValor;

        public SaudeValorMaterialReport Listagem(DevKitDB db, PrecoMaterialFilter filter)
        {
            var ret = new SaudeValorMaterialReport();

            var query = from e in db.SaudeValorMaterial
                        where e.fkEmpresa == db.currentUser.fkEmpresa
                        select e;

            ret.count = query.Count();
            ret.results = query.Skip(filter.skip).Take(filter.take).ToList();

            foreach (var item in ret.results)
            {
                item.LoadAssociations(db);
            }

            return ret;
        }

        public void LoadAssociations(DevKitDB db)
        {
            var mon = new money();

            if (fkFabricanteMaterial != null)
                sfkFabricanteMaterial = db.SaudeFabricanteMaterialEmpresa.FirstOrDefault(y => y.id == this.id).stNome;

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
