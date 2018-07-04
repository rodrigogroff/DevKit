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
                        nsu,
                        portador,
                        dtSolicitacao, 
                        codCred,
                        credenciado,
                        especialidade,
                        cnpj,
                        parcela,
                        vlr,
                        vlrConsulta,
                        vlrCoPart,
                        tuss;
    }

    public class FechAssocAnalDetalheExtra
    {
        public string   serial,
                        nsu,
                        portador,
                        cnpj,
                        credenciado,
                        dtSolicitacao,
                        codCred,
                        vlr,
                        parcela,
                        tipo,
                        desc;
    }

    public class FechAssocAnalitico
    {
        public long     totVlr = 0,
                        totVlrConsulta = 0,
                        totCoPart = 0,
                        totExtra_diaria = 0,
                        totExtra_mat = 0,
                        totExtra_meds = 0,
                        totExtra_naomed = 0,
                        totExtra_opme = 0,
                        totExtra_pacserv = 0;

        public string   secao,
                        matricula,
                        nome,
                        cpf,
                        stotVlr,
                        stotVlrConsulta,
                        stotCoPart, 
                        tuss,
                        stotExtra_diaria,
                        stotExtra_mat,
                        stotExtra_meds,
                        stotExtra_naomed,
                        stotExtra_opme,
                        stotExtra_pacserv;

        public List<FechAssocAnalDetalhe> results = new List<FechAssocAnalDetalhe>();
        public List<FechAssocAnalDetalheExtra> resultsExtras = new List<FechAssocAnalDetalheExtra>();
    }

    public class EmissorFechamentoAssocAnaliticoReport
    {
        public bool failed = false;

        public long totCreds = 0,
                    totAssociados = 0,
                    totVlr = 0,
                    totVlrConsulta = 0,
                    totCoPart = 0,
                    totExtra_diaria = 0,
                        totExtra_mat = 0,
                        totExtra_meds = 0,
                        totExtra_naomed = 0,
                        totExtra_opme = 0,
                        totExtra_pacserv = 0;

        public string stotVlr,
                        stotVlrConsulta,
                        stotCoPart,
                        mesAno,
                        stotExtra_diaria,
                        stotExtra_mat,
                        stotExtra_meds,
                        stotExtra_naomed,
                        stotExtra_opme,
                        stotExtra_pacserv;

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
                        Where(y => y.tgSituacao == 1 || y.tgSituacao == 2 || y.tgSituacao == 3).
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

            var empConsultaValores = db.EmpresaConsultaAno.
                                        Where(y => y.nuAno == filter.ano && y.fkEmpresa == db.currentUser.fkEmpresa).
                                        FirstOrDefault();

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
                        totVlr = 0,
                        totVlrConsulta = 0,
                    };

                    long qtConsulta = 0;

                    // --------------------
                    // Procedimentos
                    // --------------------

                    #region - code - 

                    foreach (var aut in auts.
                                        Where(y => y.fkAssociado == assocTb.id).
                                        Where( y=> y.nuTipoAutorizacao == 1).
                                        OrderBy(y => y.dtSolicitacao).
                                        ToList())
                    {
                        var cred = lstCreds.
                                    Where(y => y.id == aut.fkCredenciado).
                                    FirstOrDefault();
                        
                        var fkProc = procsTuus.Where(y => y.id == aut.fkProcedimento).FirstOrDefault();

                        var portador = db.Associado.Where(y => y.id == aut.fkAssociadoPortador).FirstOrDefault();

                        switch (fkProc.nuCodTUSS)
                        {
                            case 10101012: case 10101039: case 10102019: case 10103015: case 10103023: case 10103031:
                            case 10104011: case 10104020: case 10106014: case 10106030: case 10106049: case 90000001:
                                qtConsulta++;
                                break;
                        }

                        long _vlrCons = 0;

                        switch (qtConsulta)
                        {
                            case 1: _vlrCons = (long)empConsultaValores.vrPreco1; break;
                            case 2: _vlrCons = (long)empConsultaValores.vrPreco2; break;
                            case 3: _vlrCons = (long)empConsultaValores.vrPreco3; break;
                            case 4: _vlrCons = (long)empConsultaValores.vrPreco4; break;
                            case 5: _vlrCons = (long)empConsultaValores.vrPreco5; break;
                            case 6: _vlrCons = (long)empConsultaValores.vrPreco6; break;
                            case 7: _vlrCons = (long)empConsultaValores.vrPreco7; break;
                            case 8: _vlrCons = (long)empConsultaValores.vrPreco8; break;
                            case 9: _vlrCons = (long)empConsultaValores.vrPreco9; break;
                        }

                        resultAssoc.results.Add(new FechAssocAnalDetalhe
                        {
                            serial = serial.ToString(),
                            nsu = aut.nuNSU != null ? aut.nuNSU.ToString() : "",
                            portador = portador != null ? portador.stName : "",
                            cnpj = cred.stCnpj,
                            codCred = cred.nuCodigo.ToString(),
                            credenciado = cred.stNome,
                            especialidade = lstEspecialidade.Where (y=> y.id == cred.fkEspecialidade).FirstOrDefault().stNome,
                            parcela = aut.nuIndice + " / " + aut.nuTotParcelas,
                            dtSolicitacao = Convert.ToDateTime(aut.dtSolicitacao).ToString("dd/MM/yyyy hh:mm"),                            
                            vlr = mon.setMoneyFormat((long)aut.vrParcela),
                            vlrCoPart = mon.setMoneyFormat((long)aut.vrParcelaCoPart),
                            vlrConsulta = mon.setMoneyFormat(_vlrCons),
                            tuss = fkProc.nuCodTUSS + " - " + fkProc.stProcedimento
                        });

                        resultAssoc.totVlr += (long)aut.vrParcela;
                        resultAssoc.totVlrConsulta += _vlrCons;
                        resultAssoc.totCoPart += (long)aut.vrParcelaCoPart;
                    }

                    #endregion
                    
                    // --------------
                    // Extras
                    // --------------

                    #region - code - 

                    foreach (var aut in auts.
                                        Where(y => y.fkAssociado == assocTb.id).
                                        Where(y => y.nuTipoAutorizacao > 1).
                                        OrderBy(y => y.dtSolicitacao).
                                        ToList())
                    {
                        var cred = lstCreds.Where(y => y.id == aut.fkCredenciado).FirstOrDefault();
                        var portador = db.Associado.Where(y => y.id == aut.fkAssociadoPortador).FirstOrDefault();

                        var e = new FechAssocAnalDetalheExtra
                        {
                            serial = serial.ToString(),
                            nsu = aut.nuNSU != null ? aut.nuNSU.ToString() : "",
                            portador = portador != null ? portador.stName : "",
                            cnpj = cred.stCnpj,
                            codCred = cred.nuCodigo.ToString(),
                            credenciado = cred.stNome,
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
                            case 2: e.desc = db.SaudeValorDiaria.Where(y=> y.id == aut.fkPrecificacao).Select ( y=> y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                            case 3: e.desc = db.SaudeValorMaterial.Where(y => y.id == aut.fkPrecificacao).Select(y => y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                            case 4: e.desc = db.SaudeValorMedicamento.Where(y => y.id == aut.fkPrecificacao).Select(y => y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                            case 5: e.desc = db.SaudeValorNaoMedico.Where(y => y.id == aut.fkPrecificacao).Select(y => y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                            case 6: e.desc = db.SaudeValorOPME.Where(y => y.id == aut.fkPrecificacao).Select(y => y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                            case 7: e.desc = db.SaudeValorPacote.Where(y => y.id == aut.fkPrecificacao).Select(y => y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                        }

                        resultAssoc.totVlr += (long)aut.vrParcela;

                        switch (aut.nuTipoAutorizacao)
                        {
                            case 2: resultAssoc.totExtra_diaria += (long)aut.vrParcela; break;
                            case 3: resultAssoc.totExtra_mat += (long)aut.vrParcela; break;
                            case 4: resultAssoc.totExtra_meds += (long)aut.vrParcela; break;
                            case 5: resultAssoc.totExtra_naomed += (long)aut.vrParcela; break;
                            case 6: resultAssoc.totExtra_opme += (long)aut.vrParcela; break;
                            case 7: resultAssoc.totExtra_pacserv += (long)aut.vrParcela; break;
                        }

                        resultAssoc.resultsExtras.Add(e);
                    }

                    #endregion

                    // ------------------------------
                    // totais do associado
                    // ------------------------------

                    resultAssoc.stotVlr = mon.setMoneyFormat(resultAssoc.totVlr);
                    resultAssoc.stotVlrConsulta = mon.setMoneyFormat(resultAssoc.totVlrConsulta);
                    resultAssoc.stotCoPart = mon.setMoneyFormat(resultAssoc.totCoPart);
                    resultAssoc.stotExtra_diaria = mon.setMoneyFormat(resultAssoc.totExtra_diaria);
                    resultAssoc.stotExtra_mat = mon.setMoneyFormat(resultAssoc.totExtra_mat);
                    resultAssoc.stotExtra_meds = mon.setMoneyFormat(resultAssoc.totExtra_meds);
                    resultAssoc.stotExtra_naomed = mon.setMoneyFormat(resultAssoc.totExtra_naomed);
                    resultAssoc.stotExtra_opme = mon.setMoneyFormat(resultAssoc.totExtra_opme);
                    resultAssoc.stotExtra_pacserv = mon.setMoneyFormat(resultAssoc.totExtra_pacserv);

                    // ------------------------------
                    // totais do resultado
                    // ------------------------------

                    resultado.totVlr += resultAssoc.totVlr;
                    resultado.totVlrConsulta += resultAssoc.totVlrConsulta;
                    resultado.totCoPart += resultAssoc.totCoPart;
                    resultado.totExtra_diaria += resultAssoc.totExtra_diaria;
                    resultado.totExtra_mat += resultAssoc.totExtra_mat;
                    resultado.totExtra_meds += resultAssoc.totExtra_meds;
                    resultado.totExtra_naomed += resultAssoc.totExtra_naomed;
                    resultado.totExtra_opme += resultAssoc.totExtra_opme;
                    resultado.totExtra_pacserv += resultAssoc.totExtra_pacserv;
                    
                    resultado.results.Add(resultAssoc);
                }
            }

            // valores em dinheiro

            resultado.stotVlr = mon.setMoneyFormat(resultado.totVlr);
            resultado.stotVlrConsulta = mon.setMoneyFormat(resultado.totVlrConsulta);
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
