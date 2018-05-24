using DevKit.DataAccess;
using LinqToDB;
using SyCrafEngine;
using System;
using System.Linq;

namespace DataModel
{
    public class AutorizaProcedimentoParams
    {
        public string ca, senha, titVia;
        public long emp, mat, tuss, codigoCred;

        public string Export()
        {
            return "EMP: " + emp + 
                   " MAT: " + mat + 
                   " CA: " + ca + 
                   " SENHA: " + senha + 
                   " TITVIA: " + titVia + 
                   " TUSS:" + tuss + 
                   " CCRED:" + codigoCred;
        } 
    }

    public class CupomAutorizacao
    {
        public bool ok = false;

        public string   resp = "",
                        validade = "",
                        empresa = "",
                        emissao = "",
                        autorizacao = "",
                        associadoMat = "",
                        associadoNome = "",
                        associadoTit = "",
                        associadoMatSaude = "",
                        credenciado = "",
                        secao = "",
                        tuss = "",
                        procedimento = "",
                        vrIntegral = "",
                        vrCoPart = "";

        public string Export()
        {
            return  " ok: " + ok +
                    " resp: '" + resp +
                    "' validade: '" + validade +
                    "' empresa: '" + empresa +
                    "' emissao: '" + emissao +
                    "' autorizacao: '" + autorizacao +
                    "' associadoMat: '" + associadoMat +
                    "' associadoNome: '" + associadoNome +
                    "' associadoTit: '" + associadoTit +
                    "' associadoMatSaude: '" + associadoMatSaude +
                    "' credenciado: '" + credenciado +
                    "' secao: '" + secao +
                    "' tuss: '" + tuss +
                    "' procedimento: '" + procedimento +
                    "' vrIntegral: '" + vrIntegral +
                    "' vrCoPart: '" + vrCoPart + 
                    "'";
        }
    }

