using DevKit.DataAccess;
using LinqToDB;
using SyCrafEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
    public class FechAssocSint
    {
        public string serial,
                        matricula,
                        associado,
                        qtdAutos,
                        vlrAutos,
                        vlrCoPart,
                        ncads;
    }

    public class EmissorFechamentoAssocSintReport
    {
        public long totCreds = 0,
                    totAssocs = 0,
                    totVlr = 0,
                    totCoPart = 0,
                    procsNCad = 0;

        public string stotVlr, stotCoPart;

        public List<FechAssocSint> results = new List<FechAssocSint>();
    }

    public partial class Empresa
    {
		public EmissorFechamentoAssocSintReport ListagemFechamento_AssocSint ( DevKitDB db, ListagemFechamentoFilter filter )
		{
            var ret = new EmissorFechamentoAssocSintReport();
            
            var query = from e in db.Associado
                        where e.fkEmpresa == db.currentUser.fkEmpresa
                        where e.nuTitularidade == 1
                        orderby e.stName
                        select e;

            LoaderAssocSint(db, query.ToList(), filter, ref ret );
            
            return ret;
		}

        public void LoaderAssocSint ( DevKitDB db, 
                                      List<Associado> lst,
                                      ListagemFechamentoFilter filter,
                                      ref EmissorFechamentoAssocSintReport resultado )
        {
            var auts = db.Autorizacao.
                        Where(y => y.fkEmpresa == db.currentUser.fkEmpresa).
                        Where(y => y.nuMes == filter.mes).
                        Where(y => y.nuAno == filter.ano).
                        ToList();

            var lstTitDeps = db.Associado.
                                Where(y => y.fkEmpresa == db.currentUser.fkEmpresa).
                                ToList();

            var procsCredTuus = db.CredenciadoEmpresaTuss.
                                    Where(y => y.fkEmpresa == db.currentUser.fkEmpresa).
                                    ToList();

            var procsTuus = db.TUSS.ToList();

            int serial = 1;

            var mon = new money();

            resultado.totCreds = auts.Select(y => y.fkCredenciado).Distinct().Count();
            resultado.totAssocs = auts.Select(y => y.fkAssociado).Distinct().Count();

            resultado.results = new List<FechAssocSint>();

            foreach (var assoc in lst)
            {
                var tLstTitDeps = lstTitDeps.
                                    Where(y => y.nuMatricula == assoc.nuMatricula).
                                    Select (y=> y.id).
                                    ToList();

                var item = new FechAssocSint
                {
                    serial = serial.ToString(),
                    matricula = assoc.nuMatricula.ToString(),
                    associado = assoc.stName,
                    qtdAutos = auts.Where(y => tLstTitDeps.Contains((long)y.fkAssociado)).Count().ToString()
                };

                long totVlr = 0, totCoPart = 0;

                item.ncads = "";

                foreach (var aut in auts.
                                    Where (y=> tLstTitDeps.Contains((long)y.fkAssociado)).
                                    ToList())
                {
                    var fkProc = procsTuus.
                                    Where(y => y.id == aut.fkProcedimento).
                                    FirstOrDefault();

                    if (fkProc != null)
                    {
                        var cfgTuss = procsCredTuus.
                                        Where(y => y.fkCredenciado == aut.fkCredenciado).
                                        Where(y => y.nuTUSS == fkProc.nuCodTUSS).
                                        FirstOrDefault();

                        item.ncads += fkProc.nuCodTUSS + ", ";

                        if (cfgTuss != null)
                        {
                            totVlr += (long)cfgTuss.vrProcedimento;
                            totCoPart += (long)cfgTuss.vrCoPart;
                        }
                        else
                            resultado.procsNCad++;
                    }
                }

                item.ncads = item.ncads.Trim().TrimEnd(',');

                resultado.totVlr += totVlr;
                resultado.totCoPart += totCoPart;

                item.vlrAutos = mon.setMoneyFormat(totVlr);
                item.vlrCoPart = mon.setMoneyFormat(totCoPart);

                resultado.results.Add(item);
                serial++;
            }

            resultado.stotVlr = mon.setMoneyFormat(resultado.totVlr);
            resultado.stotCoPart = mon.setMoneyFormat(resultado.totCoPart);
        }
    }
}
