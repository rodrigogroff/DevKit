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
        public long     _totVlr = 0,
                        _totVlrConsulta = 0,
                        _totCoPart = 0,

                        _totExtra_procs = 0,
                        _totExtra_diaria = 0,
                        _totExtra_mat = 0,
                        _totExtra_meds = 0,
                        _totExtra_naomed = 0,
                        _totExtra_opme = 0,
                        _totExtra_pacserv = 0;

        public string   matricula,
                        nome,
                        cpf,
                        tuss,

                        stotVlr,
                        stotVlrConsulta,
                        stotCoPart, 
                        stotExtra_procs,
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

                    _totVlr = 0,
                    _totVlrConsulta = 0,
                    _totCoPart = 0,
                    _totExtra_procs = 0,
                    _totExtra_diaria = 0,
                    _totExtra_mat = 0,
                    _totExtra_meds = 0,
                    _totExtra_naomed = 0,
                    _totExtra_opme = 0,
                    _totExtra_pacserv = 0;

        public string   mesAno,

                        stotVlr,
                        stotVlrConsulta,
                        stotCoPart,
                        stotExtra_procs,
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
                        where e.fkSecao == filter.fkSecao || filter.fkSecao == 0
                        where filter.mat == null || e.nuMatricula == Convert.ToInt64(filter.mat)
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
                        Where(y => y.tgSituacao == filter.tgSituacao).
                        ToList();

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

            var empConsultaValores = db.EmpresaConsultaAno.
                                        Where(y => y.nuAno == filter.ano && y.fkEmpresa == db.currentUser.fkEmpresa).
                                        FirstOrDefault();

            resultado.totCreds = lstIds.Count();
            resultado.totAssociados = 0;

            resultado.results = new List<FechAssocAnalitico>();

            foreach (var assocTb in lst)
            {
                if (auts.Any(y => y.fkAssociado == assocTb.id))
                {
                    resultado.totAssociados++;

                    var resAssociadoSint = new FechAssocAnalitico
                    {
                        cpf = assocTb.stCPF,
                        matricula = assocTb.nuMatricula.ToString(),
                        nome = assocTb.stName,                        
                        _totCoPart = 0,
                        _totVlr = 0,
                        _totVlrConsulta = 0,
                    };

                    long qtConsulta = 0;

                    // --------------------
                    // Procedimentos
                    // --------------------

                    #region - code - 

                    foreach (var aut in auts.
                                        Where(y => y.fkAssociado == assocTb.id).
                                        Where( y=> y.nuTipoAutorizacao == 1 || y.nuTipoAutorizacao == null).
                                        OrderBy(y => y.dtSolicitacao).
                                        ToList())
                    {
                        var cred = lstCreds.
                                    Where(y => y.id == aut.fkCredenciado).
                                    FirstOrDefault();
                        
                        var _proc = procsTuus.Where(y => y.id == aut.fkProcedimento).FirstOrDefault();

                        var portador = db.Associado.Where(y => y.id == aut.fkAssociadoPortador).FirstOrDefault();

                        if (_proc != null)
                            switch (_proc.nuCodTUSS)
                            {
                                case 10101012: case 10101039: case 10102019: case 10103015: case 10103023: case 10103031:
                                case 10104011: case 10104020: case 10106014: case 10106030: case 10106049: case 90000001:
                                    qtConsulta++;
                                    break;
                            }

                        long _vlrCons = 0;

                        if (_proc != null)
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

                        var especTb = lstEspecialidade.Where(y => y.id == cred.fkEspecialidade).FirstOrDefault();

                        resAssociadoSint.results.Add(new FechAssocAnalDetalhe
                        {
                            serial = serial.ToString(),
                            nsu = aut.nuNSU != null ? aut.nuNSU.ToString() : "",
                            portador = portador != null ? portador.stName : "",
                            cnpj = cred.stCnpj,
                            codCred = cred.nuCodigo.ToString(),
                            credenciado = cred.stNome,
                            especialidade = especTb != null ? especTb.stNome : "",
                            parcela = aut.nuIndice + " / " + aut.nuTotParcelas,
                            dtSolicitacao = Convert.ToDateTime(aut.dtSolicitacao).ToString("dd/MM/yyyy hh:mm"),                            
                            vlr = mon.setMoneyFormat((long)aut.vrParcela),
                            vlrCoPart = mon.setMoneyFormat((long)aut.vrParcelaCoPart),
                            vlrConsulta = mon.setMoneyFormat(_vlrCons),
                            tuss = _proc != null ? _proc.nuCodTUSS + " - " + _proc.stProcedimento : ""
                        });

                        resAssociadoSint._totVlrConsulta += _vlrCons;                        
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
                            case null: case 1: e.tipo = "Serviços"; break;
                            case 2: e.tipo = "Diária"; break;
                            case 3: e.tipo = "Materiais"; break;
                            case 4: e.tipo = "Medicamentos"; break;
                            case 5: e.tipo = "Não médicos"; break;
                            case 6: e.tipo = "OPME"; break;
                            case 7: e.tipo = "Pacote Serviços"; break;
                        }

                        switch (aut.nuTipoAutorizacao)
                        {
                            case null: case 1: e.desc = db.SaudeValorProcedimento.Where(y => y.id == aut.fkPrecificacao).Select(y => y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                            case 2: e.desc = db.SaudeValorDiaria.Where(y=> y.id == aut.fkPrecificacao).Select ( y=> y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                            case 3: e.desc = db.SaudeValorMaterial.Where(y => y.id == aut.fkPrecificacao).Select(y => y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                            case 4: e.desc = db.SaudeValorMedicamento.Where(y => y.id == aut.fkPrecificacao).Select(y => y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                            case 5: e.desc = db.SaudeValorNaoMedico.Where(y => y.id == aut.fkPrecificacao).Select(y => y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                            case 6: e.desc = db.SaudeValorOPME.Where(y => y.id == aut.fkPrecificacao).Select(y => y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                            case 7: e.desc = db.SaudeValorPacote.Where(y => y.id == aut.fkPrecificacao).Select(y => y.nuCodInterno + " - " + y.stDesc).FirstOrDefault(); break;
                        }

                        resAssociadoSint.resultsExtras.Add(e);
                    }

                    #endregion

                    // ------------------------------
                    // totais do associado
                    // ------------------------------

                    var q = auts.Where(y => y.fkAssociado == assocTb.id).ToList();

                    var q1 = q.Where(y => y.nuTipoAutorizacao == 1 || y.nuTipoAutorizacao == null).ToList();
                    var q2 = q.Where(y => y.nuTipoAutorizacao == 2).ToList();
                    var q3 = q.Where(y => y.nuTipoAutorizacao == 3).ToList();
                    var q4 = q.Where(y => y.nuTipoAutorizacao == 4).ToList();
                    var q5 = q.Where(y => y.nuTipoAutorizacao == 5).ToList();
                    var q6 = q.Where(y => y.nuTipoAutorizacao == 6).ToList();
                    var q7 = q.Where(y => y.nuTipoAutorizacao == 7).ToList();

                    resAssociadoSint._totExtra_procs = q1.Sum(y => (long)y.vrParcela);
                    resAssociadoSint._totCoPart = q1.Sum(y => (long)y.vrParcelaCoPart);                    
                    resAssociadoSint._totExtra_diaria = q2.Sum(y => (long)y.vrParcela);                    
                    resAssociadoSint._totExtra_mat = q3.Sum(y => (long)y.vrParcela);                    
                    resAssociadoSint._totExtra_meds = q4.Sum(y => (long)y.vrParcela);                    
                    resAssociadoSint._totExtra_naomed = q5.Sum(y => (long)y.vrParcela);                    
                    resAssociadoSint._totExtra_opme = q6.Sum(y => (long)y.vrParcela);                    
                    resAssociadoSint._totExtra_pacserv = q7.Sum(y => (long)y.vrParcela);

                    resAssociadoSint._totVlr = resAssociadoSint._totVlrConsulta +
                                                resAssociadoSint._totExtra_procs +
                                                resAssociadoSint._totExtra_diaria +
                                                resAssociadoSint._totExtra_mat +
                                                resAssociadoSint._totExtra_meds +
                                                resAssociadoSint._totExtra_naomed +
                                                resAssociadoSint._totExtra_opme +
                                                resAssociadoSint._totExtra_pacserv;
                    
                    resAssociadoSint.stotVlr = mon.setMoneyFormat(resAssociadoSint._totVlr);
                    resAssociadoSint.stotVlrConsulta = mon.setMoneyFormat(resAssociadoSint._totVlrConsulta);
                    resAssociadoSint.stotCoPart = mon.setMoneyFormat(resAssociadoSint._totCoPart);
                    
                    resAssociadoSint.stotExtra_procs = mon.setMoneyFormat(resAssociadoSint._totExtra_procs);
                    resAssociadoSint.stotExtra_diaria = mon.setMoneyFormat(resAssociadoSint._totExtra_diaria);
                    resAssociadoSint.stotExtra_mat = mon.setMoneyFormat(resAssociadoSint._totExtra_mat);
                    resAssociadoSint.stotExtra_meds = mon.setMoneyFormat(resAssociadoSint._totExtra_meds);
                    resAssociadoSint.stotExtra_naomed = mon.setMoneyFormat(resAssociadoSint._totExtra_naomed);
                    resAssociadoSint.stotExtra_opme = mon.setMoneyFormat(resAssociadoSint._totExtra_opme);
                    resAssociadoSint.stotExtra_pacserv = mon.setMoneyFormat(resAssociadoSint._totExtra_pacserv);

                    // ------------------------------
                    // totais do resultado
                    // ------------------------------

                    resultado._totVlr += resAssociadoSint._totVlr;
                    resultado._totVlrConsulta += resAssociadoSint._totVlrConsulta;
                    resultado._totCoPart += resAssociadoSint._totCoPart;

                    resultado._totExtra_procs += resAssociadoSint._totExtra_procs;
                    resultado._totExtra_diaria += resAssociadoSint._totExtra_diaria;
                    resultado._totExtra_mat += resAssociadoSint._totExtra_mat;
                    resultado._totExtra_meds += resAssociadoSint._totExtra_meds;
                    resultado._totExtra_naomed += resAssociadoSint._totExtra_naomed;
                    resultado._totExtra_opme += resAssociadoSint._totExtra_opme;
                    resultado._totExtra_pacserv += resAssociadoSint._totExtra_pacserv;
                    
                    resultado.results.Add(resAssociadoSint);
                }
            }

            resultado.stotVlr = mon.setMoneyFormat(resultado._totVlr);
            resultado.stotVlrConsulta = mon.setMoneyFormat(resultado._totVlrConsulta);
            resultado.stotCoPart = mon.setMoneyFormat(resultado._totCoPart);

            resultado.stotExtra_procs = mon.setMoneyFormat(resultado._totExtra_procs);
            resultado.stotExtra_diaria = mon.setMoneyFormat(resultado._totExtra_diaria);
            resultado.stotExtra_mat = mon.setMoneyFormat(resultado._totExtra_mat);
            resultado.stotExtra_meds = mon.setMoneyFormat(resultado._totExtra_meds);
            resultado.stotExtra_naomed = mon.setMoneyFormat(resultado._totExtra_naomed);
            resultado.stotExtra_opme = mon.setMoneyFormat(resultado._totExtra_opme);
            resultado.stotExtra_pacserv = mon.setMoneyFormat(resultado._totExtra_pacserv);
        }
    }
}
