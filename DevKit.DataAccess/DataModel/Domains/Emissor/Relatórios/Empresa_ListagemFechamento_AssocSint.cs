using LinqToDB;
using SyCrafEngine;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
    public class FechAssocSint
    {
        public long     _vlrConsulta, 
                        _vlrGeral,            
                        _vlrProcs,                        
                        _vlrDiaria,
                        _vlrMateriais,
                        _vlrMeds,
                        _vlrNM,
                        _vlrOPME,
                        _vlrCoPart,
                        _vlrServ;

        public string   serial,
                        matricula,
                        associado,
                        qtdAutos,
                        vlrConsulta,
                        vlrGeral,
                        vlrCoPart,
                        vlrProcs,
                        vlrDiaria,
                        vlrMateriais,
                        vlrMeds,
                        vlrNM,
                        vlrOPME,
                        vlrServ,
                        qtdProcs,
                        qtdDiaria,
                        qtdMateriais,
                        qtdMeds,
                        qtdNM,
                        qtdOPME,
                        qtdServ;
    }

    public class EmissorFechamentoAssocSintReport
    {
        public bool failed = false;

        public long totCreds = 0,
                    totAssocs = 0,

                    _totCoPart = 0,
                    _vlrProcs = 0,
                    _vlrDiaria = 0,
                    _vlrMateriais = 0,
                    _vlrMeds = 0,
                    _vlrNM = 0,
                    _vlrOPME = 0,
                    _vlrServ = 0,
                    _vlrConsultas = 0,
                    _vlrCoPart = 0,
                    _vlrAutos = 0;

        public string   mesAno,
                        stotVlrConsulta,
                        stotVlr,
                        stotCoPart,
                        stot_Procs,
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
                        where e.fkSecao == filter.fkSecao 
                        where e.nuTitularidade == 1
                        orderby e.stName
                        select e;

            LoaderAssocSint(db, query.ToList(), filter, ref ret );
            
            return ret;
		}

        public void LoaderAssocSint ( DevKitDB db, 
                                      List<Associado> lst,
                                      ListagemFechamentoFilter filter,
                                      ref EmissorFechamentoAssocSintReport resGeral )
        {
            var lstIdsSecao = lst.Select(y => y.id).ToList();

            var auts = db.Autorizacao.
                        Where(y => y.fkEmpresa == db.currentUser.fkEmpresa).
                        Where( y=> lstIdsSecao.Contains ((long)y.fkAssociado)).
                        Where(y => y.nuMes == filter.mes).
                        Where(y => y.nuAno == filter.ano).
                        Where(y => y.tgSituacao == filter.tgSituacao).
                        ToList();

            resGeral.failed = !auts.Any();

            if (resGeral.failed)
                return;

            resGeral.mesAno = filter.mes.ToString().PadLeft(2, '0') + " / " + filter.ano.ToString();

            var procsCredTuus = db.CredenciadoEmpresaTuss.
                                    Where(y => y.fkEmpresa == db.currentUser.fkEmpresa).
                                    ToList();

            var procsTuus = db.TUSS.ToList();

            int serial = 1;

            var mon = new money();

            resGeral.totCreds = auts.Select(y => y.fkCredenciado).Distinct().Count();
            resGeral.totAssocs = auts.Select(y => y.fkAssociado).Distinct().Count();

            var secoes = db.EmpresaSecao.ToList();

            var empConsultaValores = db.EmpresaConsultaAno.
                                        Where(y => y.nuAno == filter.ano && y.fkEmpresa == db.currentUser.fkEmpresa).
                                        FirstOrDefault();

            resGeral.results = new List<FechAssocSint>();

            foreach (var assoc in lst)
            {
                var resAssociadoSint = new FechAssocSint
                {
                    serial = serial.ToString(),
                    matricula = assoc.nuMatricula.ToString(),
                    associado = assoc.stName,
                };

                bool found = false;

                long totVlrConsulta = 0, qtConsulta = 0;                     

                foreach (var aut in auts.Where(y => y.fkAssociado == assoc.id && (y.nuTipoAutorizacao == 1 || y.nuTipoAutorizacao == null)).ToList())
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

                if (found)
                {
                    resAssociadoSint.qtdAutos = qtConsulta.ToString();

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
                    
                    resAssociadoSint._vlrConsulta = totVlrConsulta;

                    var q = auts.Where(y => y.fkAssociado == assoc.id).ToList();

                    var q1 = q.Where(y => y.nuTipoAutorizacao == 1 || y.nuTipoAutorizacao == null).ToList();
                    var q2 = q.Where(y => y.nuTipoAutorizacao == 2).ToList();
                    var q3 = q.Where(y => y.nuTipoAutorizacao == 3).ToList();
                    var q4 = q.Where(y => y.nuTipoAutorizacao == 4).ToList();
                    var q5 = q.Where(y => y.nuTipoAutorizacao == 5).ToList();
                    var q6 = q.Where(y => y.nuTipoAutorizacao == 6).ToList();
                    var q7 = q.Where(y => y.nuTipoAutorizacao == 7).ToList();

                    resAssociadoSint._vlrProcs = q1.Sum(y => (long)y.vrParcela);
                    resAssociadoSint._vlrCoPart = q1.Sum(y => (long)y.vrParcelaCoPart);
                    resAssociadoSint.qtdProcs = q1.Count().ToString();
                    resAssociadoSint._vlrDiaria = q2.Sum(y => (long)y.vrParcela);
                    resAssociadoSint.qtdDiaria = q2.Count().ToString();
                    resAssociadoSint._vlrMateriais = q3.Sum(y => (long)y.vrParcela);
                    resAssociadoSint.qtdMateriais = q3.Count().ToString();
                    resAssociadoSint._vlrMeds = q4.Sum(y => (long)y.vrParcela);
                    resAssociadoSint.qtdMeds = q4.Count().ToString();
                    resAssociadoSint._vlrNM = q5.Sum(y => (long)y.vrParcela);
                    resAssociadoSint.qtdNM = q5.Count().ToString();
                    resAssociadoSint._vlrOPME = q6.Sum(y => (long)y.vrParcela);
                    resAssociadoSint.qtdOPME = q6.Count().ToString();
                    resAssociadoSint._vlrServ = q7.Sum(y => (long)y.vrParcela);
                    resAssociadoSint.qtdServ = q7.Count().ToString();

                    resAssociadoSint._vlrGeral = resAssociadoSint._vlrProcs +
                                                 resAssociadoSint._vlrDiaria +
                                                 resAssociadoSint._vlrMateriais +
                                                 resAssociadoSint._vlrMeds +
                                                 resAssociadoSint._vlrNM +
                                                 resAssociadoSint._vlrOPME +
                                                 resAssociadoSint._vlrServ;

                    resAssociadoSint.vlrGeral = mon.setMoneyFormat(resAssociadoSint._vlrGeral);
                    resAssociadoSint.vlrConsulta = mon.setMoneyFormat(resAssociadoSint._vlrConsulta);
                    resAssociadoSint.vlrCoPart = mon.setMoneyFormat(resAssociadoSint._vlrCoPart);

                    resAssociadoSint.vlrProcs = mon.setMoneyFormat(resAssociadoSint._vlrProcs);
                    resAssociadoSint.vlrDiaria = mon.setMoneyFormat(resAssociadoSint._vlrDiaria);
                    resAssociadoSint.vlrMateriais = mon.setMoneyFormat(resAssociadoSint._vlrMateriais);
                    resAssociadoSint.vlrMeds  = mon.setMoneyFormat(resAssociadoSint._vlrMeds);
                    resAssociadoSint.vlrNM = mon.setMoneyFormat(resAssociadoSint._vlrNM);
                    resAssociadoSint.vlrServ = mon.setMoneyFormat(resAssociadoSint._vlrServ);

                    resGeral._vlrAutos += resAssociadoSint._vlrGeral;
                    resGeral._vlrConsultas += resAssociadoSint._vlrConsulta;
                    resGeral._vlrCoPart += resAssociadoSint._vlrCoPart;
                    resGeral._vlrProcs += resAssociadoSint._vlrProcs;
                    resGeral._vlrDiaria += resAssociadoSint._vlrDiaria;
                    resGeral._vlrMateriais += resAssociadoSint._vlrMateriais;
                    resGeral._vlrMeds += resAssociadoSint._vlrMeds;
                    resGeral._vlrNM += resAssociadoSint._vlrNM;
                    resGeral._vlrOPME += resAssociadoSint._vlrOPME;
                    resGeral._vlrServ += resAssociadoSint._vlrServ;

                    resGeral.results.Add(resAssociadoSint);

                    serial++;
                }
            }

            resGeral.stotVlr = mon.setMoneyFormat(resGeral._vlrAutos);
            resGeral.stotVlrConsulta = mon.setMoneyFormat(resGeral._vlrConsultas);
            resGeral.stotCoPart = mon.setMoneyFormat(resGeral._vlrCoPart);

            resGeral.stot_Procs = mon.setMoneyFormat(resGeral._vlrProcs);
            resGeral.stot_Diaria = mon.setMoneyFormat(resGeral._vlrDiaria);
            resGeral.stot_Materiais = mon.setMoneyFormat(resGeral._vlrMateriais);
            resGeral.stot_Meds = mon.setMoneyFormat(resGeral._vlrMeds);
            resGeral.stot_NM = mon.setMoneyFormat(resGeral._vlrNM );
            resGeral.stot_OPME = mon.setMoneyFormat(resGeral._vlrOPME);
            resGeral.stot_Serv = mon.setMoneyFormat(resGeral._vlrServ);
        }
    }
}