    public partial class Credenciado
    {        
        public CupomAutorizacao AutorizaProcedimento ( DevKitDB db, AutorizaProcedimentoParams _params )
		{
            var cupom = new CupomAutorizacao();
            var util = new Util();

            util.SetupFile();
            util.Registry(" --- AutorizaProcedimento --- ");
            util.Registry("PARAMS");
            util.Registry(_params.Export());
            util.Registry(" ------------------------------------ ");

            try
            {
                var nuTit = Convert.ToInt32(_params.titVia.Substring(0, 2));
                var nuVia = Convert.ToInt32(_params.titVia.Substring(2, 2));

                var secaoTb = db.EmpresaSecao.FirstOrDefault(y => y.nuEmpresa == _params.emp);

                util.Registry("db.EmpresaSecao.FirstOrDefault(y => y.nuEmpresa == " + _params.emp );

                if (secaoTb == null)
                {
                    cupom.ok = false;
                    cupom.resp = "Seção inválida";

                    util.ErrorRegistry(cupom.resp);
                    util.CloseFile();

                    return cupom;
                }

                var empTb = db.Empresa.FirstOrDefault(y => y.id == secaoTb.fkEmpresa);

                util.Registry("db.Empresa.FirstOrDefault(y => y.id == " + secaoTb.fkEmpresa);

                if (empTb == null)
                {
                    cupom.ok = false;
                    cupom.resp = "Empresa inválida";

                    util.ErrorRegistry(cupom.resp);
                    util.CloseFile();

                    return cupom;
                }

                var associadoPortador = db.Associado.FirstOrDefault ( e => e.fkEmpresa == secaoTb.fkEmpresa && 
                                                                      e.nuMatricula == _params.mat && 
                                                                      e.nuTitularidade == nuTit && 
                                                                      e.nuViaCartao == nuVia );

                util.Registry("db.Associado.FirstOrDefault(e => e.fkEmpresa == " + secaoTb.fkEmpresa + " && e.nuMatricula == " + _params.mat + " && e.nuTitularidade == " + nuTit + " && e.nuViaCartao == " + nuVia);

                if (associadoPortador == null)
                {
                    cupom.ok = false;
                    cupom.resp = "Matrícula inválida";

                    util.ErrorRegistry(cupom.resp);
                    util.CloseFile();

                    return cupom;
                }

                util.Registry("calculaCodigoAcesso");

                var caCalc = util.calculaCodigoAcesso(secaoTb.nuEmpresa.ToString().PadLeft(6, '0'),
                                                        _params.mat.ToString().PadLeft(6, '0'),
                                                        associadoPortador.nuTitularidade.ToString(),
                                                        associadoPortador.nuViaCartao.ToString(),
                                                        associadoPortador.stCPF);

                util.Registry("_params.ca != caCalc");
                util.Registry(_params.ca + " != " + caCalc);

                if (_params.ca != caCalc)
                {
                    cupom.ok = false;
                    cupom.resp = "Dados do cartão inválidos!";

                    util.ErrorRegistry(cupom.resp);
                    util.CloseFile();

                    return cupom;
                }

                util.Registry("associadoPortador.nuTitularidade " + associadoPortador.nuTitularidade);

                if (associadoPortador.nuTitularidade > 1)
                {
                    util.Registry("dependente");

                    var dadosDep = db.AssociadoDependente.FirstOrDefault(y => y.fkCartao == associadoPortador.id);

                    util.Registry("db.AssociadoDependente.FirstOrDefault(y => y.fkCartao == " + associadoPortador.id);
                    
                    if (dadosDep == null)
                    {
                        cupom.ok = false;
                        cupom.resp = "Dados do cartão dependente inválidos!";

                        util.ErrorRegistry(cupom.resp);
                        util.CloseFile();

                        return cupom;
                    }

                    if (dadosDep.dtNasc != null)
                    {
                        util.Registry("dadosDep.dtNasc " + dadosDep.dtNasc);

                        var dtNascDep = Convert.ToDateTime(dadosDep.dtNasc);

                        util.Registry("dadosDep.fkTipoCoberturaDependente " + dadosDep.fkTipoCoberturaDependente);

                        switch (dadosDep.fkTipoCoberturaDependente)
                        {
                            case 2:
                                if (DateTime.Now > dtNascDep.AddYears(21))
                                {
                                    cupom.ok = false;
                                    cupom.resp = "Idade do dependente excede 21!";

                                    util.ErrorRegistry(cupom.resp);
                                    util.CloseFile();

                                    return cupom;
                                }
                                    
                                break;

                            case 3:
                            case 4:

                                if (DateTime.Now < dtNascDep.AddYears(21))
                                {
                                    cupom.ok = false;
                                    cupom.resp = "Idade do dependente precisa ser maior que 21!";

                                    util.ErrorRegistry(cupom.resp);
                                    util.CloseFile();

                                    return cupom;
                                }
                                
                                break;

                            default: break;
                        }
                    }
                }

                var associadoTit = db.Associado.FirstOrDefault(e => e.fkEmpresa == secaoTb.fkEmpresa && 
                                                                    e.nuMatricula == _params.mat &&
                                                                    e.nuTitularidade == 1 );

                util.Registry("db.Associado.FirstOrDefault(e => e.fkEmpresa == " + secaoTb.fkEmpresa + " && e.nuMatricula == " + _params.mat + " && e.nuTitularidade == 1)");

                if (associadoTit == null)
                {
                    cupom.ok = false;
                    cupom.resp = "Titular inválido!";

                    util.ErrorRegistry(cupom.resp);
                    util.CloseFile();

                    return cupom;
                }

                util.Registry("_params.senha != associadoTit.stSenha");
                util.Registry(_params.senha + " != " + associadoTit.stSenha);

                if ( _params.senha != associadoTit.stSenha )
                {
                    cupom.ok = false;
                    cupom.resp = "Senha inválida!";

                    util.ErrorRegistry(cupom.resp);
                    util.CloseFile();

                    return cupom;
                }

                util.Registry("associadoPortador.tgStatus: " + (associadoPortador.tgStatus == null ? "NULO" : associadoPortador.tgStatus.ToString()));

                if (associadoPortador.tgStatus == TipoSituacaoCartao.Bloqueado)
                {
                    cupom.ok = false;
                    cupom.resp = "Cartão bloqueado!";

                    util.ErrorRegistry(cupom.resp);
                    util.CloseFile();

                    return cupom;
                }
                    
                var curCred = db.currentCredenciado;

                util.Registry("db.currentCredenciado: " + (db.currentCredenciado == null ? "NULO" : db.currentCredenciado.id.ToString()));

                util.Registry("_params.codigoCred: " + (_params.codigoCred == 0 ? "ZERO" : _params.codigoCred.ToString()));

                if (_params.codigoCred > 0)
                    curCred = db.Credenciado.FirstOrDefault(y => y.nuCodigo == _params.codigoCred);

                if (curCred == null)
                {
                    cupom.ok = false;
                    cupom.resp = "Requer credenciado!";

                    util.ErrorRegistry(cupom.resp);
                    util.CloseFile();

                    return cupom;
                }

                util.Registry("!db.CredenciadoEmpresa.Any(y => y.fkCredenciado == " + curCred.id + " && y.fkEmpresa == " + secaoTb.fkEmpresa);

                if (!db.CredenciadoEmpresa.Any(y => y.fkCredenciado == curCred.id && y.fkEmpresa == secaoTb.fkEmpresa))
                {
                    cupom.ok = false;
                    cupom.resp = "Credenciado não conveniado à empresa " + _params.emp;

                    util.ErrorRegistry(cupom.resp);
                    util.CloseFile();

                    return cupom;
                }
                else
                    util.Registry("Conveniado!");

                // -------------------------------
                // pronto para cupom!
                // -------------------------------

                var dt = DateTime.Now;

                cupom.validade = empTb.nuDiaFech.ToString().PadLeft(2,'0') + " / " +
                                 DateTime.Now.AddMonths(1).Month.ToString().PadLeft(2, '0') + " / " + 
                                 DateTime.Now.AddMonths(1).Year.ToString();

                cupom.empresa = empTb.stNome;
                cupom.secao = secaoTb.nuEmpresa.ToString();
                cupom.credenciado = curCred.stNome;

                cupom.associadoMat = associadoPortador.nuMatricula.ToString();
                cupom.associadoNome = associadoPortador.stName.ToString();
                cupom.associadoTit = associadoPortador.nuTitularidade.ToString();
                cupom.associadoMatSaude = associadoPortador.nuMatSaude != null ? associadoPortador.nuMatSaude.ToString() : "";

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

                    cupom.tuss = "(NÃO INFORMADO)";
                    cupom.procedimento = "(NÃO INFORMADO)";
                    cupom.vrCoPart = "--------";
                    cupom.vrIntegral = "--------";

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
                        nuTotParcelas = 1,
                        vrProcedimento = 0,
                        vrParcela = 0,
                        vrCoPart = 0,
                        vrParcelaCoPart = 0,
                        fkAssociadoPortador = associadoPortador.id
                    }));
                }
                else
                {
                    var proc = db.CredenciadoEmpresaTuss.
                                Where(y => y.fkCredenciado == curCred.id).
                                Where(y => y.fkEmpresa == associadoPortador.fkEmpresa).
                                Where(y => y.nuTUSS == tuss.nuCodTUSS).
                                FirstOrDefault();

                    if (proc == null)
                    {
                        cupom.ok = false;
                        cupom.resp = "TUSS inválido (" + tuss.nuCodTUSS  + ")";

                        util.ErrorRegistry(cupom.resp);
                        util.CloseFile();

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

                util.Registry( "CUPOM: " + cupom.Export());
                util.Registry("--- Fim! ");

                util.CloseFile();

                return cupom;
            }
            catch (SystemException ex)
            {
                cupom.ok = false;
                cupom.resp = ex.ToString();

                util.ErrorRegistry(" *ERROR: " + ex.ToString());

                util.CloseFile();

                return cupom;
            }
		}
	}
}
