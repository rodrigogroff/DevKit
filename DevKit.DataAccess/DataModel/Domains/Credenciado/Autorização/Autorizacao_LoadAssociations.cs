using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System;
using DevKit.DataAccess;
using SyCrafEngine;

namespace DataModel
{
	public partial class Autorizacao
    {
		public Autorizacao LoadAssociations(DevKitDB db)
		{
            var proc = db.TUSS.Where(y => y.id == fkProcedimento).FirstOrDefault();
            var assoc = db.Associado.Where(y => y.id == fkAssociado).FirstOrDefault();
            var assocPortador = db.Associado.Where(y => y.id == fkAssociadoPortador).FirstOrDefault();
            var cred = db.Credenciado.Where(y => y.id == fkCredenciado).FirstOrDefault();

            if (cred != null )
            {
                var espec = db.Especialidade.Where(y => y.id == cred.fkEspecialidade).FirstOrDefault();
                sfkEspecialidade = espec.stNome;

                snuCodigoCredenciado = cred.nuCodigo.ToString();
                sfkCredenciado = cred.stNome;
            }

            sdtSolicitacao = Convert.ToDateTime(dtSolicitacao).ToString("dd/MM/yyyy HH:mm");

            sfkEmpresa = db.EmpresaSecao.Where(y => y.id == assoc.fkSecao).Select (y=> y.nuEmpresa + " - " + y.stDesc).FirstOrDefault();

            if (proc != null)
                sfkProcedimento = proc.nuCodTUSS + " - " + proc.stProcedimento;

            sfkAssociado = assoc.stName;

            if (assocPortador != null)
            {
                sfkAssociadoPortador = assocPortador.stName;
                sfkAssociadoPortadorTit = assocPortador.nuTitularidade.ToString().PadLeft(2,'0');
            }
            else
                sfkAssociadoPortadorTit = assoc.nuTitularidade.ToString().PadLeft(2, '0');

            snuMatriculaAssociado = assoc.nuMatricula.ToString();

            switch (tgSituacao)
            {
                case TipoSitAutorizacao.Autorizado: stgSituacao = "Autorizado"; break;
                case TipoSitAutorizacao.Confirmado: stgSituacao = "Confirmado"; break;
                case TipoSitAutorizacao.Glosado:    stgSituacao = "Glosado"; break;
                case TipoSitAutorizacao.Rejeitado:  stgSituacao = "Rejeitado"; break;
            }

            LoadCupom(db);

            if (nuTotParcelas == 0)
                nuTotParcelas = 1;

            return this;
		}

        public void LoadCupom(DevKitDB db)
        {
            var portador = db.Associado.FirstOrDefault(y => y.id == this.fkAssociado);
            var secao = db.EmpresaSecao.FirstOrDefault(y => y.id == portador.fkSecao);
            var empresa = db.Empresa.FirstOrDefault(y => y.id == portador.fkEmpresa);

            var cred = new Credenciado();
            var tuss = new TUSS();
            var cet = new CredenciadoEmpresaTuss();

            if (this.fkCredenciado != null)
                cred = db.Credenciado.FirstOrDefault(y => y.id == this.fkCredenciado);
            else
                cred = null;

            if (this.fkProcedimento != null)
            {
                tuss = db.TUSS.FirstOrDefault(y => y.id == this.fkProcedimento);

                if (tuss != null)
                    cet = db.CredenciadoEmpresaTuss.FirstOrDefault(y => y.fkCredenciado == this.fkCredenciado &&
                                                                    y.fkEmpresa == portador.fkEmpresa &&
                                                                    y.nuTUSS == tuss.nuCodTUSS);
                else
                    cet = null;
            }
            else
                tuss = null;

            var dtSol = Convert.ToDateTime(this.dtSolicitacao);

            this.cupom = new CupomAutorizacao
            {
                emissao = dtSol.ToString("dd/MM/yyyy HH:mm"),
                autorizacao = this.nuNSU.ToString(),
                associadoMat = portador.nuMatricula.ToString(),
                associadoNome = portador.stName,
                associadoTit = portador.nuTitularidade.ToString(),
                associadoMatSaude = portador.nuMatSaude != null ? portador.nuMatSaude.ToString() : "",
                credenciado = cred != null ? cred.stNome : "(NÃO FORNECIDO)",
                empresa = empresa.stNome,
                procedimento = tuss != null ? tuss.stProcedimento : "(NÃO FORNECIDO)",
                secao = secao.nuEmpresa.ToString(),
                tuss = tuss != null ? tuss.nuCodTUSS.ToString() : "(NÃO FORNECIDO)",
                vrCoPart = cet != null ? new money().setMoneyFormat((long)cet.vrCoPart) : "-------",
                vrIntegral = cet != null ? new money().setMoneyFormat((long)cet.vrProcedimento) : "-------",
                validade = empresa.nuDiaFech + " / " + dtSol.AddMonths(1).Month + " / " + dtSol.AddMonths(1).Year
            };
        }
    }
}
