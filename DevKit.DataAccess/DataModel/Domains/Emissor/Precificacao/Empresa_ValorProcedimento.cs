using DevKit.DataAccess;
using LinqToDB;
using SyCrafEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
    public class PrecoProcedimentoFilter
    {
        public string codigo = "";

        public int skip, take;
    }

    public class SaudeValorProcedimentoReport
    {
        public int count = 0;

        public List<SaudeValorProcedimento> results = new List<SaudeValorProcedimento>();
    }

    public partial class SaudeValorProcedimento
    {
        public string svrTotalHMCO,
                      sfkSaudePorteProcedimento, 
                      svrValorHM,
                      svrValorCO,
                      snuAux,
                      snuAnestesistas,
                      svrPorteAnestesista,
                      snuFilme4C,
                      svrFilme;

        public SaudeValorProcedimentoReport Listagem(DevKitDB db, PrecoProcedimentoFilter filter)
        {
            var ret = new SaudeValorProcedimentoReport();

            var query = from e in db.SaudeValorProcedimento
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

            if (vrTotalHMCO != null)
                svrTotalHMCO = mon.setMoneyFormat((long)vrTotalHMCO);

            if (fkSaudePorteProcedimento != null)
                sfkSaudePorteProcedimento = db.SaudePorteProcedimentoEmpresa.FirstOrDefault(y=>y.id == fkSaudePorteProcedimento).stNome;

            if (vrValorHM != null)
                svrValorHM = mon.setMoneyFormat((long)vrValorHM);

            if (vrValorCO != null)
                svrValorCO = mon.setMoneyFormat((long)vrValorCO);

            if (nuAux != null)
                snuAux = nuAux.ToString();

            if (nuAnestesistas != null)
                snuAnestesistas = nuAnestesistas.ToString();

            if (vrPorteAnestesista != null)
                svrPorteAnestesista = mon.setMoneyFormat((long)vrPorteAnestesista);

            if (nuFilme4C != null)
            {
                snuFilme4C = nuFilme4C.ToString();

                if (snuFilme4C.Length >= 5)
                    snuFilme4C = snuFilme4C.Insert(snuFilme4C.Length - 4, ",");
            }

            if (vrFilme != null)
                svrFilme = mon.setMoneyFormat((long)vrFilme);
        }
    }
}
