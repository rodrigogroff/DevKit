using DevKit.DataAccess;
using LinqToDB;
using SyCrafEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
    public class PrecoNaoMedicoFilter
    {
        public string codigo = "";

        public int skip, take;
    }

    public class SaudeValorNaoMedicoReport
    {
        public int count = 0;

        public List<SaudeValorNaoMedico> results = new List<SaudeValorNaoMedico>();
    }

    public partial class SaudeValorNaoMedico
    {
        public string svrValor;

        public SaudeValorNaoMedicoReport Listagem(DevKitDB db, PrecoNaoMedicoFilter filter)
        {
            var ret = new SaudeValorNaoMedicoReport();

            var query = from e in db.SaudeValorNaoMedico
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

            if (vrValor != null)
                svrValor = mon.setMoneyFormat((long)vrValor);
        }
    }
}
