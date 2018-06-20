using DevKit.DataAccess;
using LinqToDB;
using SyCrafEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
    public class PrecoDiariaFilter
    {
        public string codigo = "", desc = "";

        public int skip, take;
    }

    public class SaudeValorDiariaReport
    {
        public int count = 0;

        public List<SaudeValorDiaria> results = new List<SaudeValorDiaria>();
    }

    public partial class SaudeValorDiaria
    {
        public string svrNivel1 = "",
                        svrNivel2 = "",
                        svrNivel3 = "",
                        svrNivel4 = "",
                        svrNivel5 = "";

        public SaudeValorDiariaReport Listagem(DevKitDB db, PrecoDiariaFilter filter)
        {
            var ret = new SaudeValorDiariaReport();

            var query = from e in db.SaudeValorDiaria
                        where e.fkEmpresa == db.currentUser.fkEmpresa
                        select e;

            if (!string.IsNullOrEmpty(filter.codigo))
                query = query.Where(y => y.nuCodInterno.ToString() == filter.codigo);

            if (!string.IsNullOrEmpty(filter.desc))
                query = query.Where(y => y.stDesc.ToUpper().Contains(filter.desc.ToUpper()));

            ret.count = query.Count();
            ret.results = (query.Skip(filter.skip).Take(filter.take)).ToList();

            foreach (var item in ret.results)
            {
                item.LoadAssociations();
            }

            return ret;
        }

        public void LoadAssociations()
        {
            var mon = new money();

            if (this.vrNivel1 != null)
                svrNivel1 = mon.setMoneyFormat((long)this.vrNivel1);

            if (this.vrNivel2 != null)
                svrNivel2 = mon.setMoneyFormat((long)this.vrNivel2);

            if (this.vrNivel3 != null)
                svrNivel3 = mon.setMoneyFormat((long)this.vrNivel3);

            if (this.vrNivel4 != null)
                svrNivel4 = mon.setMoneyFormat((long)this.vrNivel4);

            if (this.vrNivel5 != null)
                svrNivel5 = mon.setMoneyFormat((long)this.vrNivel5);
        }
    }
}
