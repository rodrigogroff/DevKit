using LinqToDB;
using SyCrafEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
    public class FechAssocAnalDetalhe
    {
        public string   serial, 
                        portador,
                        dtSolicitacao, 
                        codCred,
                        credenciado,
                        especialidade,
                        cnpj,
                        parcela,
                        vlr,
                        vlrCoPart,
                        tuss;
    }

    public class FechAssocAnalitico
    {
        public long     totVlr = 0, 
                        totCoPart = 0;

        public string   secao,
                        matricula,
                        nome,
                        cpf,
                        stotVlr,
                        stotCoPart, 
                        tuss;

        public List<FechAssocAnalDetalhe> results = new List<FechAssocAnalDetalhe>();
    }

    public class EmissorFechamentoAssocAnaliticoReport
    {
        public bool failed = false;

        public long totCreds = 0,
                    totAssociados = 0,
                    totVlr = 0,
                    totCoPart = 0;            

        public string stotVlr, 
                      stotCoPart,
                      mesAno;

        public List<FechAssocAnalitico> results = new List<FechAssocAnalitico>();
    }

    public partial class Empresa
    {
		public EmissorFechamentoAssocAnaliticoReport ListagemFechamento_AssocAnalitico ( DevKitDB db, ListagemFechamentoFilter filter )
		{
            var ret = new EmissorFechamentoAssocAnaliticoReport();
            
            var query = from e in db.Associado                        
                        where e.fkEmpresa == db.currentUser.fkEmpresa
                        orderby e.stName
                        select e;

            LoaderAssocAnalitico(db, query.ToList(), filter, ref ret);

            return ret;
		}

        public void LoaderAssocAnalitico ( DevKitDB db, 
                                          List<Associado> lst,
                                          ListagemFechamentoFilter filter,
                                          ref EmissorFechamentoAssocAnaliticoReport resultado )
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

            var lstIds = auts.Select(y => y.fkCredenciado).Distinct().ToList();
            var lstCreds = db.Credenciado.Where(y => lstIds.Contains(y.id)).ToList();

            var lstEspecialidade = db.Especialidade.ToList();

            var lstSecao = db.EmpresaSecao.Where(y => y.fkEmpresa == db.currentUser.fkEmpresa).ToList();

            resultado.totCreds = lstIds.Count();
            resultado.totAssociados = lst.Count();

            resultado.results = new List<FechAssocAnalitico>();

            foreach (var assocTb in lst)
            {
                if (auts.Any(y => y.fkAssociado == assocTb.id))
                {
                    var secao = lstSecao.Where(y => y.id == assocTb.fkSecao).FirstOrDefault();

                    var resultAssoc = new FechAssocAnalitico
                    {
                        cpf = assocTb.stCPF,
                        matricula = assocTb.nuMatricula.ToString(),
                        nome = assocTb.stName,
                        secao = secao.nuEmpresa + " - " + secao.stDesc,
                        totCoPart = 0,
                        totVlr = 0
                    };

                    foreach (var aut in auts.Where(y => y.fkAssociado == assocTb.id).
                                             OrderBy(y => y.dtSolicitacao).
                                             ToList())
                    {
                        var cred = lstCreds.
                                    Where(y => y.id == aut.fkCredenciado).
                                    FirstOrDefault();
                        
                        var proc = procsTuus.Where(y => y.id == aut.fkProcedimento).FirstOrDefault();

                        var portador = db.Associado.Where(y => y.id == aut.fkAssociadoPortador).FirstOrDefault();

                        resultAssoc.results.Add(new FechAssocAnalDetalhe
                        {
                            serial = serial.ToString(),
                            portador = portador != null ? portador.stName : "",
                            cnpj = cred.stCnpj,
                            codCred = cred.nuCodigo.ToString(),
                            credenciado = cred.stNome,
                            especialidade = lstEspecialidade.Where (y=> y.id == cred.fkEspecialidade).FirstOrDefault().stNome,
                            parcela = aut.nuIndice + " / " + aut.nuTotParcelas,
                            dtSolicitacao = Convert.ToDateTime(aut.dtSolicitacao).ToString("dd/MM/yyyy hh:mm"),                            
                            vlr = mon.setMoneyFormat((long)aut.vrParcela),
                            vlrCoPart = mon.setMoneyFormat((long)aut.vrParcelaCoPart),
                            tuss = proc.nuCodTUSS + " - " + proc.stProcedimento
                        });

                        resultAssoc.totVlr += (long)aut.vrParcela;
                        resultAssoc.totCoPart += (long)aut.vrParcelaCoPart;
                    }

                    resultAssoc.stotVlr = mon.setMoneyFormat(resultAssoc.totVlr);
                    resultAssoc.stotCoPart = mon.setMoneyFormat(resultAssoc.totCoPart);

                    resultado.totVlr += resultAssoc.totVlr;
                    resultado.totCoPart += resultAssoc.totCoPart;
                    
                    resultado.results.Add(resultAssoc);
                }
            }

            resultado.stotVlr = mon.setMoneyFormat(resultado.totVlr);
            resultado.stotCoPart = mon.setMoneyFormat(resultado.totCoPart);
        }
    }
}
