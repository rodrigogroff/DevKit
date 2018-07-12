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
                        nsu,
                        dtSolicitacao, 
                        cpf,
                        secao,
                        matricula,
                        associado,
                        parcela,
                        vlr,
                        vlrTotal,
                        vlrCoPart,
                        tuss;

        public List<FechCredAnalDetalheExtra> resultsExtras = new List<FechCredAnalDetalheExtra>();
    }

    public class FechCredAnalDetalheExtra
    {
        public string serial,
                        nsu,
                        nsuRef,
                        portador,
                        matricula,
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

                        totExtra_procs = 0,
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

                        stotExtra_procs,
                        stotExtra_diaria,
                        stotExtra_mat,
                        stotExtra_meds,
                        stotExtra_naomed,
                        stotExtra_opme,
                        stotExtra_pacserv;

        public List<FechCredAnalDetalhe> results = new List<FechCredAnalDetalhe>();
    }

    public class EmissorFechamentoCredAnaliticoReport
    {
        public bool failed = false;

        public long totCreds = 0,
                        totAssociados = 0,
                        totVlr = 0,
                        totCoPart = 0,

                        totExtra_procs = 0,
                        totExtra_diaria = 0,
                        totExtra_mat = 0,
                        totExtra_meds = 0,
                        totExtra_naomed = 0,
                        totExtra_opme = 0,
                        totExtra_pacserv = 0;

        public string   stotVlr, 
                        stotCoPart,
                        mesAno,

                        stotExtra_procs,
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
                        where filter.fkEmpresa == null || (filter.fkEmpresa != null && ce.fkEmpresa == filter.fkEmpresa)
                        where filter.fkCredenciado == null || (filter.fkCredenciado != null && e.id == filter.fkCredenciado)
                        where filter.codCred == null || e.nuCodigo == Convert.ToInt64(filter.codCred)
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

                    var auts_principais = auts.Where(y => y.fkCredenciado == cred.id).
                                             Where(y => y.nuNSURef == null).
                                             OrderBy(y => y.dtSolicitacao).
                                             ToList();

                    foreach (var aut in auts_principais)
                    {
                        var assoc = lstAssocs.
                                    Where(y => y.id == aut.fkAssociadoPortador).
                                    FirstOrDefault();

                        if (assoc == null)
                            assoc = lstAssocs.
                                    Where(y => y.id == aut.fkAssociado).
                                    FirstOrDefault();

                        if (filter.fkSecao != assoc.fkSecao)
                            continue;

                        var secao = secoes.Where(y => y.id == assoc.fkSecao).FirstOrDefault();
                        var proc = procsTuus.Where(y => y.id == aut.fkProcedimento).FirstOrDefault();

                        long _vlrTotal = (long) aut.vrParcela;

                        var det = new FechCredAnalDetalhe
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
                        };

                        resultCred.results.Add(det);

                        resultCred.totVlr += (long)aut.vrParcela;
                        resultCred.totCoPart += (long)aut.vrParcelaCoPart;

                        var lstExtras = auts.
                                        Where(y => y.fkCredenciado == cred.id).
                                        Where(y => y.nuNSURef == aut.nuNSU).                                        
                                        OrderBy(y => y.dtSolicitacao).
                                        ToList();

                        foreach (var autExtra in lstExtras)
                        {
                            var portador = db.Associado.Where(y => y.id == autExtra.fkAssociadoPortador).FirstOrDefault();

                            var extra = new FechCredAnalDetalheExtra
                            {
                                serial = serial.ToString(),
                                nsu = autExtra.nuNSU != null ? autExtra.nuNSU.ToString() : "",
                                nsuRef = autExtra.nuNSURef != null ? autExtra.nuNSURef.ToString() : "",
                                portador = portador != null ? portador.stName : "",
                                matricula = portador != null ? portador.nuMatricula.ToString() : "",
                                cpf = portador.stCPF,
                                parcela = autExtra.nuIndice + " / " + autExtra.nuTotParcelas,
                                dtSolicitacao = Convert.ToDateTime(autExtra.dtSolicitacao).ToString("dd/MM/yyyy hh:mm"),
                                vlr = mon.setMoneyFormat((long)autExtra.vrParcela),
                            };

                            _vlrTotal += (long) autExtra.vrParcela;

                            if (autExtra.nuTipoAutorizacao == 1)
                            {
                                extra.vlr = mon.setMoneyFormat((long)autExtra.vrParcela) + " / " + mon.setMoneyFormat((long)autExtra.vrCoPart);
                                resultCred.totCoPart += (long)autExtra.vrCoPart;
                            }                                

                            switch (autExtra.nuTipoAutorizacao)
                            {
                                case 1: extra.tipo = "Procedimentos"; break;
                                case 2: extra.tipo = "Diárias"; break;
                                case 3: extra.tipo = "Materiais"; break;
                                case 4: extra.tipo = "Medicamentos"; break;
                                case 5: extra.tipo = "Não médicos"; break;
                                case 6: extra.tipo = "OPME"; break;
                                case 7: extra.tipo = "Pacote Serviços"; break;
                            }

                            switch (autExtra.nuTipoAutorizacao)
                            {
                                case 1: extra.desc = db.SaudeValorProcedimento.Where(y => y.id == autExtra.fkPrecificacao).Select(y => y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                                case 2: extra.desc = db.SaudeValorDiaria.Where(y => y.id == autExtra.fkPrecificacao).Select(y => y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                                case 3: extra.desc = db.SaudeValorMaterial.Where(y => y.id == autExtra.fkPrecificacao).Select(y => y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                                case 4: extra.desc = db.SaudeValorMedicamento.Where(y => y.id == autExtra.fkPrecificacao).Select(y => y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                                case 5: extra.desc = db.SaudeValorNaoMedico.Where(y => y.id == autExtra.fkPrecificacao).Select(y => y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                                case 6: extra.desc = db.SaudeValorOPME.Where(y => y.id == autExtra.fkPrecificacao).Select(y => y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                                case 7: extra.desc = db.SaudeValorPacote.Where(y => y.id == autExtra.fkPrecificacao).Select(y => y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                            }

                            resultCred.totVlr += (long)autExtra.vrParcela;

                            switch (autExtra.nuTipoAutorizacao)
                            {
                                case 1: resultCred.totExtra_procs += (long)autExtra.vrParcela; break;
                                case 2: resultCred.totExtra_diaria += (long)autExtra.vrParcela; break;
                                case 3: resultCred.totExtra_mat += (long)autExtra.vrParcela; break;
                                case 4: resultCred.totExtra_meds += (long)autExtra.vrParcela; break;
                                case 5: resultCred.totExtra_naomed += (long)autExtra.vrParcela; break;
                                case 6: resultCred.totExtra_opme += (long)autExtra.vrParcela; break;
                                case 7: resultCred.totExtra_pacserv += (long)autExtra.vrParcela; break;
                            }

                            det.resultsExtras.Add(extra);
                        }

                        det.vlrTotal = mon.setMoneyFormat(_vlrTotal);
                    }

                    resultCred.stotVlr = mon.setMoneyFormat(resultCred.totVlr);
                    resultCred.stotCoPart = mon.setMoneyFormat(resultCred.totCoPart);

                    resultCred.stotExtra_procs = mon.setMoneyFormat(resultCred.totExtra_procs);
                    resultCred.stotExtra_diaria = mon.setMoneyFormat(resultCred.totExtra_diaria);
                    resultCred.stotExtra_mat = mon.setMoneyFormat(resultCred.totExtra_mat);
                    resultCred.stotExtra_meds = mon.setMoneyFormat(resultCred.totExtra_meds);
                    resultCred.stotExtra_naomed = mon.setMoneyFormat(resultCred.totExtra_naomed);
                    resultCred.stotExtra_opme = mon.setMoneyFormat(resultCred.totExtra_opme);
                    resultCred.stotExtra_pacserv = mon.setMoneyFormat(resultCred.totExtra_pacserv);

                    resultado.totVlr += resultCred.totVlr;
                    resultado.totCoPart += resultCred.totCoPart;

                    resultado.totExtra_procs += resultCred.totExtra_procs;
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

            resultado.stotExtra_procs = mon.setMoneyFormat(resultado.totExtra_procs);
            resultado.stotExtra_diaria = mon.setMoneyFormat(resultado.totExtra_diaria);
            resultado.stotExtra_mat = mon.setMoneyFormat(resultado.totExtra_mat);
            resultado.stotExtra_meds = mon.setMoneyFormat(resultado.totExtra_meds);
            resultado.stotExtra_naomed = mon.setMoneyFormat(resultado.totExtra_naomed);
            resultado.stotExtra_opme = mon.setMoneyFormat(resultado.totExtra_opme);
            resultado.stotExtra_pacserv = mon.setMoneyFormat(resultado.totExtra_pacserv);
        }
    }
}
