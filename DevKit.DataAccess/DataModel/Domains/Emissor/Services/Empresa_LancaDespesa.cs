using DevKit.DataAccess;
using LinqToDB;
using System;
using System.Linq;

namespace DataModel
{
    public class LancaDespesa_PARAMS
    {
        public long? matricula,
                        credenciado,
                        vrValor,
                        vrValorCO,
                        nuTipo,
                        nsuRef,
                        fkPrecificacao,
                        vrTotal,
                        nuParcelas;

        public DateTime? dataLanc;
    }

    public partial class Empresa
    {
        public string LancaDespesa(DevKitDB db, LancaDespesa_PARAMS _params)
        {
            Autorizacao refAut = null;

            if (_params.nsuRef != null)
                refAut = db.Autorizacao.Where(y => y.nuNSU == _params.nsuRef).FirstOrDefault();

            if (refAut == null)
                if (_params.dataLanc == null)
                    return "Data inválida";

            if (refAut == null)
                if (_params.vrValor == null || _params.vrValor == 0)
                    return "Valor inválido";

            if (refAut == null)
                if (_params.nuParcelas == null || _params.nuParcelas == 0 || _params.nuParcelas > 12)
                    return "Numero parcelas inválido";

            #region - associado - 

            if (refAut == null)
                if (_params.matricula == 0)
                    return "Matrícula inválida";

            Associado assoc = null;

            if (refAut == null)
                assoc = db.Associado.FirstOrDefault(y => y.fkEmpresa == db.currentUser.fkEmpresa &&
                                            y.nuMatricula == _params.matricula &&
                                            y.nuTitularidade == 1);
            else
                assoc = db.Associado.FirstOrDefault(y => y.id == refAut.fkAssociado);

            if (assoc == null)
                return "Matrícula inválida";

            if (assoc.tgStatus == TipoSituacaoCartao.Bloqueado)
                return "Matrícula bloqueada";

            #endregion

            #region - credenciado -

            Credenciado cred = null;

            if (refAut == null)
                if (_params.credenciado == 0)
                    return "Credenciado inválido";

            if (refAut == null)
                cred = db.Credenciado.FirstOrDefault(y => y.nuCodigo == _params.credenciado);
            else
                cred = db.Credenciado.FirstOrDefault(y => y.id == refAut.fkCredenciado);

            if (cred == null)
                return "Credenciado inválido";

            #endregion

            if (_params.nuTipo == 0)
                return "Tipo de precificação inválido";

            switch (_params.nuTipo)
            {
                case 1: if (!db.SaudeValorProcedimento.Any(y => y.id == _params.fkPrecificacao)) return "Procedimento inválido"; break;
                case 2: if (!db.SaudeValorDiaria.Any(y => y.id == _params.fkPrecificacao)) return "Diária inválida"; break;
                case 3: if (!db.SaudeValorMaterial.Any(y => y.id == _params.fkPrecificacao)) return "Material inválida"; break;
                case 4: if (!db.SaudeValorMedicamento.Any(y => y.id == _params.fkPrecificacao)) return "Material inválida"; break;
                case 5: if (!db.SaudeValorNaoMedico.Any(y => y.id == _params.fkPrecificacao)) return "Não Médico inválido"; break;
                case 6: if (!db.SaudeValorOPME.Any(y => y.id == _params.fkPrecificacao)) return "OPME inválido"; break;
                case 7: if (!db.SaudeValorPacote.Any(y => y.id == _params.fkPrecificacao)) return "Pacote inválido"; break;
            }

            // ------------------
            // obtem nsu
            // ------------------

            var nsu = Convert.ToInt64(db.InsertWithIdentity(new NSU
            {
                dtLog = DateTime.Now,
                fkEmpresa = db.currentUser.fkEmpresa
            }));

            var dt = Convert.ToDateTime(_params.dataLanc);

            if (refAut != null)
                dt = Convert.ToDateTime(refAut.dtSolicitacao);

            var aut = new Autorizacao
            {
                dtSolicitacao = DateTime.Now,
                fkAssociado = assoc.id,
                fkCredenciado = cred.id,
                fkEmpresa = assoc.fkEmpresa,
                fkProcedimento = 0,
                nuAno = dt.Year,
                nuMes = dt.Month,
                nuNSU = nsu,
                nuNSURef = _params.nsuRef,
                tgSituacao = TipoSitAutorizacao.EmAberto,
                fkAutOriginal = null,
                nuIndice = 1,
                nuTotParcelas = 1,
                vrProcedimento = _params.vrValor,
                vrParcela = _params.vrValor,
                vrCoPart = _params.vrValorCO,
                vrParcelaCoPart = 0,
                fkAssociadoPortador = assoc.id,
                fkPrecificacao = _params.fkPrecificacao,
                nuTipoAutorizacao = _params.nuTipo
            };

            if (refAut != null)
                aut.tgSituacao = refAut.tgSituacao;

            aut.id = Convert.ToInt64(db.InsertWithIdentity(aut));

            // ---------------------------
            // distribui em parcelas
            // ---------------------------

            if (_params.nuParcelas > 1)
            {
                for (int nuParc = 2; nuParc <= _params.nuParcelas; ++nuParc)
                {
                    dt = dt.AddMonths(1);

                    db.Insert(new Autorizacao
                    {
                        dtSolicitacao = DateTime.Now,
                        fkAssociado = aut.fkAssociado,
                        fkCredenciado = aut.fkCredenciado,
                        fkEmpresa = aut.fkEmpresa,
                        fkProcedimento = 0,
                        nuAno = dt.Year,
                        nuMes = dt.Month,
                        nuNSU = nsu,
                        nuNSURef = _params.nsuRef,
                        tgSituacao = TipoSitAutorizacao.EmAberto,
                        fkAutOriginal = aut.id,
                        nuIndice = nuParc,
                        nuTotParcelas = aut.nuTotParcelas,
                        vrProcedimento = 0,
                        vrParcela = _params.vrValor / _params.nuParcelas,
                        vrCoPart = _params.vrValorCO / _params.nuParcelas,
                        vrParcelaCoPart = 0,
                        fkAssociadoPortador = aut.fkAssociadoPortador,
                        fkPrecificacao = aut.fkPrecificacao,
                        nuTipoAutorizacao = aut.nuTipoAutorizacao
                    });
                }
            }

            return "";
        }
    }
}
