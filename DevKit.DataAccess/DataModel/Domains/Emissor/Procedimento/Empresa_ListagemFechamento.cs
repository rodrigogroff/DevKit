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
                        qtdAutos,
                        vlrAutos, 
                        vlrCoPart;
    }

    public class EmissorFechamentoCredSintReport
    {
        public int count = 0;
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
                        select e;
            
            return new EmissorFechamentoCredSintReport
            {
                count = query.Count(),
                results = LoaderCredSint(db, query.ToList(), filter)
            };
		}

        public List<FechCredSint> LoaderCredSint ( DevKitDB db, 
                                                   List<Credenciado> lst,
                                                   ListagemFechamentoFilter filter)
        {
            var results = new List<FechCredSint>();

            var auts = db.Autorizacao.
                        Where(y => y.fkEmpresa == db.currentUser.fkEmpresa).
                        Where(y => y.nuMes == filter.mes).
                        Where(y => y.nuAno == filter.ano).
                        ToList();

            var procsCredTuus = db.CredenciadoEmpresaTuss.
                                    Where(y => y.fkEmpresa == db.currentUser.fkEmpresa).
                                    ToList();

            var procsTuus = db.TUSS.ToList();

            int serial = 1;

            var mon = new money();

            foreach (var cred in lst)
            {
                var item = new FechCredSint
                {
                    serial = serial.ToString(),
                    cpfcnpj = cred.stCnpj,
                    codigoCred = cred.nuCodigo.ToString(),
                    nomeCred = cred.stNome,
                    qtdAutos = auts.Where(y => y.fkCredenciado == cred.id).Count().ToString()
                };

                long totVlr = 0, totCoPart = 0;

                foreach (var aut in auts.
                                    Where (y=> y.fkCredenciado == cred.id ).
                                    ToList())
                {
                    var fkProc = procsTuus.
                                    Where(y => y.id == aut.fkProcedimento).
                                    FirstOrDefault();

                    if (fkProc != null)
                    {
                        var cfgTuss = procsCredTuus.
                                        Where(y => y.fkCredenciado == cred.id).
                                        Where(y => y.nuTUSS == fkProc.nuCodTUSS).
                                        FirstOrDefault();

                        if (cfgTuss != null)
                        {
                            totVlr += (long)cfgTuss.vrProcedimento;
                            totCoPart += (long)cfgTuss.vrCoPart;
                        }                        
                    }                    
                }

                item.vlrAutos = mon.setMoneyFormat(totVlr);
                item.vlrCoPart = mon.setMoneyFormat(totCoPart);

                results.Add(item);
                serial++;
            }

            return results;
        }
    }
}
