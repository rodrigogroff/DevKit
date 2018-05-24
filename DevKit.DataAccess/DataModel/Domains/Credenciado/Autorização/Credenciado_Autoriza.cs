using DevKit.DataAccess;
using LinqToDB;
using SyCrafEngine;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace DataModel
{
    public class AutorizaProcedimentoParams
    {
        public string ca, senha, titVia;
        public long emp, mat, tuss, codigoCred;

        public string Export()
        {
            return "EMP: " + emp + " MAT: " + mat + " CA: " + ca + " SENHA: " + senha + " TITVIA: " + titVia + " TUSS:" + tuss + " CCRED:" + codigoCred;
        } 
    }

    public class CupomAutorizacao
    {
        public bool ok = false;
        public string resp = "";

        // campos
        public string   validade = "",
                        empresa = "",
                        emissao = "",
                        autorizacao = "",
                        associadoMat = "",
                        associadoNome = "",
                        associadoTit = "",
                        credenciado = "",
                        secao = "",
                        tuss = "",
                        procedimento = "",
                        vrIntegral = "",
                        vrCoPart = "";
    }

    public partial class Credenciado
    {        
        public CupomAutorizacao AutorizaProcedimento ( DevKitDB db, AutorizaProcedimentoParams _params )
		{
            var cupom = new CupomAutorizacao();
            var util = new Util();

            util.SetupFile();
            util.Registry(" --- AutorizaProcedimento --- ");

            try
            {
                var nuTit = Convert.ToInt32(_params.titVia.Substring(0, 2));
                var nuVia = Convert.ToInt32(_params.titVia.Substring(2, 2));

                var secaoTb = (from e in db.EmpresaSecao
                               where e.nuEmpresa == _params.emp
                               select e).
                               FirstOrDefault();

                if (secaoTb == null)
                {
                    cupom.ok = false;
                    cupom.resp = "Empresa inválida";
                    return cupom;
                }                    

                var empTb = (from e in db.Empresa
                             where e.id == secaoTb.fkEmpresa
                             select e).
                             FirstOrDefault();

                var associadoPortador = (from e in db.Associado
                                 where e.fkEmpresa == secaoTb.fkEmpresa
                                 where e.nuMatricula == _params.mat
                                 where e.nuTitularidade == nuTit
                                 where e.nuViaCartao == nuVia
                                 select e).
                                 FirstOrDefault();

                if (associadoPortador == null)
                {
                    cupom.ok = false;
                    cupom.resp = "Matrícula inválida";

                    return cupom;
                }

                var caCalc = util.calculaCodigoAcesso(secaoTb.nuEmpresa.ToString().PadLeft(6, '0'),
                                                        _params.mat.ToString().PadLeft(6, '0'),
                                                        associadoPortador.nuTitularidade.ToString(),
                                                        associadoPortador.nuViaCartao.ToString(),
                                                        associadoPortador.stCPF);

                if (_params.ca != caCalc)
                {
                    cupom.ok = false;
                    cupom.resp = "Dados do cartão inválidos!";

                    return cupom;
                }
                    
                if (associadoPortador.nuTitularidade > 1)
                {
                    var dadosDep = db.AssociadoDependente.FirstOrDefault(y => y.fkCartao == associadoPortador.id);

                    /*
                        1)ESPOSA 
                        2)FILHOS MENORES DE 21 ANOS
                        3)FILHOS MAIORES DE 21 ESTUDANTES
                        4)FILHOS MAIORES DE 21 ANOS COM DOENCA PRE EXISTENTES
                    */

                    if (dadosDep.dtNasc != null)
                    {
                        var dtNascDep = Convert.ToDateTime(dadosDep.dtNasc);

                        switch (dadosDep.fkTipoCoberturaDependente)
                        {
                            case 2:
                                if (DateTime.Now > dtNascDep.AddYears(21))
                                {
                                    cupom.ok = false;
                                    cupom.resp = "Idade do dependente excede 21!";

                                    return cupom;
                                }
                                    
                                break;

                            case 3:
                            case 4:
                                if (DateTime.Now < dtNascDep.AddYears(21))
                                {
                                    cupom.ok = false;
                                    cupom.resp = "Idade do dependente precisa ser maior que 21!";

                                    return cupom;
                                }
                                
                                break;

                            default: break;
                        }
                    }
                }

                var associadoTit = (from e in db.Associado
                                    where e.fkEmpresa == secaoTb.fkEmpresa
                                    where e.nuMatricula == _params.mat
                                    where e.nuTitularidade == 1
                                    select e).
                                    FirstOrDefault();

                if (_params.senha != associadoTit.stSenha)
                {
                    cupom.ok = false;
                    cupom.resp = "Senha inválida!";

                    return cupom;
                }                    

                if (associadoPortador.tgStatus == TipoSituacaoCartao.Bloqueado)
                {
                    cupom.ok = false;
                    cupom.resp = "Cartão bloqueado!";

                    return cupom;
                }
                    
                var curCred = db.currentCredenciado;

                if (_params.codigoCred > 0)
                {
                    curCred = db.Credenciado.
                                Where(y => y.nuCodigo == _params.codigoCred).
                                FirstOrDefault();
                }

                if (curCred != null)
                if (!db.CredenciadoEmpresa.Any(y => y.fkCredenciado == curCred.id &&
                                               y.fkEmpresa == secaoTb.fkEmpresa))
                {
                    cupom.ok = false;
                    cupom.resp = "Credenciado não conveniado à empresa " + _params.emp;

                    return cupom;
                }

                // -------------------------------
                // pronto para cupom!
                // -------------------------------

                var dt = DateTime.Now;

                cupom.validade = empTb.nuDiaFech.ToString().PadLeft(2,'0') + " / " +
                                 DateTime.Now.AddMonths(1).Month.ToString().PadLeft(2, '0') + " / " + 
                                 DateTime.Now.AddMonths(1).Year.ToString();

                cupom.empresa = empTb.stNome;
                cupom.secao = secaoTb.nuEmpresa.ToString();

                if (curCred != null)
                    cupom.credenciado = curCred.stNome;

                cupom.associadoMat = associadoPortador.nuMatricula.ToString();
                cupom.associadoNome = associadoPortador.stName.ToString();
                cupom.associadoTit = associadoPortador.nuTitularidade.ToString();
                cupom.emissao = dt.ToString("dd/MM/yyyy HH:mm");
                
                var tuss = db.TUSS.Where(y => y.nuCodTUSS == _params.tuss).FirstOrDefault();

                if (tuss == null)
                {
                    var nsu = Convert.ToInt64(db.InsertWithIdentity(new NSU
                    {
                        dtLog = DateTime.Now,
                        fkEmpresa = associadoTit.fkEmpresa
                    }));

                    cupom.autorizacao = nsu.ToString();

                    var idAutOriginal = Convert.ToInt64(db.InsertWithIdentity(new Autorizacao
                    {
                        dtSolicitacao = DateTime.Now,
                        fkAssociado = associadoTit.id,
                        fkCredenciado = curCred != null ? (long?)curCred.id : null,
                        fkEmpresa = associadoTit.fkEmpresa,
                        fkProcedimento = 0,
                        nuAno = dt.Year,
                        nuMes = dt.Month,
                        nuNSU = nsu,
                        tgSituacao = TipoSitAutorizacao.Autorizado,
                        fkAutOriginal = null,
                        nuIndice = 1,
                        nuTotParcelas = 0,
                        vrProcedimento = 0,
                        vrParcela = 0,
                        vrCoPart = 0,
                        vrParcelaCoPart = 0,
                        fkAssociadoPortador = associadoPortador.id
                    }));
                }
                else
                {
                    if (curCred == null)
                    {
                        cupom.ok = false;
                        cupom.resp = "Requer credenciado!";

                        return cupom;
                    }
                                        
                    var proc = db.CredenciadoEmpresaTuss.
                                Where(y => y.fkCredenciado == curCred.id).
                                Where(y => y.fkEmpresa == associadoPortador.fkEmpresa).
                                Where(y => y.nuTUSS == tuss.nuCodTUSS).
                                FirstOrDefault();

                    if (proc == null)
                    {
                        cupom.ok = false;
                        cupom.resp = "TUSS inválido (" + tuss.nuCodTUSS  + ")";

                        return cupom;
                    }

                    cupom.tuss = proc.nuTUSS.ToString();
                    cupom.procedimento = proc.stProcedimento;

                    cupom.vrIntegral = new money().setMoneyFormat((long)proc.vrProcedimento);
                    cupom.vrCoPart = new money().setMoneyFormat((long)proc.vrCoPart);

                    if (dt.Day < empTb.nuDiaFech)
                        dt = dt.AddMonths(-1);

                    var nsu = Convert.ToInt64(db.InsertWithIdentity(new NSU
                    {
                        dtLog = DateTime.Now,
                        fkEmpresa = associadoTit.fkEmpresa
                    }));

                    cupom.autorizacao = nsu.ToString();

                    var idAutOriginal = Convert.ToInt64(db.InsertWithIdentity(new Autorizacao
                    {
                        dtSolicitacao = DateTime.Now,
                        fkAssociado = associadoTit.id,
                        fkCredenciado = curCred.id,
                        fkEmpresa = associadoTit.fkEmpresa,
                        fkProcedimento = tuss.id,
                        nuAno = dt.Year,
                        nuMes = dt.Month,
                        nuNSU = nsu,
                        tgSituacao = TipoSitAutorizacao.Autorizado,
                        fkAutOriginal = null,
                        nuIndice = 1,
                        nuTotParcelas = proc.nuParcelas,
                        vrProcedimento = proc.vrProcedimento,
                        vrParcela = proc.vrProcedimento / proc.nuParcelas,
                        vrCoPart = proc.vrCoPart,
                        vrParcelaCoPart = proc.vrCoPart / proc.nuParcelas,
                        fkAssociadoPortador = associadoPortador.id
                    }));

                    if (proc.nuParcelas > 1)
                    {
                        for (int nuParc = 2; nuParc <= proc.nuParcelas; ++nuParc)
                        {
                            dt = dt.AddMonths(1);

                            db.Insert(new Autorizacao
                            {
                                dtSolicitacao = DateTime.Now,
                                fkAssociado = associadoPortador.id,
                                fkCredenciado = curCred.id,
                                fkEmpresa = associadoPortador.fkEmpresa,
                                fkProcedimento = tuss.id,
                                nuAno = dt.Year,
                                nuMes = dt.Month,
                                nuNSU = nsu,
                                tgSituacao = TipoSitAutorizacao.Autorizado,
                                fkAutOriginal = idAutOriginal,
                                nuIndice = nuParc,
                                nuTotParcelas = proc.nuParcelas,
                                vrProcedimento = proc.vrProcedimento,
                                vrParcela = proc.vrProcedimento / proc.nuParcelas,
                                vrCoPart = proc.vrCoPart,
                                vrParcelaCoPart = proc.vrCoPart / proc.nuParcelas,
                                fkAssociadoPortador = associadoPortador.id
                            });
                        }
                    }
                }

                cupom.ok = true;

                util.Registry("--- Fim! ");

                util.CloseFile();

                return cupom;
            }
            catch (SystemException ex)
            {
                util.ErrorRegistry(" *ERROR: " + ex.ToString());

                cupom.ok = false;
                cupom.resp = ex.ToString();

                util.CloseFile();

                return cupom;
            }
		}
	}
}
