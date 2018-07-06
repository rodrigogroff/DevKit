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
                        nsu,
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

    public class FechCredAnalDetalheExtra
    {
        public string serial,
                        nsu,
                        portador,
                        cpf,                        
                        dtSolicitacao,                        
                        vlr,
                        parcela,
                        tipo,
                        desc;
    }

    public class FechCredAnalitico
    {
        public long     totVlr = 0, 
                        totCoPart = 0,
                        totExtra_diaria = 0,
                        totExtra_mat = 0,
                        totExtra_meds = 0,
                        totExtra_naomed = 0,
                        totExtra_opme = 0,
                        totExtra_pacserv = 0;

        public string codCredenciado,
                        nomeCredenciado,
                        cnpj,
                        stotVlr,
                        stotCoPart,
                        sDadosBancarios,
                        tuss,
                        stotExtra_diaria,
                        stotExtra_mat,
                        stotExtra_meds,
                        stotExtra_naomed,
                        stotExtra_opme,
                        stotExtra_pacserv;

        public List<FechCredAnalDetalhe> results = new List<FechCredAnalDetalhe>();
        public List<FechCredAnalDetalheExtra> resultsExtras = new List<FechCredAnalDetalheExtra>();
    }

    public class EmissorFechamentoCredAnaliticoReport
    {
        public bool failed = false;

        public long totCreds = 0,
                        totAssociados = 0,
                        totVlr = 0,
                        totCoPart = 0,
                        totExtra_diaria = 0,
                        totExtra_mat = 0,
                        totExtra_meds = 0,
                        totExtra_naomed = 0,
                        totExtra_opme = 0,
                        totExtra_pacserv = 0;

        public string   stotVlr, 
                        stotCoPart,
                        mesAno,
                        
                        stotExtra_diaria,
                        stotExtra_mat,
                        stotExtra_meds,
                        stotExtra_naomed,
                        stotExtra_opme,
                        stotExtra_pacserv;

        public List<FechCredAnalitico> results = new List<FechCredAnalitico>();
    }

    public partial class Empresa
    {
		public EmissorFechamentoCredAnaliticoReport ListagemFechamento_CredAnalitico ( DevKitDB db, ListagemFechamentoFilter filter )
		{
            var ret = new EmissorFechamentoCredAnaliticoReport();
            
            var query = from e in db.Credenciado
                        join ce in db.CredenciadoEmpresa on e.id equals ce.fkCredenciado
                        join es in db.EmpresaSecao on ce.fkEmpresa equals es.fkEmpresa
                        where filter.fkEmpresa == null || (filter.fkEmpresa != null && ce.fkEmpresa == filter.fkEmpresa)
                        where filter.fkCredenciado == null || (filter.fkCredenciado != null && e.id == filter.fkCredenciado)
                        where es.id == filter.fkSecao
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
                        Where(y => y.tgSituacao == filter.tgSituacao).
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
                                             Where (y=> y.nuTipoAutorizacao == 1 || y.nuTipoAutorizacao == null).
                                             OrderBy(y => y.dtSolicitacao).
                                             ToList())
                    {
                        var assoc = lstAssocs.
                                    Where(y => y.id == aut.fkAssociadoPortador).
                                    FirstOrDefault();

                        if (assoc == null)
                            assoc = lstAssocs.
                                    Where(y => y.id == aut.fkAssociado).
                                    FirstOrDefault();

                        var secao = secoes.Where(y => y.id == assoc.fkSecao).FirstOrDefault();
                        var proc = procsTuus.Where(y => y.id == aut.fkProcedimento).FirstOrDefault();

                        resultCred.results.Add(new FechCredAnalDetalhe
                        {
                            serial = serial.ToString(),
                            nsu = aut.nuNSU != null ? aut.nuNSU.ToString() : "",
                            associado = assoc.stName,
                            cpf = assoc.stCPF,
                            secao = secao.nuEmpresa + " - " + secao.stDesc,
                            parcela = aut.nuIndice + " / " + aut.nuTotParcelas,
                            dtSolicitacao = Convert.ToDateTime(aut.dtSolicitacao).ToString("dd/MM/yyyy hh:mm"),
                            matricula = assoc.nuMatricula.ToString(),
                            vlr = mon.setMoneyFormat((long)aut.vrParcela),
                            vlrCoPart = mon.setMoneyFormat((long)aut.vrParcelaCoPart),
                            tuss = proc != null ? proc.nuCodTUSS + " - " + proc.stProcedimento : ""
                        });

                        resultCred.totVlr += (long)aut.vrParcela;
                        resultCred.totCoPart += (long)aut.vrParcelaCoPart;
                    }

                    #region - code - 

                    foreach (var aut in auts.
                                        Where(y => y.fkCredenciado == cred.id).
                                        Where(y => y.nuTipoAutorizacao > 1).
                                        OrderBy(y => y.dtSolicitacao).
                                        ToList())
                    {
                        var portador = db.Associado.Where(y => y.id == aut.fkAssociadoPortador).FirstOrDefault();

                        var e = new FechCredAnalDetalheExtra
                        {
                            serial = serial.ToString(),
                            nsu = aut.nuNSU != null ? aut.nuNSU.ToString() : "",
                            portador = portador != null ? portador.stName : "",
                            cpf = portador.stCPF,
                            parcela = aut.nuIndice + " / " + aut.nuTotParcelas,
                            dtSolicitacao = Convert.ToDateTime(aut.dtSolicitacao).ToString("dd/MM/yyyy hh:mm"),
                            vlr = mon.setMoneyFormat((long)aut.vrParcela),
                        };

                        switch (aut.nuTipoAutorizacao)
                        {
                            case 2: e.tipo = "Diária"; break;
                            case 3: e.tipo = "Materiais"; break;
                            case 4: e.tipo = "Medicamentos"; break;
                            case 5: e.tipo = "Não médicos"; break;
                            case 6: e.tipo = "OPME"; break;
                            case 7: e.tipo = "Pacote Serviços"; break;
                        }

                        switch (aut.nuTipoAutorizacao)
                        {
                            case 2: e.desc = db.SaudeValorDiaria.Where(y => y.id == aut.fkPrecificacao).Select(y => y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                            case 3: e.desc = db.SaudeValorMaterial.Where(y => y.id == aut.fkPrecificacao).Select(y => y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                            case 4: e.desc = db.SaudeValorMedicamento.Where(y => y.id == aut.fkPrecificacao).Select(y => y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                            case 5: e.desc = db.SaudeValorNaoMedico.Where(y => y.id == aut.fkPrecificacao).Select(y => y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                            case 6: e.desc = db.SaudeValorOPME.Where(y => y.id == aut.fkPrecificacao).Select(y => y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                            case 7: e.desc = db.SaudeValorPacote.Where(y => y.id == aut.fkPrecificacao).Select(y => y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                        }

                        resultCred.totVlr += (long)aut.vrParcela;

                        switch (aut.nuTipoAutorizacao)
                        {
                            case 2: resultCred.totExtra_diaria += (long)aut.vrParcela; break;
                            case 3: resultCred.totExtra_mat += (long)aut.vrParcela; break;
                            case 4: resultCred.totExtra_meds += (long)aut.vrParcela; break;
                            case 5: resultCred.totExtra_naomed += (long)aut.vrParcela; break;
                            case 6: resultCred.totExtra_opme += (long)aut.vrParcela; break;
                            case 7: resultCred.totExtra_pacserv += (long)aut.vrParcela; break;
                        }

                        resultCred.resultsExtras.Add(e);
                    }

                    #endregion

                    resultCred.stotVlr = mon.setMoneyFormat(resultCred.totVlr);
                    resultCred.stotCoPart = mon.setMoneyFormat(resultCred.totCoPart);

                    resultCred.stotExtra_diaria = mon.setMoneyFormat(resultCred.totExtra_diaria);
                    resultCred.stotExtra_mat = mon.setMoneyFormat(resultCred.totExtra_mat);
                    resultCred.stotExtra_meds = mon.setMoneyFormat(resultCred.totExtra_meds);
                    resultCred.stotExtra_naomed = mon.setMoneyFormat(resultCred.totExtra_naomed);
                    resultCred.stotExtra_opme = mon.setMoneyFormat(resultCred.totExtra_opme);
                    resultCred.stotExtra_pacserv = mon.setMoneyFormat(resultCred.totExtra_pacserv);

                    resultado.totVlr += resultCred.totVlr;
                    resultado.totCoPart += resultCred.totCoPart;

                    resultado.totExtra_diaria += resultCred.totExtra_diaria;
                    resultado.totExtra_mat += resultCred.totExtra_mat;
                    resultado.totExtra_meds += resultCred.totExtra_meds;
                    resultado.totExtra_naomed += resultCred.totExtra_naomed;
                    resultado.totExtra_opme += resultCred.totExtra_opme;
                    resultado.totExtra_pacserv += resultCred.totExtra_pacserv;

                    resultado.results.Add(resultCred);
                }
            }

            resultado.stotVlr = mon.setMoneyFormat(resultado.totVlr);
            resultado.stotCoPart = mon.setMoneyFormat(resultado.totCoPart);

            resultado.stotExtra_diaria = mon.setMoneyFormat(resultado.totExtra_diaria);
            resultado.stotExtra_mat = mon.setMoneyFormat(resultado.totExtra_mat);
            resultado.stotExtra_meds = mon.setMoneyFormat(resultado.totExtra_meds);
            resultado.stotExtra_naomed = mon.setMoneyFormat(resultado.totExtra_naomed);
            resultado.stotExtra_opme = mon.setMoneyFormat(resultado.totExtra_opme);
            resultado.stotExtra_pacserv = mon.setMoneyFormat(resultado.totExtra_pacserv);
        }
    }
}
