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
        public long __id = 0;

        public string   nsu,
                        dtSolicitacao, 
                        cpf,
                        secao,
                        matricula,
                        associado,
                        parcela,
                        nu_tipo,
                        vlr,
                        vlrTotal,
                        vlrCoPart,
                        tuss;

        public List<FechCredAnalDetalheExtra> resultsExtras = new List<FechCredAnalDetalheExtra>();
    }

    public class FechCredAnalDetalheExtra
    {
        public long __id = 0;

        public string   nsu,
                        nsuRef,
                        portador,
                        matricula,
                        cpf,                        
                        dtSolicitacao,                        
                        vlr,
                        vlrCoPart,
                        parcela,
                        tipo,
                        nu_tipo,
                        desc;
    }

    public class FechCredenciadoAnalitico
    {
        public long     _totVlr = 0, 
                        _totCoPart = 0,
                        _tot_procs = 0,
                        _tot_diaria = 0,
                        _tot_mat = 0,
                        _tot_meds = 0,
                        _tot_naomed = 0,
                        _tot_opme = 0,
                        _tot_pacserv = 0;

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
        public List<FechCredAnalDetalheExtra> resultsExtras = new List<FechCredAnalDetalheExtra>();
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

        public List<FechCredenciadoAnalitico> results = new List<FechCredenciadoAnalitico>();
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

            var procsCredTuus = db.CredenciadoEmpresaTuss.Where(y => y.fkEmpresa == db.currentUser.fkEmpresa).ToList();
            var procsTuus = db.TUSS.ToList();
            var lstEspecs = db.Especialidade.ToList();

            var lstIdsAssocs = auts.Select(y => y.fkAssociado).Distinct().ToList();
            var lstMats = db.Associado.Where(y => lstIdsAssocs.Contains(y.id)).Select (y=>y.nuMatricula).ToList();
            var lstAssocs = db.Associado.Where(y => lstMats.Contains(y.nuMatricula)).ToList();

            resultado.totCreds = auts.Select(y => y.fkCredenciado).Distinct().Count();
            resultado.totAssociados = auts.Select(y => y.fkAssociado).Distinct().Count();

            resultado.totVlr = 0;

            resultado.results = new List<FechCredenciadoAnalitico>();

            foreach (var cred in lst)
            {
                if (auts.Any(y => y.fkCredenciado == cred.id))
                {
                    var resultCred = new FechCredenciadoAnalitico
                    {
                        cnpj = cred.stCnpj,
                        codCredenciado = cred.nuCodigo.ToString(),
                        nomeCredenciado = cred.stNome,
                        sDadosBancarios = "=> Banco: " + cred.stBanco + " Agência " + cred.stAgencia + " Conta " + cred.stConta
                    };

                    var auts_principais = auts.Where(y => y.fkCredenciado == cred.id).
                                             Where(y => y.nuNSURef == null).
                                             OrderBy(y => y.dtSolicitacao).
                                             ToList();

                    var hshDup = new Hashtable();

                    foreach (var autPrincipal in auts_principais)
                    {
                        hshDup[autPrincipal.id] = true;

                        var assoc = lstAssocs.
                                    Where(y => y.id == autPrincipal.fkAssociadoPortador).
                                    FirstOrDefault();

                        if (assoc == null)
                            assoc = lstAssocs.
                                    Where(y => y.id == autPrincipal.fkAssociado).
                                    FirstOrDefault();

                        if (filter.fkSecao != assoc.fkSecao)
                            continue;

                        var secao = secoes.Where(y => y.id == assoc.fkSecao).FirstOrDefault();
                        var proc = procsTuus.Where(y => y.id == autPrincipal.fkProcedimento).FirstOrDefault();

                        var itemAutorizado = new FechCredAnalDetalhe
                        {
                            __id = autPrincipal.id,
                            nsu = autPrincipal.nuNSU != null ? autPrincipal.nuNSU.ToString() : "",
                            associado = assoc.stName,
                            cpf = assoc.stCPF,
                            secao = secao.nuEmpresa + " - " + secao.stDesc,
                            parcela = autPrincipal.nuIndice + " / " + autPrincipal.nuTotParcelas,
                            dtSolicitacao = Convert.ToDateTime(autPrincipal.dtSolicitacao).ToString("dd/MM/yyyy hh:mm"),
                            matricula = assoc.nuMatricula.ToString(),
                            vlr = mon.setMoneyFormat((long)autPrincipal.vrParcela),                              
                            vlrCoPart = mon.setMoneyFormat((long)autPrincipal.vrParcelaCoPart),
                            vlrTotal = "",
                            tuss = proc != null ? proc.nuCodTUSS + " - " + proc.stProcedimento : "",
                            nu_tipo = "1"
                        };

                        resultCred.results.Add(itemAutorizado);

                        resultCred._totVlr += (long)autPrincipal.vrParcela;

                        resultCred._tot_procs += (long)autPrincipal.vrParcela;
                        resultCred._totCoPart += (long)autPrincipal.vrParcelaCoPart;
                                                
                        var lstExtras = auts.
                                        Where(y => y.fkCredenciado == cred.id).
                                        Where(y => y.nuNSURef == autPrincipal.nuNSU).                                        
                                        OrderBy(y => y.dtSolicitacao).
                                        ToList();

                        foreach (var autExtra in lstExtras)
                        {
                            hshDup[autExtra.id] = true;

                            var portador = db.Associado.Where(y => y.id == autExtra.fkAssociadoPortador).FirstOrDefault();

                            var extra = new FechCredAnalDetalheExtra
                            {
                                __id = autExtra.id,
                                nsu = autExtra.nuNSU != null ? autExtra.nuNSU.ToString() : "",
                                nsuRef = autExtra.nuNSURef != null ? autExtra.nuNSURef.ToString() : "",
                                portador = portador != null ? portador.stName : "",
                                matricula = portador != null ? portador.nuMatricula.ToString() : "",
                                cpf = portador.stCPF,
                                parcela = autExtra.nuIndice + " / " + autExtra.nuTotParcelas,
                                dtSolicitacao = Convert.ToDateTime(autExtra.dtSolicitacao).ToString("dd/MM/yyyy hh:mm"),
                                vlr = mon.setMoneyFormat((long)autExtra.vrParcela),
                                vlrCoPart = mon.setMoneyFormat((long)autExtra.vrParcelaCoPart),
                                nu_tipo = autExtra.nuTipoAutorizacao.ToString(),
                            };

                            switch (autExtra.nuTipoAutorizacao)
                            {
                                case 1: extra.tipo = "Serviços"; break;
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

                            switch (autExtra.nuTipoAutorizacao)
                            {
                                case 1: resultCred._tot_procs += (long)autExtra.vrParcela;
                                        resultCred._totCoPart += (long)autExtra.vrParcelaCoPart;
                                    break;

                                case 2: resultCred._tot_diaria += (long)autExtra.vrParcela; break;
                                case 3: resultCred._tot_mat += (long)autExtra.vrParcela; break;
                                case 4: resultCred._tot_meds += (long)autExtra.vrParcela; break;
                                case 5: resultCred._tot_naomed += (long)autExtra.vrParcela; break;
                                case 6: resultCred._tot_opme += (long)autExtra.vrParcela; break;
                                case 7: resultCred._tot_pacserv += (long)autExtra.vrParcela; break;
                            }

                            resultCred._totVlr += (long)autExtra.vrParcela;

                            itemAutorizado.resultsExtras.Add(extra);
                        }

                        itemAutorizado.vlrTotal = mon.setMoneyFormat(resultCred._totVlr);
                    }

                    // autorizações extras

                    var lstExtra = auts.Where(y => y.fkCredenciado == cred.id).OrderBy(y => y.dtSolicitacao).ToList();

                    foreach (var autExtra in lstExtra)
                    {
                        if (hshDup[autExtra.id] != null)
                            continue;

                        var portador = db.Associado.Where(y => y.id == autExtra.fkAssociadoPortador).FirstOrDefault();

                        var extra = new FechCredAnalDetalheExtra
                        {
                            __id = autExtra.id,
                            nsu = autExtra.nuNSU != null ? autExtra.nuNSU.ToString() : "",
                            nsuRef = autExtra.nuNSURef != null ? autExtra.nuNSURef.ToString() : "",
                            portador = portador != null ? portador.stName : "",
                            matricula = portador != null ? portador.nuMatricula.ToString() : "",
                            cpf = portador.stCPF,
                            parcela = autExtra.nuIndice + " / " + autExtra.nuTotParcelas,
                            dtSolicitacao = Convert.ToDateTime(autExtra.dtSolicitacao).ToString("dd/MM/yyyy hh:mm"),
                            vlr = mon.setMoneyFormat((long)autExtra.vrParcela),
                            nu_tipo = autExtra.nuTipoAutorizacao.ToString()
                        };

                        switch (autExtra.nuTipoAutorizacao)
                        {
                            case null: case 1:  extra.tipo = "Serviços";
                                                extra.vlrCoPart = mon.setMoneyFormat((long)autExtra.vrParcelaCoPart);
                                                break;

                            case 2: extra.tipo = "Diárias"; break;
                            case 3: extra.tipo = "Materiais"; break;
                            case 4: extra.tipo = "Medicamentos"; break;
                            case 5: extra.tipo = "Não médicos"; break;
                            case 6: extra.tipo = "OPME"; break;
                            case 7: extra.tipo = "Pacote Serviços"; break;
                        }

                        switch (autExtra.nuTipoAutorizacao)
                        {
                            case null: case 1: extra.desc = db.SaudeValorProcedimento.Where(y => y.id == autExtra.fkPrecificacao).Select(y => y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                            case 2: extra.desc = db.SaudeValorDiaria.Where(y => y.id == autExtra.fkPrecificacao).Select(y => y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                            case 3: extra.desc = db.SaudeValorMaterial.Where(y => y.id == autExtra.fkPrecificacao).Select(y => y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                            case 4: extra.desc = db.SaudeValorMedicamento.Where(y => y.id == autExtra.fkPrecificacao).Select(y => y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                            case 5: extra.desc = db.SaudeValorNaoMedico.Where(y => y.id == autExtra.fkPrecificacao).Select(y => y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                            case 6: extra.desc = db.SaudeValorOPME.Where(y => y.id == autExtra.fkPrecificacao).Select(y => y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                            case 7: extra.desc = db.SaudeValorPacote.Where(y => y.id == autExtra.fkPrecificacao).Select(y => y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                        }

                        switch (autExtra.nuTipoAutorizacao)
                        {
                            case null:
                            case 1:
                                resultCred._tot_procs += (long)autExtra.vrParcela;
                                resultCred._totCoPart += (long)autExtra.vrParcelaCoPart;
                                break;

                            case 2: resultCred._tot_diaria += (long)autExtra.vrParcela; break;
                            case 3: resultCred._tot_mat += (long)autExtra.vrParcela; break;
                            case 4: resultCred._tot_meds += (long)autExtra.vrParcela; break;
                            case 5: resultCred._tot_naomed += (long)autExtra.vrParcela; break;
                            case 6: resultCred._tot_opme += (long)autExtra.vrParcela; break;
                            case 7: resultCred._tot_pacserv += (long)autExtra.vrParcela; break;
                        }

                        resultCred._totVlr += (long)autExtra.vrParcela;

                        resultCred.resultsExtras.Add(extra);
                    }

                    resultCred.stotVlr = mon.setMoneyFormat(resultCred._totVlr);
                    resultCred.stotCoPart = mon.setMoneyFormat(resultCred._totCoPart);

                    resultCred.stotExtra_procs = mon.setMoneyFormat(resultCred._tot_procs);
                    resultCred.stotExtra_diaria = mon.setMoneyFormat(resultCred._tot_diaria);
                    resultCred.stotExtra_mat = mon.setMoneyFormat(resultCred._tot_mat);
                    resultCred.stotExtra_meds = mon.setMoneyFormat(resultCred._tot_meds);
                    resultCred.stotExtra_naomed = mon.setMoneyFormat(resultCred._tot_naomed);
                    resultCred.stotExtra_opme = mon.setMoneyFormat(resultCred._tot_opme);
                    resultCred.stotExtra_pacserv = mon.setMoneyFormat(resultCred._tot_pacserv);

                    resultado.totExtra_procs += resultCred._tot_procs;
                    resultado.totExtra_diaria += resultCred._tot_diaria;
                    resultado.totExtra_mat += resultCred._tot_mat;
                    resultado.totExtra_meds += resultCred._tot_meds;
                    resultado.totExtra_naomed += resultCred._tot_naomed;
                    resultado.totExtra_opme += resultCred._tot_opme;
                    resultado.totExtra_pacserv += resultCred._tot_pacserv;

                    resultado.totVlr += resultCred._totVlr;

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
