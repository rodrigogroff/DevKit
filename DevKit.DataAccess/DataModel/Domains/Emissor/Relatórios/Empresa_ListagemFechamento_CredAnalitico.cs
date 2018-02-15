using DevKit.DataAccess;
using LinqToDB;
using SyCrafEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
    public class FechCredAnalDetalhe
    {
        public string   serial, 
                        dtSolicitacao, 
                        cpf,
                        matricula,
                        associado,
                        vlr,
                        vlrCoPart,
                        tuss;
    }

    public class FechCredAnalitico
    {
        public long     totVlr = 0, 
                        totCoPart = 0;

        public string   codCredenciado,
                        nomeCredenciado,
                        cnpj;

        public List<FechCredAnalDetalhe> results = new List<FechCredAnalDetalhe>();
    }

    public class EmissorFechamentoCredAnaliticoReport
    {
        public bool failed = false;

        public long     totCreds = 0,
                        totAssociados = 0,
                        totVlr = 0,
                        totCoPart = 0;

        public string   stotVlr, 
                        stotCoPart;

        public List<FechCredAnalitico> results = new List<FechCredAnalitico>();
    }

    public partial class Empresa
    {
		public EmissorFechamentoCredAnaliticoReport ListagemFechamento_CredAnalitico ( DevKitDB db, ListagemFechamentoFilter filter )
		{
            var ret = new EmissorFechamentoCredAnaliticoReport();
            
            var query = from e in db.Credenciado
                        join ce in db.CredenciadoEmpresa on e.id equals ce.fkCredenciado
                        where ce.fkEmpresa == db.currentUser.fkEmpresa
                        orderby e.stNome
                        select e;

            LoaderCredAnalitico(db, query.ToList(), filter, ref ret);

            return ret;
		}

        public void LoaderCredAnalitico ( DevKitDB db, 
                                          List<Credenciado> lst,
                                          ListagemFechamentoFilter filter,
                                          ref EmissorFechamentoCredAnaliticoReport resultado )
        {
            var auts = db.Autorizacao.
                        Where(y => y.fkEmpresa == db.currentUser.fkEmpresa).
                        Where(y => y.nuMes == filter.mes).
                        Where(y => y.nuAno == filter.ano).
                        ToList();

            resultado.failed = !auts.Any();

            if (resultado.failed)
                return;

            var mon = new money();

            int serial = 1;

            var procsCredTuus = db.CredenciadoEmpresaTuss.Where(y => y.fkEmpresa == db.currentUser.fkEmpresa).ToList();
            var procsTuus = db.TUSS.ToList();
            var lstEspecs = db.Especialidade.ToList();

            var lstIdsAssocs = auts.Select(y => y.fkAssociado).Distinct().ToList();
            var lstMats = db.Associado.Where(y => lstIdsAssocs.Contains(y.id)).Select (y=>y.nuMatricula).ToList();
            var lstAssocs = db.Associado.Where(y => lstMats.Contains(y.nuMatricula)).ToList();

            

            resultado.totCreds = auts.Select(y => y.fkCredenciado).Distinct().Count();
            resultado.totAssociados = auts.Select(y => y.fkAssociado).Distinct().Count();

            resultado.results = new List<FechCredAnalitico>();

            foreach (var cred in lst)
            {
                if (auts.Any(y => y.fkCredenciado == cred.id))
                {
                    var resultCred = new FechCredAnalitico
                    {
                        cnpj = cred.stCnpj,
                        codCredenciado = cred.nuCodigo.ToString(),
                        nomeCredenciado = cred.stNome
                    };

                    foreach (var aut in auts.Where(y => y.fkCredenciado == cred.id).
                                             OrderBy(y => y.dtSolicitacao).
                                             ToList())
                    {
                        var assoc = lstAssocs.
                                    Where(y => y.id == aut.fkAssociado).
                                    FirstOrDefault();

                        resultCred.results.Add(new FechCredAnalDetalhe
                        {
                            serial = serial.ToString(),
                            associado = assoc.stName,
                            dtSolicitacao = Convert.ToDateTime(aut.dtSolicitacao).ToString("dd/MM/yyyy hh:mm"),
                            matricula = assoc.nuMatricula.ToString()
                        });
                    }

                    resultado.results.Add(resultCred);
                }
            }

            resultado.stotVlr = mon.setMoneyFormat(resultado.totVlr);
            resultado.stotCoPart = mon.setMoneyFormat(resultado.totCoPart);
        }
    }
}
