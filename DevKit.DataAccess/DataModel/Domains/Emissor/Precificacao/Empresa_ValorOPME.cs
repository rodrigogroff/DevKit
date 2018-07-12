using DevKit.DataAccess;
using LinqToDB;
using SyCrafEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
    public class PrecoOPMEFilter
    {
        public string codigo = "", desc = "";

        public int skip, take;
    }

    public class SaudeValorOPMEReport
    {
        public int count = 0;

        public List<SaudeValorOPME> results = new List<SaudeValorOPME>();
    }

    public partial class SaudeValorOPME
    {
        public string   sfkClassificacao,
                        sfkEspecialidade,
                        svrValor;

        public SaudeValorOPMEReport Listagem(DevKitDB db, PrecoOPMEFilter filter)
        {
            var ret = new SaudeValorOPMEReport();

            var query = from e in db.SaudeValorOPME
                        where e.fkEmpresa == db.currentUser.fkEmpresa
                        select e;

            if (!string.IsNullOrEmpty(filter.codigo))
                query = query.Where(y => y.nuCodInterno == Convert.ToInt64(filter.codigo));
            
            if (!string.IsNullOrEmpty(filter.desc))
                query = query.Where(y => y.stDesc.ToUpper().Contains(filter.desc.ToUpper()));

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

            if (fkClassificacao != null)
                sfkClassificacao = db.SaudeOPMEClassificacaoEmpresa.FirstOrDefault(y => y.id == fkClassificacao).stNome;

            if (fkEspecialidade != null)
                sfkEspecialidade = db.SaudeOPMEEspecialidadeEmpresa.FirstOrDefault(y => y.id == fkEspecialidade).stNome;

            if (vrValor != null)
                svrValor = mon.setMoneyFormat((long)vrValor);
        }
    }
}
