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
                        nuTipo,
                        fkPrecificacao,
                        vrTotal,
                        nuParcelas;

        public DateTime? dataLanc;
    }

    public partial class Empresa
    {
        public string LancaDespesa(DevKitDB db, LancaDespesa_PARAMS _params)
        {
            if (_params.dataLanc == null)
                return "Data inválida";

            if (_params.vrValor == null || _params.vrValor == 0)
                return "Valor inválido";

            if (_params.nuParcelas == null || _params.nuParcelas == 0 || _params.nuParcelas > 12)
                return "Numero parcelas inválido";

            #region - associado - 

            if (_params.matricula == 0)
                return "Matrícula inválida";

            var assoc = db.Associado.FirstOrDefault(y => y.fkEmpresa == db.currentUser.fkEmpresa &&
                                            y.nuMatricula == _params.matricula &&
                                            y.nuTitularidade == 1);

            if (assoc == null)
                return "Matrícula inválida";

            if (assoc.tgStatus == TipoSituacaoCartao.Bloqueado)
                return "Matrícula bloqueada";

            #endregion

            #region - credenciado -

            if (_params.credenciado == 0)
                return "Credenciado inválido";

            var cred = db.Credenciado.FirstOrDefault(y => y.nuCodigo == _params.credenciado);

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

            var aut = new Autorizacao
            {
                dtSolicitacao = dt,
                fkAssociado = assoc.id,
                fkCredenciado = cred.id,
                fkEmpresa = assoc.fkEmpresa,
                fkProcedimento = 0,
                nuAno = dt.Year,
                nuMes = dt.Month,
                nuNSU = nsu,
                tgSituacao = TipoSitAutorizacao.EmAberto,
                fkAutOriginal = null,
                nuIndice = 1,
                nuTotParcelas = 1,
                vrProcedimento = _params.vrValor,
                vrParcela = _params.vrValor,
                vrCoPart = 0,
                vrParcelaCoPart = 0,
                fkAssociadoPortador = assoc.id,
                fkPrecificacao = _params.fkPrecificacao,
                nuTipoAutorizacao = _params.nuTipo
            };

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
                        tgSituacao = TipoSitAutorizacao.EmAberto,
                        fkAutOriginal = aut.id,
                        nuIndice = nuParc,
                        nuTotParcelas = aut.nuTotParcelas,
                        vrProcedimento = 0,
                        vrParcela = _params.vrValor / _params.nuParcelas,
                        vrCoPart = 0,
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
