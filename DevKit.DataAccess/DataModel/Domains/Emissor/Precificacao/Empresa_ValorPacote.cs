using DevKit.DataAccess;
using LinqToDB;
using SyCrafEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
    public class PrecoPacoteFilter
    {
        public string codigo = "";

        public int skip, take;
    }

    public class SaudeValorPacoteReport
    {
        public int count = 0;

        public List<SaudeValorPacote> results = new List<SaudeValorPacote>();
    }

    public partial class SaudeValorPacote
    {
        public string svrValor;

        public SaudeValorPacoteReport Listagem(DevKitDB db, PrecoPacoteFilter filter)
        {
            var ret = new SaudeValorPacoteReport();

            var query = from e in db.SaudeValorPacote
                        where e.fkEmpresa == db.currentUser.fkEmpresa
                        select e;

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

            if (vrValor != null)
                svrValor = mon.setMoneyFormat((long)vrValor);
        }
    }
}
