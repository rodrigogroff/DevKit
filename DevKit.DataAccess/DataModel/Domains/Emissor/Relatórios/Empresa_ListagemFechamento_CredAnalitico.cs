using LinqToDB;
using SyCrafEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
    public class FechCredAnalDetalhe
    {
        public string   serial, 
                        dtSolicitacao, 
                        cpf,
                        secao,
                        matricula,
                        associado,
                        parcela,
                        vlr,
                        vlrCoPart,
                        tuss;
    }

    public class FechCredAnalitico
    {
        public long     totVlr = 0, 
                        totCoPart = 0;

        public string codCredenciado,
                        nomeCredenciado,
                        cnpj,
                        stotVlr,
                        stotCoPart,
                        sDadosBancarios,
                        tuss;

        public List<FechCredAnalDetalhe> results = new List<FechCredAnalDetalhe>();
    }

    public class EmissorFechamentoCredAnaliticoReport
    {
        public bool failed = false;

        public long totCreds = 0,
                        totAssociados = 0,
                        totVlr = 0,
                        totCoPart = 0;            

        public string   stotVlr, 
                        stotCoPart,
                        mesAno;

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

            var secoes = db.EmpresaSecao.Where(y => y.fkEmpresa == db.currentUser.fkEmpresa).ToList();

            resultado.failed = !auts.Any();
            resultado.mesAno = filter.mes.ToString().PadLeft(2, '0') + " / " + filter.ano.ToString();
            
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
                        nomeCredenciado = cred.stNome,
                        totCoPart = 0,
                        totVlr = 0,
                        sDadosBancarios = "=> Banco: " + cred.stBanco + " Agência " + cred.stAgencia + " Conta " + cred.stConta
                    };

                    foreach (var aut in auts.Where(y => y.fkCredenciado == cred.id).
                                             OrderBy(y => y.dtSolicitacao).
                                             ToList())
                    {
                        var assoc = lstAssocs.
                                    Where(y => y.id == aut.fkAssociado).
                                    FirstOrDefault();

                        var secao = secoes.Where(y => y.id == assoc.fkSecao).FirstOrDefault();
                        var proc = procsTuus.Where(y => y.id == aut.fkProcedimento).FirstOrDefault();

                        resultCred.results.Add(new FechCredAnalDetalhe
                        {
                            serial = serial.ToString(),
                            associado = assoc.stName,
                            cpf = assoc.stCPF,
                            secao = secao.nuEmpresa + " - " + secao.stDesc,
                            parcela = aut.nuIndice + " / " + aut.nuTotParcelas,
                            dtSolicitacao = Convert.ToDateTime(aut.dtSolicitacao).ToString("dd/MM/yyyy hh:mm"),
                            matricula = assoc.nuMatricula.ToString(),
                            vlr = mon.setMoneyFormat((long)aut.vrParcela),
                            vlrCoPart = mon.setMoneyFormat((long)aut.vrParcelaCoPart),
                            tuss = proc.nuCodTUSS + " - " + proc.stProcedimento
                        });

                        resultCred.totVlr += (long)aut.vrParcela;
                        resultCred.totCoPart += (long)aut.vrParcelaCoPart;
                    }

                    resultCred.stotVlr = mon.setMoneyFormat(resultCred.totVlr);
                    resultCred.stotCoPart = mon.setMoneyFormat(resultCred.totCoPart);

                    resultado.totVlr += resultCred.totVlr;
                    resultado.totCoPart += resultCred.totCoPart;
                    
                    resultado.results.Add(resultCred);
                }
            }

            resultado.stotVlr = mon.setMoneyFormat(resultado.totVlr);
            resultado.stotCoPart = mon.setMoneyFormat(resultado.totCoPart);
        }
    }
}
