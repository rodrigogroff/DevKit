using DevKit.DataAccess;
using LinqToDB;
using SyCrafEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
    public class ListagemFechamentoFilter
    {
        public long? fkEmpresa;

        public int mes, 
                   ano,
                   tipo,
                   modo;
    }

    public class FechCredSint
    {
        public string serial,
                        cpfcnpj,
                        codigoCred,
                        nomeCred,
                        especialidade,
                        qtdAutos,
                        vlrAutos,
                        vlrCoPart,
                        pcads,
                        ncads;
    }

    public class EmissorFechamentoCredSintReport
    {
        public bool failed = false;

        public long totCreds = 0,
                    totVlr = 0,
                    totCoPart = 0,
                    totAssociados = 0,
                    procsNCad = 0;

        public string stotVlr, stotCoPart;

        public List<FechCredSint> results = new List<FechCredSint>();
    }

    public partial class Empresa
    {
		public EmissorFechamentoCredSintReport ListagemFechamento_CredSint ( DevKitDB db, ListagemFechamentoFilter filter )
		{
            var ret = new EmissorFechamentoCredSintReport();
            
            var query = from e in db.Credenciado
                        join ce in db.CredenciadoEmpresa on e.id equals ce.fkCredenciado
                        where ce.fkEmpresa == db.currentUser.fkEmpresa
                        orderby e.stNome
                        select e;

            LoaderCredSint(db, query.ToList(), filter, ref ret );
            
            return ret;
		}

        public void LoaderCredSint ( DevKitDB db, 
                                     List<Credenciado> lst,
                                     ListagemFechamentoFilter filter,
                                     ref EmissorFechamentoCredSintReport resultado )
        {
            var auts = db.Autorizacao.
                        Where(y => y.fkEmpresa == db.currentUser.fkEmpresa).
                        Where(y => y.nuMes == filter.mes).
                        Where(y => y.nuAno == filter.ano).
                        ToList();

            resultado.failed = !auts.Any();

            if (resultado.failed)
                return;

            var procsCredTuus = db.CredenciadoEmpresaTuss.
                                    Where(y => y.fkEmpresa == db.currentUser.fkEmpresa).
                                    ToList();

            var procsTuus = db.TUSS.ToList();

            var lstEspecs = db.Especialidade.ToList();

            int serial = 1;

            var mon = new money();

            resultado.totCreds = auts.Select(y => y.fkCredenciado).Distinct().Count();
            resultado.totAssociados = auts.Select(y => y.fkAssociado).Distinct().Count();

            resultado.results = new List<FechCredSint>();

            foreach (var cred in lst)
            {
                var item = new FechCredSint
                {
                    serial = serial.ToString(),
                    cpfcnpj = cred.stCnpj,
                    codigoCred = cred.nuCodigo.ToString(),
                    nomeCred = cred.stNome,
                    especialidade = lstEspecs.Where (y=> y.id == cred.fkEspecialidade).FirstOrDefault().stNome,
                    qtdAutos = auts.Where(y => y.fkCredenciado == cred.id).Count().ToString()
                };

                long totVlr = 0, totCoPart = 0;

                bool found = false;

                item.pcads = "";
                item.ncads = "";
                
                foreach (var aut in auts.Where(y => y.fkCredenciado == cred.id).ToList())
                {
                    found = true;

                    var fkProc = procsTuus.
                                    Where(y => y.id == aut.fkProcedimento).
                                    FirstOrDefault();

                    if (fkProc != null)
                    {
                        var strTUSs = fkProc.nuCodTUSS.ToString();

                        var cfgTuss = procsCredTuus.
                                        Where(y => y.fkCredenciado == cred.id).
                                        Where(y => y.nuTUSS.ToString() == strTUSs).
                                        FirstOrDefault();

                        if (cfgTuss != null)
                        {
                            if (!item.pcads.Contains(strTUSs))
                                item.pcads += strTUSs + ", ";
                            totVlr += (long)cfgTuss.vrProcedimento;
                            totCoPart += (long)cfgTuss.vrCoPart;
                        }
                        else
                        {
                            if (!item.ncads.Contains(strTUSs))
                                item.ncads += strTUSs + ", ";
                            resultado.procsNCad++;
                        }                            
                    }
                }

                if (found)
                {
                    item.pcads = item.pcads.Trim().TrimEnd(',');
                    item.ncads = item.ncads.Trim().TrimEnd(',');

                    resultado.totVlr += totVlr;
                    resultado.totCoPart += totCoPart;

                    item.vlrAutos = mon.setMoneyFormat(totVlr);
                    item.vlrCoPart = mon.setMoneyFormat(totCoPart);

                    resultado.results.Add(item);
                    serial++;
                }
            }

            resultado.stotVlr = mon.setMoneyFormat(resultado.totVlr);
            resultado.stotCoPart = mon.setMoneyFormat(resultado.totCoPart);
        }
    }
}
