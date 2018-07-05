using LinqToDB;
using SyCrafEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
    public class FechAssocSint
    {
        public string   serial,
                        secao,
                        matricula,
                        associado,
                        qtdAutos,
                        vlrConsulta,
                        vlrAutos,
                        vlrCoPart,

                        vlrDiaria,
                        vlrMateriais,
                        vlrMeds,
                        vlrNM,
                        vlrOPME,
                        vlrServ,

                        qtdDiaria,
                        qtdMateriais,
                        qtdMeds,
                        qtdNM,
                        qtdOPME,
                        qtdServ,

                        ncads;
    }

    public class EmissorFechamentoAssocSintReport
    {
        public bool failed = false;

        public long totCreds = 0,
                    totAssocs = 0,
                    totVlrConsulta = 0,
                    totVlr = 0,
                    totCoPart = 0,
                    procsNCad = 0;

        public string stotVlrConsulta,
                        stotVlr,
                        stotCoPart,

                        // Procedimentos, diária, Materiais,Medicamentos,Não médicos,OPME,Pacote Serviços

                        stot_Diaria,
                        stot_Materiais,
                        stot_Meds,
                        stot_NM,
                        stot_OPME,
                        stot_Serv;

        public List<FechAssocSint> results = new List<FechAssocSint>();
    }

    public partial class Empresa
    {
		public EmissorFechamentoAssocSintReport ListagemFechamento_AssocSint ( DevKitDB db, ListagemFechamentoFilter filter )
		{
            var ret = new EmissorFechamentoAssocSintReport();
            
            var query = from e in db.Associado
                        where e.fkEmpresa == db.currentUser.fkEmpresa
                        where e.fkSecao == filter.fkSecao || filter.fkSecao == 0
                        where e.nuTitularidade == 1
                        orderby e.stName
                        select e;

            LoaderAssocSint(db, query.ToList(), filter, ref ret );
            
            return ret;
		}

        public void LoaderAssocSint ( DevKitDB db, 
                                      List<Associado> lst,
                                      ListagemFechamentoFilter filter,
                                      ref EmissorFechamentoAssocSintReport resultado )
        {
            var auts = db.Autorizacao.
                        Where(y => y.fkEmpresa == db.currentUser.fkEmpresa).
                        Where(y => y.nuMes == filter.mes).
                        Where(y => y.nuAno == filter.ano).
                        Where(y => y.tgSituacao == filter.tgSituacao).
                        ToList();

            resultado.failed = !auts.Any();

            if (resultado.failed)
                return;

            var lstTitDeps = db.Associado.
                                Where(y => y.fkEmpresa == db.currentUser.fkEmpresa).
                                ToList();

            var procsCredTuus = db.CredenciadoEmpresaTuss.
                                    Where(y => y.fkEmpresa == db.currentUser.fkEmpresa).
                                    ToList();

            var procsTuus = db.TUSS.ToList();

            int serial = 1;

            var mon = new money();

            resultado.totCreds = auts.Select(y => y.fkCredenciado).Distinct().Count();
            resultado.totAssocs = auts.Select(y => y.fkAssociado).Distinct().Count();

            var secoes = db.EmpresaSecao.ToList();

            var empConsultaValores = db.EmpresaConsultaAno.
                                        Where(y => y.nuAno == filter.ano && y.fkEmpresa == db.currentUser.fkEmpresa).
                                        FirstOrDefault();

            resultado.results = new List<FechAssocSint>();

            foreach (var assoc in lst)
            {
                var tLstTitDeps = lstTitDeps.
                                    Where(y => y.nuMatricula == assoc.nuMatricula).
                                    Select (y=> y.id).
                                    ToList();

                var item = new FechAssocSint
                {
                    serial = serial.ToString(),
                    secao = secoes.Where (y=>y.id == assoc.fkSecao).Select(y=> y.nuEmpresa + " - " + y.stDesc ).FirstOrDefault(),
                    matricula = assoc.nuMatricula.ToString(),
                    associado = assoc.stName,
                    qtdAutos = auts.Where(y => tLstTitDeps.Contains((long)y.fkAssociado)).Count().ToString()
                };

                bool found = false;

                long totVlrConsulta = 0, 
                     totVlr = 0,
                     totCoPart = 0,
                     qtConsulta = 0;

                foreach (var aut in auts.Where(y => tLstTitDeps.Contains((long)y.fkAssociado) &&
                                                    (y.nuTipoAutorizacao == 1 || y.nuTipoAutorizacao == null)).ToList())
                {
                    found = true;

                    if (aut.fkPrecificacao == null)
                        continue;

                    var fkProc = procsTuus.
                                   Where(y => y.id == aut.fkPrecificacao).
                                   FirstOrDefault();

                    if (fkProc != null)
                    switch (fkProc.nuCodTUSS)
                    {
                        case 10101012: case 10101039: case 10102019: case 10103015: case 10103023: case 10103031:
                        case 10104011: case 10104020: case 10106014: case 10106030: case 10106049: case 90000001:
                            qtConsulta++;
                            break;
                    }
                }

                for (int t = 1; t <= qtConsulta; ++t)
                {
                    switch (t)
                    {
                        case 1: totVlrConsulta += (long)empConsultaValores.vrPreco1; break;
                        case 2: totVlrConsulta += (long)empConsultaValores.vrPreco2; break;
                        case 3: totVlrConsulta += (long)empConsultaValores.vrPreco3; break;
                        case 4: totVlrConsulta += (long)empConsultaValores.vrPreco4; break;
                        case 5: totVlrConsulta += (long)empConsultaValores.vrPreco5; break;
                        case 6: totVlrConsulta += (long)empConsultaValores.vrPreco6; break;
                        case 7: totVlrConsulta += (long)empConsultaValores.vrPreco7; break;
                        case 8: totVlrConsulta += (long)empConsultaValores.vrPreco8; break;
                        case 9: totVlrConsulta += (long)empConsultaValores.vrPreco9; break;
                    }
                }

                item.ncads = "";
                
                foreach (var aut in auts.Where(y => tLstTitDeps.Contains((long)y.fkAssociado) && y.nuTipoAutorizacao > 1).ToList())
                {
                    found = true;

                    var fkProc = procsTuus.
                                    Where(y => y.id == aut.fkProcedimento).
                                    FirstOrDefault();

                    if (fkProc != null)
                    {
                        var cfgTuss = procsCredTuus.
                                        Where(y => y.fkCredenciado == aut.fkCredenciado).
                                        Where(y => y.nuTUSS == fkProc.nuCodTUSS).
                                        FirstOrDefault();

                        item.ncads += fkProc.nuCodTUSS + ", ";

                        if (cfgTuss != null)
                        {
                            totVlr += (long)cfgTuss.vrProcedimento;
                            totCoPart += (long)cfgTuss.vrCoPart;
                        }
                        else
                            resultado.procsNCad++;
                    }
                }

                if (found)
                {
                    item.ncads = item.ncads.Trim().TrimEnd(',');

                    resultado.totVlrConsulta += totVlrConsulta;
                    resultado.totVlr += totVlr;
                    resultado.totCoPart += totCoPart;

                    item.vlrConsulta = mon.setMoneyFormat(totVlrConsulta);
                    item.vlrAutos = mon.setMoneyFormat(totVlr);
                    item.vlrCoPart = mon.setMoneyFormat(totCoPart);
                }
                
                long vr = 0;

                vr = auts.Where(y => y.nuTipoAutorizacao == 2).Where(y => tLstTitDeps.Contains((long)y.fkAssociado)).Sum(y => (long)y.vrParcela);

                item.vlrDiaria = mon.setMoneyFormat(vr);
                item.qtdDiaria = auts.Where(y => y.nuTipoAutorizacao == 2).Where(y => tLstTitDeps.Contains((long)y.fkAssociado)).Count().ToString();

                // ------------------------

                vr = auts.Where(y => y.nuTipoAutorizacao == 3).Where(y => tLstTitDeps.Contains((long)y.fkAssociado)).Sum(y => (long)y.vrParcela);

                item.vlrMateriais = mon.setMoneyFormat(vr);
                item.qtdMateriais = auts.Where(y => y.nuTipoAutorizacao == 3).Where(y => tLstTitDeps.Contains((long)y.fkAssociado)).Count().ToString();

                // ------------------------

                vr = auts.Where(y => y.nuTipoAutorizacao == 4).Where(y => tLstTitDeps.Contains((long)y.fkAssociado)).Sum(y => (long)y.vrParcela);

                item.vlrMeds = mon.setMoneyFormat(vr);
                item.qtdMeds = auts.Where(y => y.nuTipoAutorizacao == 4).Where(y => tLstTitDeps.Contains((long)y.fkAssociado)).Count().ToString();

                // ------------------------

                vr = auts.Where(y => y.nuTipoAutorizacao == 5).Where(y => tLstTitDeps.Contains((long)y.fkAssociado)).Sum(y => (long)y.vrParcela);

                item.vlrNM = mon.setMoneyFormat(vr);
                item.qtdNM = auts.Where(y => y.nuTipoAutorizacao == 5).Where(y => tLstTitDeps.Contains((long)y.fkAssociado)).Count().ToString();

                // ------------------------

                vr = auts.Where(y => y.nuTipoAutorizacao == 6).Where(y => tLstTitDeps.Contains((long)y.fkAssociado)).Sum(y => (long)y.vrParcela);

                item.vlrOPME = mon.setMoneyFormat(vr);
                item.qtdOPME = auts.Where(y => y.nuTipoAutorizacao == 6).Where(y => tLstTitDeps.Contains((long)y.fkAssociado)).Count().ToString();

                // ------------------------

                vr = auts.Where(y => y.nuTipoAutorizacao == 7).Where(y => tLstTitDeps.Contains((long)y.fkAssociado)).Sum(y => (long)y.vrParcela);

                item.vlrServ = mon.setMoneyFormat(vr);
                item.qtdServ = auts.Where(y => y.nuTipoAutorizacao == 7).Where(y => tLstTitDeps.Contains((long)y.fkAssociado)).Count().ToString();

                if (found || Convert.ToInt32(item.qtdDiaria) > 0
                    || Convert.ToInt32(item.qtdMateriais) > 0
                    || Convert.ToInt32(item.qtdMeds) > 0
                    || Convert.ToInt32(item.qtdNM) > 0
                    || Convert.ToInt32(item.qtdOPME) > 0
                    || Convert.ToInt32(item.qtdServ) > 0)
                {
                    resultado.results.Add(item);
                    serial++;
                }                
            }

            resultado.stotVlrConsulta = mon.setMoneyFormat(resultado.totVlrConsulta);
            resultado.stotVlr = mon.setMoneyFormat(resultado.totVlr);            
            resultado.stotCoPart = mon.setMoneyFormat(resultado.totCoPart);

            resultado.stot_Diaria = mon.setMoneyFormat(auts.Where(y => y.nuTipoAutorizacao == 2).Sum(y => (long)y.vrParcela));
            resultado.stot_Materiais = mon.setMoneyFormat(auts.Where(y => y.nuTipoAutorizacao == 3).Sum(y => (long)y.vrParcela));
            resultado.stot_Meds = mon.setMoneyFormat(auts.Where(y => y.nuTipoAutorizacao == 4).Sum(y => (long)y.vrParcela));
            resultado.stot_NM = mon.setMoneyFormat(auts.Where(y => y.nuTipoAutorizacao == 5).Sum(y => (long)y.vrParcela));
            resultado.stot_OPME = mon.setMoneyFormat(auts.Where(y => y.nuTipoAutorizacao == 6).Sum(y => (long)y.vrParcela));
            resultado.stot_Serv = mon.setMoneyFormat(auts.Where(y => y.nuTipoAutorizacao == 7).Sum(y => (long)y.vrParcela));
        }
    }
}
