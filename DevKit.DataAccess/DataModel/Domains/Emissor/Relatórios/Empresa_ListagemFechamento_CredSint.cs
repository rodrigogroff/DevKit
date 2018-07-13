using DevKit.DataAccess;
using LinqToDB;
using SyCrafEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
    public class ListagemFechamentoFilter
    {
        public long? fkEmpresa, fkCredenciado;

        public int mes,
                   ano,
                   tipo,
                   modo,
                   tgSituacao,
                   fkSecao;

        public string codCred, mat;
    }

    public class FechCredSint
    {
        public long     _vlrTotal = 0,
                        _vlrCoPart = 0,            
                        _vlrProcs = 0,
                        _vlrDiaria = 0,
                        _vlrMateriais = 0,
                        _vlrMeds = 0,
                        _vlrNM = 0,
                        _vlrOPME = 0,
                        _vlrServ = 0;

        public string serial,
                        cpfcnpj,
                        codigoCred,
                        nomeCred,
                        especialidade,
                        qtdAutos,                        
                        vlrCoPart,
                        pcads,
                        ncads,

                        vlrTotal,
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

    public class EmissorFechamentoCredSintReport
    {
        public bool failed = false;

        public long totCreds = 0,
                    totAssociados = 0,

                    _totVlr,
                    _totVlrCoPart,
                    _totProcs,
                    _totDiaria,
                    _totMateriais,
                    _totMeds,
                    _totNM,
                    _totOPME,
                    _totServ,

                    procsNCad = 0;

        public string   stotVlr, 
                        stotCoPart,
                        mesAno,                        

                        stot_Procs,            
                        stot_Diaria,
                        stot_Materiais,
                        stot_Meds,
                        stot_NM,
                        stot_OPME,
                        stot_Serv;

        public List<FechCredSint> results = new List<FechCredSint>();
    }

    public partial class Empresa
    {
		public EmissorFechamentoCredSintReport ListagemFechamento_CredSint ( DevKitDB db, ListagemFechamentoFilter filter )
		{
            var ret = new EmissorFechamentoCredSintReport();

            var query = from e in db.Credenciado
                        join ce in db.CredenciadoEmpresa on e.id equals ce.fkCredenciado
                        where ce.fkEmpresa == db.currentUser.fkEmpresa
                        where filter.codCred == null || e.nuCodigo == Convert.ToInt64(filter.codCred)
                        orderby e.stNome
                        select e;

            LoaderCredSint(db, query.ToList(), filter, ref ret );
            
            return ret;
		}

        public void LoaderCredSint ( DevKitDB db, 
                                     List<Credenciado> lst,
                                     ListagemFechamentoFilter filter,
                                     ref EmissorFechamentoCredSintReport resGeral )
        {
            var auts = db.Autorizacao.
                        Where(y => y.fkEmpresa == db.currentUser.fkEmpresa).
                        Where(y => y.nuMes == filter.mes).
                        Where(y => y.nuAno == filter.ano).
                        Where(y => y.tgSituacao == filter.tgSituacao).
                        ToList();

            resGeral.failed = !auts.Any();

            resGeral.mesAno = filter.mes.ToString().PadLeft(2, '0') + " / " + filter.ano.ToString();

            if (resGeral.failed)
                return;

            var procsCredTuus = db.CredenciadoEmpresaTuss.
                                    Where(y => y.fkEmpresa == db.currentUser.fkEmpresa).
                                    ToList();

            var procsTuus = db.TUSS.ToList();

            var lstEspecs = db.Especialidade.ToList();

            int serial = 1;

            var mon = new money();

            resGeral.totCreds = auts.Select(y => y.fkCredenciado).Distinct().Count();
            resGeral.totAssociados = auts.Select(y => y.fkAssociado).Distinct().Count();

            resGeral.results = new List<FechCredSint>();

            foreach (var cred in lst)
            {
                var resSintCred = new FechCredSint
                {
                    serial = serial.ToString(),
                    cpfcnpj = cred.stCnpj,
                    codigoCred = cred.nuCodigo.ToString(),
                    nomeCred = cred.stNome,
                    especialidade = cred.fkEspecialidade != null ? lstEspecs.Where (y=> y.id == cred.fkEspecialidade).FirstOrDefault().stNome : "",
                    qtdAutos = auts.Where(y => y.fkCredenciado == cred.id).Count().ToString()
                };

                bool found = false;

                resSintCred.pcads = "";
                resSintCred.ncads = "";

                #region - ncads -

                foreach (var aut in auts.Where(y => y.fkCredenciado == cred.id ).ToList())
                {
                    found = true;

                    TUSS fkProc = procsTuus.
                                    Where(y => y.id == aut.fkProcedimento).
                                    FirstOrDefault();

                    if (fkProc == null)
                    {
                        if (aut.nuTipoAutorizacao == 1 || aut.nuTipoAutorizacao == null)
                        {
                            if (aut.fkPrecificacao != null)
                                fkProc = procsTuus.
                                        Where(y => y.id == aut.fkPrecificacao).
                                        FirstOrDefault();
                        }
                    }

                    if (fkProc != null)
                    {
                        var strTUSs = fkProc.nuCodTUSS.ToString();

                        var cfgTuss = procsCredTuus.
                                        Where(y => y.fkCredenciado == cred.id).
                                        Where(y => y.nuTUSS == fkProc.nuCodTUSS).
                                        FirstOrDefault();

                        if (cfgTuss != null)
                        {
                            if (!resSintCred.pcads.Contains(strTUSs))
                                resSintCred.pcads += strTUSs + ", ";                            
                        }
                        else
                        {
                            if (!resSintCred.ncads.Contains(strTUSs))
                                resSintCred.ncads += strTUSs + ", ";
                            resGeral.procsNCad++;
                        }                            
                    }
                }

                #endregion

                if (found)
                {
                    var q = auts.Where(y => y.fkCredenciado == cred.id).
                                 Where (y=> y.tgSituacao == filter.tgSituacao).
                                 ToList();

                    var q1 = q.Where(y => y.nuTipoAutorizacao == 1 || y.nuTipoAutorizacao == null).ToList();
                    var q2 = q.Where(y => y.nuTipoAutorizacao == 2).ToList();
                    var q3 = q.Where(y => y.nuTipoAutorizacao == 3).ToList();
                    var q4 = q.Where(y => y.nuTipoAutorizacao == 4).ToList();
                    var q5 = q.Where(y => y.nuTipoAutorizacao == 5).ToList();
                    var q6 = q.Where(y => y.nuTipoAutorizacao == 6).ToList();
                    var q7 = q.Where(y => y.nuTipoAutorizacao == 7).ToList();

                    resSintCred._vlrProcs = q1.Sum(y => (long)y.vrParcela); ;
                    resSintCred.qtdProcs = q1.Count().ToString();

                    resSintCred._vlrCoPart = q1.Sum(y => (long)y.vrParcelaCoPart); ;

                    resSintCred._vlrDiaria = q2.Sum(y => (long)y.vrParcela); ;
                    resSintCred.qtdDiaria = q2.Count().ToString();

                    resSintCred._vlrMateriais = q3.Sum(y => (long)y.vrParcela); ;
                    resSintCred.qtdMateriais = q3.Count().ToString();

                    resSintCred._vlrMeds = q4.Sum(y => (long)y.vrParcela); ;
                    resSintCred.qtdMeds = q4.Count().ToString();

                    resSintCred._vlrNM = q5.Sum(y => (long)y.vrParcela); ;
                    resSintCred.qtdNM = q5.Count().ToString();

                    resSintCred._vlrOPME = q6.Sum(y => (long)y.vrParcela); ;
                    resSintCred.qtdOPME = q6.Count().ToString();

                    resSintCred._vlrServ = q7.Sum(y => (long)y.vrParcela); ;
                    resSintCred.qtdServ = q7.Count().ToString();

                    resSintCred._vlrTotal =     resSintCred._vlrProcs + 
                                                resSintCred._vlrDiaria + 
                                                resSintCred._vlrMateriais + 
                                                resSintCred._vlrMeds + 
                                                resSintCred._vlrNM +
                                                resSintCred._vlrOPME +
                                                resSintCred._vlrServ;

                    resSintCred.vlrTotal = mon.setMoneyFormat(resSintCred._vlrTotal);
                    resSintCred.vlrCoPart = mon.setMoneyFormat(resSintCred._vlrCoPart);
                    resSintCred.vlrProcs = mon.setMoneyFormat(resSintCred._vlrProcs);
                    resSintCred.vlrDiaria = mon.setMoneyFormat(resSintCred._vlrDiaria);
                    resSintCred.vlrMateriais = mon.setMoneyFormat(resSintCred._vlrMateriais);
                    resSintCred.vlrMeds = mon.setMoneyFormat(resSintCred._vlrMeds);
                    resSintCred.vlrNM = mon.setMoneyFormat(resSintCred._vlrNM);
                    resSintCred.vlrOPME = mon.setMoneyFormat(resSintCred._vlrOPME);
                    resSintCred.vlrServ = mon.setMoneyFormat(resSintCred._vlrServ);

                    resSintCred.pcads = resSintCred.pcads.Trim().TrimEnd(',');
                    resSintCred.ncads = resSintCred.ncads.Trim().TrimEnd(',');

                    resGeral._totVlr += resSintCred._vlrTotal;
                    resGeral._totVlrCoPart += resSintCred._vlrCoPart;
                    resGeral._totProcs += resSintCred._vlrProcs;
                    resGeral._totDiaria += resSintCred._vlrDiaria;
                    resGeral._totMateriais += resSintCred._vlrMateriais;
                    resGeral._totMeds += resSintCred._vlrMeds;
                    resGeral._totNM += resSintCred._vlrNM;
                    resGeral._totOPME += resSintCred._vlrOPME;
                    resGeral._totServ += resSintCred._vlrServ;

                    resGeral.results.Add(resSintCred);

                    serial++;
                }
            }

            resGeral.stotVlr = mon.setMoneyFormat(resGeral._totVlr);
            resGeral.stotCoPart = mon.setMoneyFormat(resGeral._totVlrCoPart);
            resGeral.stot_Procs = mon.setMoneyFormat(resGeral._totProcs);
            resGeral.stot_Diaria = mon.setMoneyFormat(resGeral._totDiaria);
            resGeral.stot_Materiais = mon.setMoneyFormat(resGeral._totMateriais);
            resGeral.stot_Meds = mon.setMoneyFormat(resGeral._totMeds);
            resGeral.stot_NM = mon.setMoneyFormat(resGeral._totNM);
            resGeral.stot_OPME = mon.setMoneyFormat(resGeral._totOPME);
            resGeral.stot_Serv = mon.setMoneyFormat(resGeral._totServ);
        }
    }
}
