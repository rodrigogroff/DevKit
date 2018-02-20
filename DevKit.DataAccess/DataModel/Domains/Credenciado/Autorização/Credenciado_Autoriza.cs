using DevKit.DataAccess;
using LinqToDB;
using System;
using System.Linq;

namespace DataModel
{
    public class AutorizaProcedimentoParams
    {
        public string ca, senha, titVia;
        public long emp, mat, tuss;
    }

    public partial class Credenciado
    {
		public string AutorizaProcedimento ( DevKitDB db, AutorizaProcedimentoParams _params )
		{
            var util = new Util();

            var nuTit = Convert.ToInt32(_params.titVia.Substring(0, 2));
            var nuVia = Convert.ToInt32(_params.titVia.Substring(2, 2));

            var secaoTb = (from e in db.EmpresaSecao
                           where e.nuEmpresa == _params.emp
                           select e).
                           FirstOrDefault();

            if (secaoTb == null)
                return "Empresa inválida";

            var empTb = (from e in db.Empresa where e.id == secaoTb.fkEmpresa select e).FirstOrDefault();

            var associado = (from e in db.Associado
                             where e.fkEmpresa == secaoTb.fkEmpresa
                             where e.nuMatricula == _params.mat
                             where e.nuTitularidade == nuTit
                             where e.nuViaCartao == nuVia
                             select e).
                             FirstOrDefault();

            if (associado == null)
                return "Matrícula inválida";

            var caCalc = util.calculaCodigoAcesso ( secaoTb.nuEmpresa.ToString().PadLeft(6, '0'),
                                                    _params.mat.ToString().PadLeft(6, '0'),
                                                    associado.nuTitularidade.ToString(),
                                                    associado.nuViaCartao.ToString(),
                                                    associado.stCPF );

            if (_params.ca != caCalc)
                return "Dados do cartão inválidos!";

            var associadoTit = (from e in db.Associado
                                where e.fkEmpresa == secaoTb.fkEmpresa
                                where e.nuMatricula == _params.mat
                                where e.nuTitularidade == 1
                                select e).
                             FirstOrDefault();

            if (_params.senha != associadoTit.stSenha)
                return "Senha inválida!";

            if (associado.tgStatus == TipoSituacaoCartao.Bloqueado)
                return "Cartão bloqueado!";

            if (!db.CredenciadoEmpresa.Any(y => y.fkCredenciado == db.currentCredenciado.id &&
                                           y.fkEmpresa == secaoTb.fkEmpresa))
                return "Credenciado não conveniado à empresa " + _params.emp;

            var tuss = db.TUSS.Where(y => y.nuCodTUSS == _params.tuss).FirstOrDefault();

            if (tuss == null)
                return "Procedimento " + _params.tuss + " inválido!";

            var proc = db.CredenciadoEmpresaTuss.
                            Where(y => y.fkCredenciado == db.currentCredenciado.id).
                            Where(y => y.fkEmpresa == associado.fkEmpresa).
                            Where(y => y.nuTUSS == tuss.nuCodTUSS).
                            FirstOrDefault();

            DateTime dt = DateTime.Now;

            if (dt.Day < empTb.nuDiaFech)
                dt = dt.AddMonths(-1);

            var idAutOriginal = Convert.ToInt64(db.InsertWithIdentity(new Autorizacao
            {
                dtSolicitacao = DateTime.Now,
                fkAssociado = associado.id,
                fkCredenciado = db.currentCredenciado.id,
                fkEmpresa = associado.fkEmpresa,
                fkProcedimento = tuss.id,
                nuAno = dt.Year,
                nuMes = dt.Month,
                tgSituacao = TipoSitAutorizacao.Autorizado,
                fkAutOriginal = null,
                nuIndice = 1,
                nuTotParcelas = proc.nuParcelas,
                vrProcedimento = proc.vrProcedimento,
                vrParcela = proc.vrProcedimento /  proc.nuParcelas,
                vrCoPart = proc.vrCoPart,
                vrParcelaCoPart = proc.vrCoPart / proc.nuParcelas,
            }));

            if (proc.nuParcelas > 1)
            {
                for (int nuParc = 2; nuParc <= proc.nuParcelas; ++nuParc)
                {
                    dt = dt.AddMonths(1);

                    db.Insert(new Autorizacao
                    {
                        dtSolicitacao = DateTime.Now,
                        fkAssociado = associado.id,
                        fkCredenciado = db.currentCredenciado.id,
                        fkEmpresa = associado.fkEmpresa,
                        fkProcedimento = tuss.id,
                        nuAno = dt.Year,
                        nuMes = dt.Month,
                        tgSituacao = TipoSitAutorizacao.Autorizado,
                        fkAutOriginal = idAutOriginal,
                        nuIndice = nuParc,
                        nuTotParcelas = proc.nuParcelas,
                        vrProcedimento = proc.vrProcedimento,
                        vrParcela = proc.vrProcedimento / proc.nuParcelas,
                        vrCoPart = proc.vrCoPart,
                        vrParcelaCoPart = proc.vrCoPart / proc.nuParcelas,
                    });
                }
            }

            return "";
		}
	}
}
